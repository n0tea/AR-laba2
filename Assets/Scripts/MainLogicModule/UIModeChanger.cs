using UnityEngine;
using UnityEngine.UI;
using static AppController;

public class UIModeChanger : MonoBehaviour
{
    [SerializeField]
    private Button enterButton; // ������ "Enter AR"

    [SerializeField]
    private Button exitButton;  // ������ "Exit AR"

    private void Start()
    {
        // ������������� ������
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

    // ������ �� ���� � AR-�����
    private void EnterARMode()
    {
        AppController.Instance?.SetViewMode(AppController.ViewMode.AR);
    }

    // ������ �� ����� �� AR-������
    private void ExitARMode()
    {
        AppController.Instance?.SetViewMode(AppController.ViewMode.Mode3D);
    }

    // ����� ��� ���������� UI (���������� �� AppController)
    private void UpdateUI(ViewMode mode)
    {
        enterButton.gameObject.SetActive(mode == ViewMode.Mode3D);
        exitButton.gameObject.SetActive(mode == ViewMode.AR);
    }
}