using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation.Samples;
using UnityEngine.UI;

[RequireComponent(typeof(ARRaycastManager))]
public class ARInteractionManager : PressInputBase
{
    [Header("Settings")]
    [SerializeField] private float _longPressDuration = 0.5f;
    [SerializeField] private GameObject _deleteButtonPrefab;

    private ARRaycastManager _raycastManager;
    private Transform _arSceneRoot;
    private GameObject _currentObject;
    private GameObject _deleteButton;
    private bool _isDragging;
    private float _pressTime;
    private bool _isLongPress;
    private static List<ARRaycastHit> _hits = new List<ARRaycastHit>();

    protected override void Awake()
    {
        base.Awake();
        _raycastManager = GetComponent<ARRaycastManager>();
        FindARSceneRoot();
        CreateDeleteButton();
    }

    private void CreateDeleteButton()
    {
        if (_deleteButtonPrefab != null)
        {
            _deleteButton = Instantiate(_deleteButtonPrefab);
            _deleteButton.SetActive(false);
            _deleteButton.GetComponent<Button>().onClick.AddListener(DeleteCurrentObject);
        }
    }

    private void FindARSceneRoot()
    {
        Scene arScene = SceneManager.GetSceneByName("ARModule");
        if (arScene.IsValid())
        {
            foreach (var obj in arScene.GetRootGameObjects())
            {
                if (obj.CompareTag("ARRoot"))
                {
                    _arSceneRoot = obj.transform;
                    return;
                }
            }

            GameObject root = new GameObject("ARRoot");
            SceneManager.MoveGameObjectToScene(root, arScene);
            root.tag = "ARRoot";
            _arSceneRoot = root.transform;
        }
    }

    private void OnEnable()
    {
        base.OnEnable();
        AppController.Instance.OnObjectChanged += HandleObjectChanged;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        base.OnDisable();
        AppController.Instance.OnObjectChanged -= HandleObjectChanged;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "ARModule") FindARSceneRoot();
    }

    private void HandleObjectChanged(GameObject prefab)
    {
        ClearCurrentObject();
        if (prefab != null && _arSceneRoot != null)
        {
            _currentObject = Instantiate(prefab, _arSceneRoot);
            _currentObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject() ||
            Pointer.current == null ||
            _arSceneRoot == null) return;

        HandleTouchInput();
        UpdateObjectPosition();
    }

    private void HandleTouchInput()
    {
        var touch = Pointer.current;

        // Начало касания
        if (touch.press.wasPressedThisFrame)
        {
            _pressTime = Time.time;
            _isLongPress = false;

            if (TryGetTouchedObject(out var hitObject))
            {
                _currentObject = hitObject;
                _isDragging = true;
                HideDeleteButton();
            }
        }

        // Долгое нажатие
        if (touch.press.isPressed && !_isLongPress &&
            Time.time - _pressTime > _longPressDuration)
        {
            _isLongPress = true;
            if (_currentObject != null)
            {
                ShowDeleteButton();
            }
        }

        // Окончание касания
        if (touch.press.wasReleasedThisFrame)
        {
            if (!_isLongPress && _currentObject == null &&
                AppController.Instance.SelectedPrefab != null)
            {
                CreateNewObject();
            }
            _isDragging = false;
        }
    }

    private bool TryGetTouchedObject(out GameObject hitObject)
    {
        var ray = Camera.main.ScreenPointToRay(
            Pointer.current.position.ReadValue());

        if (Physics.Raycast(ray, out var hit))
        {
            hitObject = hit.collider.gameObject;
            return true;
        }

        hitObject = null;
        return false;
    }

    private void UpdateObjectPosition()
    {
        if (!_isDragging || _currentObject == null) return;

        var touchPosition = Pointer.current.position.ReadValue();
        if (_raycastManager.Raycast(touchPosition, _hits, TrackableType.PlaneWithinPolygon))
        {
            _currentObject.transform.SetPositionAndRotation(
                _hits[0].pose.position,
                _hits[0].pose.rotation);
        }
    }

    private void CreateNewObject()
    {
        ClearCurrentObject();
        _currentObject = Instantiate(
            AppController.Instance.SelectedPrefab,
            _arSceneRoot);
        _currentObject.SetActive(true);
        _isDragging = true;
    }

    private void ShowDeleteButton()
    {
        if (_deleteButton == null || _currentObject == null) return;

        _deleteButton.SetActive(true);
        _deleteButton.transform.position =
            _currentObject.transform.position + Vector3.up * 0.2f;
    }

    private void HideDeleteButton() => _deleteButton?.SetActive(false);

    private void DeleteCurrentObject()
    {
        ClearCurrentObject();
        HideDeleteButton();
    }

    private void ClearCurrentObject()
    {
        if (_currentObject != null)
        {
            Destroy(_currentObject);
            _currentObject = null;
        }
    }

    protected override void OnPress(Vector3 position) { }
    protected override void OnPressCancel() { }
}