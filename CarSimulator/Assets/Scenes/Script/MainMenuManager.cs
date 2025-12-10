using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;
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

    private Button[] menuButtons; // храним все кнопки главного меню
    public InputField create;
    public InputField join;

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
        SceneManager.LoadScene(windowLoadingSceneIndex);
    }

    public void PlayMultiplayer()
    {
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

    // ============= ДЕЙСТВИЯ С КОМНАТАМИ =============
    public void CreateRoomAndGo()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(create.text, roomOptions);
    }

    public void JoinRoomAndGo()
    {
        PhotonNetwork.JoinRoom(join.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("test_online");
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