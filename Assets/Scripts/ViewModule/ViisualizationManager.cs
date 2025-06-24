using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class VisualizationManager : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public ARPointCloudManager pointCloudManager;

    private void Start()
    {
        AppController.Instance.OnVisualChanged += SwitchState;
        SwitchState(AppController.Instance.GetCurrentVisualState());
    }
    private void OnDestroy()
    {
        if (AppController.Instance != null)
        {
            AppController.Instance.OnVisualChanged -= SwitchState;
        }

    }
    public void SwitchState(bool state)
    {
        // ���������/���������� ����������
        if (planeManager != null)
        {
            planeManager.enabled = state;
            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(state);
            }
        }

        // ���������/���������� �����
        if (pointCloudManager != null)
        {
            pointCloudManager.enabled = state;
            foreach (var point in pointCloudManager.trackables)
            {
                point.gameObject.SetActive(state);
            }
        }
    }
}