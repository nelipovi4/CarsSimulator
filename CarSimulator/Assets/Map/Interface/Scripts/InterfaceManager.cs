using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.IO;


// Класс для обработки действий HUD
public class InterfaceManager : MonoBehaviour
{
    [Header("Меню")]
    [SerializeField] private GameObject menuPanel;        // Панель паузы (Menu)

    [Header("Весь игровой HUD")]
    [SerializeField] private GameObject gameHUD;          // Canvas или родитель всех элементов интерфейса в игре

    [Header("BLUR IMAGE С ШЕЙДЕРОМ")]
    [SerializeField] private GameObject blurImage;             // Image с шейдером blur

    [SerializeField] private int mainMenuSceneIndex = 2;

    [Header("Плавное затухание звука")]
    [SerializeField, Range(0.05f, 1f)] private float fadeDuration = 0.25f;
    private Coroutine fadeCoroutine;

    private bool isMenuOpen = false;

    private void Start()
    {
        // На старте всё как надо: меню закрыто, HUD включён, игра не на паузе
        CloseMenuAtStart();

        StopFade();
        fadeCoroutine = StartCoroutine(FadeVolumeTo(1f));
    }

    private void Update()
    {
        // Точно такой же стиль проверки Escape, как в MainMenuManager
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // Если меню уже открыто — закрываем, иначе открываем
            if (isMenuOpen)
                CloseMenu();
            else
                OpenMenu();
        }
    }

    // Открываем меню паузы
    public void OpenMenu()
    {
        GameLogger.LogEvent("MenuOpened", "Type: Pause");

        isMenuOpen = true;

        // Плавно убираем звук — БЕЗ ЩЕЛЧКА
        StopFade();
        fadeCoroutine = StartCoroutine(FadeVolumeTo(0f));

        if (menuPanel != null)
            menuPanel.SetActive(true);

        if (gameHUD != null)
            gameHUD.SetActive(false);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // ВКЛЮЧАЕМ твой красивый блюр
        if (blurImage != null)
            blurImage.SetActive(true);
    }

    // Закрываем меню паузы (Resume)
    public void CloseMenu()
    {
        isMenuOpen = false;

        // Плавно возвращаем звук
        StopFade();
        fadeCoroutine = StartCoroutine(FadeVolumeTo(1f));

        if (menuPanel != null)
            menuPanel.SetActive(false);

        if (gameHUD != null)
            gameHUD.SetActive(true);

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // ВЫКЛЮЧАЕМ блюр
        if (blurImage != null)
            blurImage.SetActive(false);
    }

    // Кнопка "Resume" в меню
    public void ResumeGame()
    {
        CloseMenu();
    }

    // Кнопка "Quit" в меню паузы
    public void QuitGame()
    {
        GameLogger.Log(GameLogger.LogCategory.System, "Выход из игры");

        // Сохраняем позицию перед выходом
        var carSaveSystem = FindObjectOfType<CarSaveSystem>();
        if (carSaveSystem != null)
            carSaveSystem.SaveCarData();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void QuitToMenu()
    {
        // Сохраняем позицию перед выходом
        var carSaveSystem = FindObjectOfType<CarSaveSystem>();
        if (carSaveSystem != null)
            carSaveSystem.SaveCarData();

        // ВАЖНО: возвращаем всё в нормальное состояние ДО загрузки новой сцены
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Если у тебя где-то ещё есть глобальные менеджеры (аудио, музыка и т.д.) — тоже сбрось их состояние здесь

        SceneManager.LoadScene(mainMenuSceneIndex);
    }

    // На старте сцены — гарантируем правильное состояние
    private void CloseMenuAtStart()
    {
        if (menuPanel != null)
            menuPanel.SetActive(false);

        if (gameHUD != null)
            gameHUD.SetActive(true);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isMenuOpen = false;

        if (blurImage != null)
            blurImage.SetActive(false);
    }

    private void StopFade()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }

    private IEnumerator FadeVolumeTo(float target)
    {
        float start = AudioListener.volume;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            AudioListener.volume = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }

        AudioListener.volume = target;
        fadeCoroutine = null;
    }
    public void GetPdf()
    {
        string pdfPath = Path.Combine(Application.dataPath, "Helper", "Documentation.pdf");
        GameLogger.Log(GameLogger.LogCategory.System, $"Попытка открыть PDF: {pdfPath}");

        if (File.Exists(pdfPath))
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = pdfPath,
                    UseShellExecute = true
                });
                GameLogger.Log(GameLogger.LogCategory.System, "PDF успешно открыт");
            }
            catch (System.Exception e)
            {
                GameLogger.LogError(GameLogger.LogCategory.Error,
                    $"Ошибка открытия PDF: {e.Message}");
            }
        }
        else
        {
            GameLogger.LogError(GameLogger.LogCategory.Error,
                $"PDF файл не найден: {pdfPath}");
        }
    }
}