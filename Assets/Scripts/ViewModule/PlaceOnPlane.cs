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
    private GameObject spawnedObject;
    private ARRaycastManager m_RaycastManager;
    private Transform arSceneRoot; // Родительский объект в ARScene

    bool m_Pressed;

    protected override void Awake()
    {
        base.Awake();
        m_RaycastManager = GetComponent<ARRaycastManager>();

        // Находим корневой объект AR сцены
        FindARSceneRoot();
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

                // Если не нашли - создаём новый
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
            spawnedObject = Instantiate(AppController.Instance.SelectedPrefab, arSceneRoot);
            spawnedObject.SetActive(false);
        }
    }

    private void HandleObjectChanged(GameObject prefab)
    {
        if (spawnedObject != null)
        {
            Destroy(spawnedObject);
        }

        if (prefab != null && arSceneRoot != null)
        {
            spawnedObject = Instantiate(prefab, arSceneRoot);
            spawnedObject.SetActive(false);
        }
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject() ||
            Pointer.current == null ||
            !m_Pressed ||
            arSceneRoot == null)
        {
            return;
        }

        var touchPosition = Pointer.current.position.ReadValue();

        if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = s_Hits[0].pose;

            if (spawnedObject != null)
            {
                if (!spawnedObject.activeSelf)
                {
                    spawnedObject.SetActive(true);
                }

                spawnedObject.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
            }
        }
    }

    protected override void OnPress(Vector3 position) => m_Pressed = true;
    protected override void OnPressCancel() => m_Pressed = false;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
}