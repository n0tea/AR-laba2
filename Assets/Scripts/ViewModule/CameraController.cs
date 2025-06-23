using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleCameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform _target; // Объект, вокруг которого вращаемся

    [Header("Rotation")]
    [SerializeField] private float _rotationSpeed = 80f;
    [SerializeField] private float _minVerticalAngle = -20f;
    [SerializeField] private float _maxVerticalAngle = 80f;

    [Header("Zoom")]
    [SerializeField] private float _zoomSpeed = 0.5f;
    [SerializeField] private float _minDistance = 0.5f;
    [SerializeField] private float _maxDistance = 10f;

    private float _currentDistance = 5f;
    private Vector2 _rotation = Vector2.zero;

    private void Start()
    {
        _rotation = new Vector2(20f, 0f); // Стартовый угол (немного сверху)
        _currentDistance = (_minDistance + _maxDistance) * 0.5f; // Среднее значение
        UpdateCameraPosition();
    }

    private void Update()
    {
        if (_target == null) return;

        HandleTouchInput();
        UpdateCameraPosition();
    }

    private void HandleTouchInput()
    {
        // Вращение (1 палец)
        if (Input.touchCount == 1 && !IsPointerOverUI())
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;
                _rotation.y += delta.x * _rotationSpeed * 0.01f;
                _rotation.x -= delta.y * _rotationSpeed * 0.01f;
                _rotation.x = Mathf.Clamp(_rotation.x, _minVerticalAngle, _maxVerticalAngle);
            }
        }

        // Зум (2 пальца)
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
            {
                float prevDistance = Vector2.Distance(
                    touch1.position - touch1.deltaPosition,
                    touch2.position - touch2.deltaPosition);
                float currentDistance = Vector2.Distance(touch1.position, touch2.position);

                _currentDistance -= (currentDistance - prevDistance) * _zoomSpeed * 0.01f;
                _currentDistance = Mathf.Clamp(_currentDistance, _minDistance, _maxDistance);
            }
        }
    }

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(_rotation.x, _rotation.y, 0);
        Vector3 position = _target.position - rotation * Vector3.forward * _currentDistance;

        transform.rotation = rotation;
        transform.position = position;
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;
        return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
    }

    /*public void SetTarget(Transform target)
    {
        _target = target;
        _currentDistance = (_minDistance + _maxDistance) * 0.5f;
        _rotation = new Vector2(20f, 0f);
        UpdateCameraPosition();
    }*/
}