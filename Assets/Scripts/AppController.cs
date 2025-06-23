using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;

public class AppController : MonoBehaviour
{
    public static AppController Instance { get; private set; }

    public event Action<GameObject> OnObjectChanged;
    public event Action<ViewMode> OnViewModeChanged;

    public GameObject SelectedPrefab { get; private set; }
    public ViewMode CurrentViewMode { get; private set; } = ViewMode.Mode3D;

    public enum ViewMode { Mode3D, AR }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetViewMode(ViewMode mode)
    {
        if (CurrentViewMode == mode)
            return;

        CurrentViewMode = mode;
        OnViewModeChanged?.Invoke(mode);

        switch (mode)
        {
            case ViewMode.AR:
                StartAR();
                break;
            case ViewMode.Mode3D:
                StopAR();
                break;
        }
    }

    private void StartAR()
    {
        PermissionRequestService.RequestCameraPermission((granted) =>
        {
            if (granted)
            {
                SceneManager.LoadScene("ARModule", LoadSceneMode.Additive);
                InitializeXR();
            }
            else
            {
                Debug.LogError("Camera permission denied. Cannot start AR.");
                SetViewMode(ViewMode.Mode3D); // fallback
            }
        });
    }

    private void StopAR()
    {
        SceneManager.UnloadSceneAsync("ARModule");
        DeinitializeXR();
    }

    private void InitializeXR() //start session
    {
        var xrManager = XRGeneralSettings.Instance?.Manager;
        if (xrManager != null)
        {
            StartCoroutine(xrManager.InitializeLoader());
            xrManager.StartSubsystems();
        }
    }

    private void DeinitializeXR() //end session
    {
        var xrManager = XRGeneralSettings.Instance?.Manager;
        if (xrManager != null)
        {
            xrManager.StopSubsystems();
            xrManager.DeinitializeLoader();
        }
    }

    public void SetSelectedObject(GameObject prefab)
    {
        SelectedPrefab = prefab;
        OnObjectChanged?.Invoke(prefab);
    }

    private void OnDestroy()
    {
        OnObjectChanged = null;
        OnViewModeChanged = null;
    }
}