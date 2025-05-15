using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class ChangesController : MonoBehaviour
{
    public static ChangesController Instance { get; private set; }

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

        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void SetSelectedObject(GameObject prefab)
    {
        SelectedPrefab = prefab;
        OnObjectChanged?.Invoke(prefab);
    }

    public void SetViewMode(ViewMode mode)
    {
        if (CurrentViewMode == mode) return;

        CurrentViewMode = mode;
        OnViewModeChanged?.Invoke(mode);

        // Загрузка/выгрузка сцен в зависимости от режима
        /*if (mode == ViewMode.AR)
        {
            SceneManager.LoadScene("ARScene", LoadSceneMode.Additive);
        }
        else
        {
            SceneManager.UnloadSceneAsync("ARScene");
        }*/
    }

    /*private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "ARScene")
        {
            // При загрузке AR сцены автоматически переключаем режим
            CurrentViewMode = ViewMode.AR;
            OnViewModeChanged?.Invoke(ViewMode.AR);
        }
    }*/

    private void OnDestroy()
    {
        //SceneManager.sceneLoaded -= OnSceneLoaded;
        OnObjectChanged = null;
        OnViewModeChanged = null;
    }
}