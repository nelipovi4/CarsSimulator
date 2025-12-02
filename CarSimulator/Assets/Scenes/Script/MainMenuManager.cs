using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Сцены")]
    [SerializeField] private int singlePlayerSceneIndex = 1;
    [SerializeField] private int windowLoadingSceneIndex = 4;
    [SerializeField] private int meltiPlayerSceneIndex = 2;

    [Header("Панели")]
    [SerializeField] private GameObject mainMenuButtons;       // объект Buttons
    [SerializeField] private GameObject aboutAuthorPanel;      // панель "Об авторе"

    [Header("BLUR IMAGE С ШЕЙДЕРОМ")]
    [SerializeField] private GameObject blurImage;             // Image с шейдером blur

    private Button[] menuButtons; // храним все кнопки главного меню


    private void Start()
    {
        CloseAllPanels(); // на старте только главное меню видно
        // Получаем все кнопки в mainMenuButtons
        menuButtons = mainMenuButtons.GetComponentsInChildren<Button>();
    }

    private void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current != null &&
        UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // Если панель "Об авторе" открыта — закрываем её
            if (aboutAuthorPanel.activeSelf)
            {
                CloseAboutAuthor();
            }
        }
    }

    // Открываем "Об авторе"
    public void OpenAboutAuthor()
    {
        // БЛОКИРУЕМ все кнопки главного меню
        SetButtonsInteractable(false);

        aboutAuthorPanel.SetActive(true);

        // ВКЛЮЧАЕМ твой красивый блюр
        if (blurImage != null)
            blurImage.SetActive(true);
    }

    // Закрываем "Об авторе"
    public void CloseAboutAuthor()
    {
        aboutAuthorPanel.SetActive(false);

        // РАЗБЛОКИРУЕМ все кнопки главного меню
        SetButtonsInteractable(true);

        // ВЫКЛЮЧАЕМ блюр
        if (blurImage != null)
            blurImage.SetActive(false);
    }

    // Вспомогательный метод для блокировки/разблокировки кнопок
    private void SetButtonsInteractable(bool interactable)
    {
        foreach (var button in menuButtons)
        {
            if (button != null)
                button.interactable = interactable;
        }
    }

    public void PlaySinglePlayer()
    {
        SceneManager.LoadScene(windowLoadingSceneIndex);
    }

    public void PlayMultiplayer()
    {
        SceneManager.LoadScene(meltiPlayerSceneIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // На всякий случай — чтобы при старте всё было красиво
    private void CloseAllPanels()
    {
        mainMenuButtons.SetActive(true);
        aboutAuthorPanel.SetActive(false);
        if (blurImage != null)
            blurImage.SetActive(false);
    }
}