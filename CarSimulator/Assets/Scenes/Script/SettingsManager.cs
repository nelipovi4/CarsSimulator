using Photon.Pun;
using Photon.Realtime;
using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


// Класс-обработчик действий главного меню
public class SettingsManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject settingsPanel;         // панель настроек
    [SerializeField] private GameObject menuPanel;             // панель главного меню

    [Header("BLUR IMAGE С ШЕЙДЕРОМ")]
    [SerializeField] private GameObject blurImage;             // Image с шейдером blur

    [Header("Меню настроек")]
    [SerializeField] private GameObject graphicsMenu;          // ScrollViewGraphics
    [SerializeField] private GameObject soundMenu;             // ScrollViewSounds
    [SerializeField] private GameObject languageMenu;          // ScrollViewLanguage
    [SerializeField] private Button graphicsButton;            // Кнопка "Графика"
    [SerializeField] private Button soundButton;               // Кнопка "Звук"
    [SerializeField] private Button languageButton;            // Кнопка "Язык"

    [Header("Цвета кнопок настроек")]
    [SerializeField] private Color activeTabColor = new Color(1f, 0f, 0f, 1f);      // Красный для активной
    [SerializeField] private Color inactiveTabColor = new Color(0.7f, 0.7f, 0.7f, 1f); // Серый для неактивной

    [Header("Менеджеры настроек")]
    [SerializeField] private GameObject graphicsSettings;  // Менеджер графики


    private void Start()
    {
        CloseAllPanels(); // на старте только главное меню видно

        // ЛОГИРОВАНИЕ ЗАПУСКА
        GameLogger.Log(GameLogger.LogCategory.System, "Главное меню загружено");
    }

    private void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // Приоритет: сначала проверяем самые глубокие уровни
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                CloseSettings();
            }
        }
    }

    // ============= НАСТРОЙКИ =============
    public void OpenSettings()
    {
        // ЗАКРЫВАЕМ главное меню
        if (menuPanel != null)
            menuPanel.SetActive(false);

        // ОТКРЫВАЕМ настройки
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);

            // По умолчанию открываем вкладку "Графика"
            ShowGraphicsMenu();
        }

        if (blurImage != null)
            blurImage.SetActive(true);

        GameLogger.LogEvent("SettingsOpened", "Settings panel opened");
    }

    public void CloseSettings()
    {
        // ЗАКРЫВАЕМ настройки
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        DeselectAllButtons();

        if (blurImage != null)
            blurImage.SetActive(false);

        GameLogger.LogEvent("SettingsClosed", "Settings panel closed");
    }

    // Переключение на вкладку "Графика"
    public void ShowGraphicsMenu()
    {
        if (graphicsMenu != null)
            graphicsMenu.SetActive(true);

        if (soundMenu != null)
            soundMenu.SetActive(false);

        if (languageMenu != null)
            languageMenu.SetActive(false);

        // Загружаем настройки графики при открытии вкладки
        //if (graphicsSettings != null)
            //graphicsSettings.LoadSettings();

        // Визуальная индикация активной кнопки
        UpdateTabButtonsState(graphicsButton);

        GameLogger.LogEvent("SettingsTab", "Tab: Graphics");
    }

    // Переключение на вкладку "Звук"
    public void ShowSoundMenu()
    {
        if (graphicsMenu != null)
            graphicsMenu.SetActive(false);

        if (soundMenu != null)
            soundMenu.SetActive(true);

        if (languageMenu != null)
            languageMenu.SetActive(false);

        // Визуальная индикация активной кнопки
        UpdateTabButtonsState(soundButton);

        GameLogger.LogEvent("SettingsTab", "Tab: Sound");
    }

    // Переключение на вкладку "Язык"
    public void ShowLanguageMenu()
    {
        if (graphicsMenu != null)
            graphicsMenu.SetActive(false);

        if (soundMenu != null)
            soundMenu.SetActive(false);

        if (languageMenu != null)
            languageMenu.SetActive(true);

        // Визуальная индикация активной кнопки
        UpdateTabButtonsState(languageButton);

        GameLogger.LogEvent("SettingsTab", "Tab: Language");
    }

    // Обновление состояния кнопок вкладок (визуальная индикация)
    private void UpdateTabButtonsState(Button activeButton)
    {
        // Обновляем кнопку "Графика"
        if (graphicsButton != null)
        {
            graphicsButton.interactable = (graphicsButton != activeButton);
            var colors = graphicsButton.colors;
            colors.normalColor = (graphicsButton == activeButton) ? activeTabColor : inactiveTabColor;
            colors.disabledColor = activeTabColor; // Цвет для выбранной (неактивной) кнопки
            graphicsButton.colors = colors;
        }

        // Обновляем кнопку "Звук"
        if (soundButton != null)
        {
            soundButton.interactable = (soundButton != activeButton);
            var colors = soundButton.colors;
            colors.normalColor = (soundButton == activeButton) ? activeTabColor : inactiveTabColor;
            colors.disabledColor = activeTabColor;
            soundButton.colors = colors;
        }

        // Обновляем кнопку "Язык"
        if (languageButton != null)
        {
            languageButton.interactable = (languageButton != activeButton);
            var colors = languageButton.colors;
            colors.normalColor = (languageButton == activeButton) ? activeTabColor : inactiveTabColor;
            colors.disabledColor = activeTabColor;
            languageButton.colors = colors;
        }
    }

    // НОВЫЙ МЕТОД: Снимает выделение со всех кнопок
    private void DeselectAllButtons()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void CloseAllPanels()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (menuPanel != null)
            menuPanel.SetActive(false);

        if (blurImage != null)
            blurImage.SetActive(false);
    }
}