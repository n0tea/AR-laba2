//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Management;

public class UIModeChanger : MonoBehaviour
{
    /*[SerializeField]
    private ARObjectData arObjectData; // ������ �� ScriptableObject

    // ����� ��� ������ �������
    public void SelectObject(GameObject objectPrefab)
    {
        arObjectData.objectPrefab = objectPrefab;
    }*/

    [SerializeField]
    private ObjectSelector objectSelector;
    [SerializeField]
    private Button enterButton; // ������ "Enter"
    [SerializeField]
    private Button exitButton;  // ������ "Exit"

    private void Start()
    {
        // ������������� ������ ��� ������
        enterButton.gameObject.SetActive(true); // ���������� ������ "Enter"
        exitButton.gameObject.SetActive(false); // �������� ������ "Exit"
        
    }
    private void OnEnable()
    {
        enterButton.onClick.AddListener(LoadARScene);
        exitButton.onClick.AddListener(UnloadARScene);
    }
    private void OnDisable()
    {
        exitButton.onClick.RemoveAllListeners();
        enterButton.onClick.RemoveAllListeners();
    }
    // ����� ��� ������� AR-�����
    public void LoadARScene()
    {
        void StartSession()
        {
            XRGeneralSettings instance = XRGeneralSettings.Instance;
            if (instance != null)
            {
                StartCoroutine(instance.Manager.InitializeLoader());
                instance.Manager.StartSubsystems();
            }
        }
        PermissionRequestService.RequestCameraPermission((granted) =>
        {
            if (granted)
            {
                SceneManager.LoadScene("ARModule", LoadSceneMode.Additive);
                enterButton.gameObject.SetActive(false);
                exitButton.gameObject.SetActive(true);
                AppState.Instance.IsARModuleLoaded = true;
                //ObjectSelector.currentObject.SetActive(false);
                StartSession();
            }
            else
            {
                Debug.LogError("artn Camera permission not granted. Cannot load AR scene.");
            }
        });
        
    }

    public async void UnloadARScene()
    {

        void EndSession()
        {
            XRGeneralSettings instance = XRGeneralSettings.Instance;
            if (instance != null)
            {
                instance.Manager.DeinitializeLoader();
                instance.Manager.StopSubsystems();
            }
        }
        EndSession();

        Debug.Log("artn Unloading AR scene...");
        var asyncOperation = SceneManager.UnloadSceneAsync("ARModule");
        await asyncOperation; // ���� ���������� ��������
        AppState.Instance.IsARModuleLoaded = false;

        if (objectSelector != null)
        {
            objectSelector.UpdateSelectedObject();
        }
        
        //ObjectSelector.currentObject.SetActive(true);
        enterButton.gameObject.SetActive(true);
        Debug.Log("artn Changed button...");
        exitButton.gameObject.SetActive(false);
        Debug.Log("artn AR scene unloaded successfully. Enter button is active, Exit button is inactive.");
    }
}