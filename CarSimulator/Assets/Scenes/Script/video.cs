using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class VideoIntroController : MonoBehaviour
{
    [Header("Компоненты")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Canvas mainCanvas;

    [Header("Настройки")]
    [SerializeField] private float delayAfterVideo = 1f;   // ← Здесь настраивай задержку (0.3–0.8 сек обычно идеально)
    [SerializeField] private bool skipWithAnyKey = true;     // Пропуск по нажатию
    [SerializeField] private Camera mainCamera;

    private void Awake()
    {
        // Автоподхват VideoPlayer, если забыли перетащить
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        // Скрываем канвас ещё до первого кадра
        if (mainCanvas != null)
            mainCanvas.gameObject.SetActive(false);

        mainCamera.backgroundColor = Color.black;
    }

    private void Start()
    {
        videoPlayer.isLooping = false;
        videoPlayer.loopPointReached += OnVideoFinished; // главное событие — конец видео

        videoPlayer.Prepare();                                      // начинаем загрузку
        videoPlayer.prepareCompleted += OnVideoReady;               // подписываемся на событие «видео готово»

        // Пока видео не готово — показываем чёрный экран
        if (mainCamera != null)
            mainCamera.backgroundColor = Color.black;
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        // Запускаем корутину с задержкой
        StartCoroutine(ShowMenuAfterDelay());
    }

    private IEnumerator ShowMenuAfterDelay()
    {
        // Ждём небольшую паузу после последнего кадра видео (по желанию)
        yield return new WaitForSeconds(delayAfterVideo);

        // Делаем фон камеры прозрачным (чтобы был виден последний кадр из RenderTexture)
        if (mainCamera != null)
            mainCamera.backgroundColor = Color.clear;

        // Включаем Canvas, но сразу с альфой = 0
        if (mainCanvas != null)
        {
            mainCanvas.gameObject.SetActive(true);

            // Добавляем (или берём существующий) CanvasGroup
            CanvasGroup cg = mainCanvas.GetComponent<CanvasGroup>();
            if (cg == null) cg = mainCanvas.gameObject.AddComponent<CanvasGroup>();

            cg.alpha = 0f;           // полностью прозрачный
            cg.blocksRaycasts = false; // пока не мешает кликам

            // Плавный фейд-ин
            float fadeTime = 0.8f;   // ← длительность появления (настраивай под себя)
            float elapsed = 0f;

            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Clamp01(elapsed / fadeTime);
                yield return null;
            }

            cg.alpha = 1f;
            cg.blocksRaycasts = true; // теперь кнопки кликабельны
        }

        Destroy(gameObject);
    }

    // Пропуск видео по нажатию любой клавиши / клику
    private void Update()
    {

    }



    private void OnVideoReady(VideoPlayer vp)
    {
        // Отписываемся, чтобы не сработало дважды
        videoPlayer.prepareCompleted -= OnVideoReady;

        //ЗАДЕРЖКА ПЕРЕД ПОЯВЛЕНИЕМ МЕНЮ
        StartCoroutine(DelayBeforePlay(5)); // 0.2 сек — идеально, можно 0f или 0.5f
    }

    private IEnumerator DelayBeforePlay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        // ТЕПЕРЬ ТОЧНО ВСЁ ГОТОВО — запускаем видео
        videoPlayer.Play();
    }
}

