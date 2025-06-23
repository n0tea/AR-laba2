using UnityEngine;
using UnityEngine.UI;
using static AppController;

public class UIModeChanger : MonoBehaviour
{
    [SerializeField]
    private Button enterButton; // Кнопка "Enter AR"

    [SerializeField]
    private Button exitButton;  // Кнопка "Exit AR"

    private void Start()
    {
        // Инициализация кнопок
        enterButton.gameObject.SetActive(true);
        exitButton.gameObject.SetActive(false);
        AppController.Instance.OnViewModeChanged += UpdateUI;
    }

    private void OnEnable()
    {
        enterButton.onClick.AddListener(EnterARMode);
        exitButton.onClick.AddListener(ExitARMode);
    }

    private void OnDisable()
    {
        enterButton.onClick.RemoveAllListeners();
        exitButton.onClick.RemoveAllListeners();
    }

    // Запрос на вход в AR-режим
    private void EnterARMode()
    {
        AppController.Instance?.SetViewMode(AppController.ViewMode.AR);
    }

    // Запрос на выход из AR-режима
    private void ExitARMode()
    {
        AppController.Instance?.SetViewMode(AppController.ViewMode.Mode3D);
    }

    // Метод для обновления UI (вызывается из AppController)
    private void UpdateUI(ViewMode mode)
    {
        enterButton.gameObject.SetActive(mode == ViewMode.Mode3D);
        exitButton.gameObject.SetActive(mode == ViewMode.AR);
    }
}