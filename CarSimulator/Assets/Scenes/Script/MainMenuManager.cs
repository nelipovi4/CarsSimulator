using Photon.Pun;
using Photon.Realtime;
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// Класс-обработчик действий главного меню
public class MainMenuManager : MonoBehaviourPunCallbacks
{
    [Header("Сцены")]
    [SerializeField] private int singlePlayerSceneIndex = 1;
    [SerializeField] private int windowLoadingSceneIndex = 4;
    [SerializeField] private int meltiPlayerSceneIndex = 2;

    [Header("Панели")]
    [SerializeField] private GameObject mainMenuButtons;       // объект Buttons
    [SerializeField] private GameObject aboutAuthorPanel;      // панель "Об авторе"
    [SerializeField] private GameObject multiplayerRoomPanel;  // панель MultiplayerRoomPanel
    [SerializeField] private GameObject ButtonsMultiPlayerRoomPanel;  // кнопки MultiplayerButtons
    [SerializeField] private GameObject createRoomPanel;       // панель CreateRoomPanel
    [SerializeField] private GameObject joinRoomPanel;         // панель JoinRoomPanel

    [Header("BLUR IMAGE С ШЕЙДЕРОМ")]
    [SerializeField] private GameObject blurImage;             // Image с шейдером blur

    [Header("Валидация")]
    [SerializeField] private RoomValidator validator;          // Валидатор для проверки названий комнат
    [SerializeField] private Text errorMessageText;            // Текст для отображения ошибок (опционально)

    private Button[] menuButtons; // храним все кнопки главного меню
    public InputField create;
    public InputField join;

    private void Start()
    {
        CloseAllPanels(); // на старте только главное меню видно
        // Получаем все кнопки в mainMenuButtons
        menuButtons = mainMenuButtons.GetComponentsInChildren<Button>();

        // Если валидатор не назначен, ищем или создаем
        if (validator == null)
        {
            validator = FindObjectOfType<RoomValidator>();
            if (validator == null)
            {
                GameObject go = new GameObject("RoomValidator");
                validator = go.AddComponent<RoomValidator>();
            }
        }

        // Скрываем текст ошибки, если он есть
        if (errorMessageText != null)
            errorMessageText.gameObject.SetActive(false);

        // ЛОГИРОВАНИЕ ЗАПУСКА
        GameLogger.Log(GameLogger.LogCategory.System, "Главное меню загружено");
    }

    private void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // Приоритет: сначала проверяем самые глубокие уровни
            if (createRoomPanel != null && createRoomPanel.activeSelf)
            {
                CloseCreateRoom();
            }
            else if (joinRoomPanel != null && joinRoomPanel.activeSelf)
            {
                CloseJoinRoom();
            }
            else if (multiplayerRoomPanel != null && multiplayerRoomPanel.activeSelf)
            {
                CloseMultiplayerRoom();
            }
            else if (aboutAuthorPanel.activeSelf)
            {
                CloseAboutAuthor();
            }
        }
    }

    // ============= ГЛАВНОЕ МЕНЮ =============
    public void PlaySinglePlayer()
    {
        GameLogger.LogSceneLoad("SinglePlayer");
        GameLogger.LogEvent("GameModeSelected", "Mode: SinglePlayer");

        SceneManager.LoadScene(windowLoadingSceneIndex);
    }

    public void PlayMultiplayer()
    {
        GameLogger.LogEvent("GameModeSelected", "Mode: Multiplayer");
        GameLogger.LogNetworkEvent("ConnectAttempt", "Connecting to Photon...");

        PhotonNetwork.ConnectUsingSettings();
        // Открываем панель выбора комнаты вместо загрузки сцены
        OpenMultiplayerRoom();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ============= ОБ АВТОРЕ =============
    public void OpenAboutAuthor()
    {
        SetButtonsInteractable(false);
        aboutAuthorPanel.SetActive(true);

        if (blurImage != null)
            blurImage.SetActive(true);
    }

    public void CloseAboutAuthor()
    {
        aboutAuthorPanel.SetActive(false);
        SetButtonsInteractable(true);
        DeselectAllButtons(); // Снимаем выделение

        if (blurImage != null)
            blurImage.SetActive(false);
    }

    // ============= МУЛЬТИПЛЕЕР ПАНЕЛИ =============
    public void OpenMultiplayerRoom()
    {
        mainMenuButtons.SetActive(false);

        if (multiplayerRoomPanel != null)
        {
            multiplayerRoomPanel.SetActive(true);
            ButtonsMultiPlayerRoomPanel.SetActive(true);
        }

        // Убеждаемся что другие панели закрыты
        if (createRoomPanel != null)
            createRoomPanel.SetActive(false);

        if (joinRoomPanel != null)
            joinRoomPanel.SetActive(false);

        if (blurImage != null)
            blurImage.SetActive(true);
    }

    public void CloseMultiplayerRoom()
    {
        if (multiplayerRoomPanel != null)
        {
            multiplayerRoomPanel.SetActive(false);
        }

        if (ButtonsMultiPlayerRoomPanel != null)
            ButtonsMultiPlayerRoomPanel.SetActive(false);

        if (createRoomPanel != null)
            createRoomPanel.SetActive(false);

        if (joinRoomPanel != null)
            joinRoomPanel.SetActive(false);

        mainMenuButtons.SetActive(true);
        DeselectAllButtons(); // Снимаем выделение со всех кнопок

        if (blurImage != null)
            blurImage.SetActive(false);
    }

    // ============= СОЗДАНИЕ КОМНАТЫ =============
    public void OpenCreateRoom()
    {
        // Скрываем кнопки мультиплеера
        if (ButtonsMultiPlayerRoomPanel != null)
            ButtonsMultiPlayerRoomPanel.SetActive(false);

        // Показываем панель создания комнаты
        if (createRoomPanel != null)
            createRoomPanel.SetActive(true);
    }

    public void CloseCreateRoom()
    {
        // Закрываем панель создания комнаты
        if (createRoomPanel != null)
            createRoomPanel.SetActive(false);

        // Возвращаем кнопки мультиплеера
        if (ButtonsMultiPlayerRoomPanel != null)
            ButtonsMultiPlayerRoomPanel.SetActive(true);
    }

    // ============= ПРИСОЕДИНЕНИЕ К КОМНАТЕ =============
    public void OpenJoinRoom()
    {
        // Скрываем кнопки мультиплеера
        if (ButtonsMultiPlayerRoomPanel != null)
            ButtonsMultiPlayerRoomPanel.SetActive(false);

        // Показываем панель присоединения
        if (joinRoomPanel != null)
            joinRoomPanel.SetActive(true);
    }

    public void CloseJoinRoom()
    {
        // Закрываем панель присоединения
        if (joinRoomPanel != null)
            joinRoomPanel.SetActive(false);

        // Возвращаем кнопки мультиплеера
        if (ButtonsMultiPlayerRoomPanel != null)
            ButtonsMultiPlayerRoomPanel.SetActive(true);
    }

    // ============= ДЕЙСТВИЯ С КОМНАТАМИ С ВАЛИДАЦИЕЙ =============
    public void CreateRoomAndGo()
    {
        string roomName = create.text;

        // ВАЛИДАЦИЯ НАЗВАНИЯ КОМНАТЫ
        var result = validator.ValidateRoomName(roomName);

        if (!result.isValid)
        {
            // Показываем ошибку пользователю
            ShowErrorMessage($"Ошибка: {result.errorMessage}");

            // Логируем попытку создания комнаты с невалидным названием
            GameLogger.LogWarning(GameLogger.LogCategory.Network,
                $"Попытка создать комнату с невалидным названием: '{roomName}' - {result.errorMessage}");
            return;
        }

        // Используем очищенное название
        string cleanRoomName = result.sanitizedValue;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;

        GameLogger.LogNetworkEvent("CreateRoom",
            $"Room: {cleanRoomName}, MaxPlayers: {roomOptions.MaxPlayers}");

        PhotonNetwork.CreateRoom(cleanRoomName, roomOptions);
    }

    public void JoinRoomAndGo()
    {
        PhotonNetwork.JoinRoom(join.text);
    }

    public override void OnJoinedRoom()
    {
        GameLogger.LogNetworkEvent("RoomJoined",
            $"Room: {PhotonNetwork.CurrentRoom.Name}, Players: {PhotonNetwork.CurrentRoom.PlayerCount}");

        PhotonNetwork.LoadLevel("test_online");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ShowErrorMessage($"Не удалось создать комнату: {message}");
        GameLogger.LogError(GameLogger.LogCategory.Network,
            $"CreateRoom failed: Code {returnCode}, Message: {message}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ShowErrorMessage($"Не удалось присоединиться: {message}");
        GameLogger.LogError(GameLogger.LogCategory.Network,
            $"JoinRoom failed: Code {returnCode}, Message: {message}");
    }

    // ============= ПОКАЗ СООБЩЕНИЙ ОБ ОШИБКАХ =============
    private void ShowErrorMessage(string message)
    {
        // Если есть UI элемент для ошибок - показываем
        if (errorMessageText != null)
        {
            errorMessageText.text = message;
            errorMessageText.gameObject.SetActive(true);

            // Автоматически скрываем через 3 секунды
            CancelInvoke(nameof(HideErrorMessage));
            Invoke(nameof(HideErrorMessage), 3f);
        }
        else
        {
            // Если UI нет - выводим в консоль
            UnityEngine.Debug.LogWarning(message);
        }
    }

    private void HideErrorMessage()
    {
        if (errorMessageText != null)
            errorMessageText.gameObject.SetActive(false);
    }

    // ============= ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ =============
    private void SetButtonsInteractable(bool interactable)
    {
        foreach (var button in menuButtons)
        {
            if (button != null)
                button.interactable = interactable;
        }
    }

    // НОВЫЙ МЕТОД: Снимает выделение со всех кнопок
    private void DeselectAllButtons()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void CloseAllPanels()
    {
        mainMenuButtons.SetActive(true);
        aboutAuthorPanel.SetActive(false);

        if (multiplayerRoomPanel != null)
            multiplayerRoomPanel.SetActive(false);

        if (ButtonsMultiPlayerRoomPanel != null)
            ButtonsMultiPlayerRoomPanel.SetActive(false);

        if (createRoomPanel != null)
            createRoomPanel.SetActive(false);

        if (joinRoomPanel != null)
            joinRoomPanel.SetActive(false);

        if (blurImage != null)
            blurImage.SetActive(false);
    }
}