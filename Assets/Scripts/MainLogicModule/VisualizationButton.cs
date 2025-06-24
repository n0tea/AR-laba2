using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using static AppController;

public class ARVisualizationButton : MonoBehaviour
{
    [SerializeField]
    private Button onButton;

    [SerializeField]
    private Button offButton;  

    //private VisualizationManager _arVisualizationManager;

    private void Start()
    {
        onButton.gameObject.SetActive(false);
        offButton.gameObject.SetActive(false);
        AppController.Instance.OnViewModeChanged += UpdateUI;
    }

    private void OnEnable()
    {
        onButton.onClick.AddListener(OnVis);
        offButton.onClick.AddListener(OffVis);
    }

    private void OnDisable()
    {
        onButton.onClick.RemoveAllListeners();
        offButton.onClick.RemoveAllListeners();
    }

    private void OnVis()
    {
        AppController.Instance?.SetVisual(true);
        onButton.gameObject.SetActive(false);
        offButton.gameObject.SetActive(true);
    }

    // Запрос на выход из AR-режима
    private void OffVis()
    {
        AppController.Instance?.SetVisual(false);
        onButton.gameObject.SetActive(true);
        offButton.gameObject.SetActive(false);
    }

    private void UpdateUI(AppController.ViewMode mode)
    {
        onButton.gameObject.SetActive(false);
        offButton.gameObject.SetActive(mode == ViewMode.AR);
        
    }
}