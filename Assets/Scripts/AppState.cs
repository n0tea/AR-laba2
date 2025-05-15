using UnityEngine;

public class AppState : MonoBehaviour
{
    private static AppState _instance;

    public static AppState Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<AppState>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<AppState>();
                    singletonObject.name = typeof(AppState).ToString();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    // State variables
    private bool _isARModuleLoaded = false;
    public bool IsARModuleLoaded
    {
        get { return _isARModuleLoaded; }
        set { _isARModuleLoaded = value; } // для изменения
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}