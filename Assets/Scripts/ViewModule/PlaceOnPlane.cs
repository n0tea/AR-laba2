using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation.Samples;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlane : PressInputBase
{
    public Camera _camera;
    private GameObject selectedPrefab;
    private GameObject currentObject;
    private ARRaycastManager m_RaycastManager;
    private Transform arSceneRoot; // Родительский объект в ARScene
    private List<GameObject> placedObjects = new List<GameObject>();

    private bool isPlacing = false;
    bool m_Pressed;

    protected override void Awake()
    {
        base.Awake();
        m_RaycastManager = GetComponent<ARRaycastManager>();

        // Находим корневой объект AR сцены
        //FindARSceneRoot();
    }

    private void FindARSceneRoot()
    {
        Scene arScene = SceneManager.GetSceneByName("ARModule");
        if (arScene.IsValid())
        {
            GameObject[] rootObjects = arScene.GetRootGameObjects();
            if (rootObjects.Length > 0)
            {
                // Ищем объект с тегом "ARRoot" или создаём новый
                foreach (var obj in rootObjects)
                {
                    if (obj.CompareTag("ARRoot"))
                    {
                        arSceneRoot = obj.transform;
                        return;
                    }
                }
                GameObject root = new GameObject("ARRoot");
                SceneManager.MoveGameObjectToScene(root, arScene);
                root.tag = "ARRoot";
                arSceneRoot = root.transform;
            }
        }
    }

    private void OnEnable()
    {
        base.OnEnable();
        AppController.Instance.OnObjectChanged += HandleObjectChanged;
        SceneManager.sceneLoaded += OnARSceneLoaded;
    }

    private void OnDisable()
    {
        base.OnDisable();
        AppController.Instance.OnObjectChanged -= HandleObjectChanged;
        SceneManager.sceneLoaded -= OnARSceneLoaded;
    }

    private void OnARSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "ARModule")
        {
            FindARSceneRoot();
            selectedPrefab = AppController.Instance.SelectedPrefab;
            /*selectedObject = Instantiate(AppController.Instance.SelectedPrefab, arSceneRoot);
            selectedObject.SetActive(false);*/
        }
    }

    private void HandleObjectChanged(GameObject prefab)
    {
        /*if (selectedObject != null)
        {
            Destroy(selectedObject);
        }

        if (prefab != null && arSceneRoot != null)
        {
            selectedObject = Instantiate(prefab, arSceneRoot);
            selectedObject.SetActive(false);
        }*/
        selectedPrefab = prefab;
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject() ||
            Pointer.current == null ||
            !m_Pressed ||
            arSceneRoot == null ||
            selectedPrefab == null)
        {
            return;
        }

        var touchPosition = Pointer.current.position.ReadValue();
        if (!isPlacing  )
        {
            Ray ray = _camera.ScreenPointToRay(touchPosition);
            RaycastHit hit;

            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f);

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit object: " + hit.collider.name);
                GameObject hitObject = hit.collider.transform.parent.gameObject;
                //GameObject child = hit.collider.gameObject;
                if (placedObjects.Contains(hitObject))
                {
                    currentObject = hitObject;
                    
                    isPlacing = true;
                    return;
                }
            }
        }

        if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = s_Hits[0].pose;

            if (!isPlacing)
            {
                currentObject = Instantiate(selectedPrefab, hitPose.position, hitPose.rotation, arSceneRoot);
                placedObjects.Add(currentObject);
                isPlacing = true;
            }
            else
            {
                var outline = currentObject.GetComponentInChildren<Outline>();
                outline.enabled = true;
                currentObject.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
            }
            /*if (selectedObject != null)
            {
                if (!selectedObject.activeSelf)
                {
                    selectedObject.SetActive(true);
                }

                selectedObject.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
            }*/
        }
    }

    protected override void OnPress(Vector3 position) => m_Pressed = true;
    protected override void OnPressCancel() {
        m_Pressed = false;
        isPlacing = false;
        if (currentObject != null)
        {
            var outline = currentObject.GetComponentInChildren<Outline>();
            outline.enabled = false;
        }
    }
    

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
}