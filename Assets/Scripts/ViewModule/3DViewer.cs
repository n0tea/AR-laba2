//using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using static ChangesController;

public class ThreeDViewer : MonoBehaviour
{
    [SerializeField] private Transform objectContainer;
    [SerializeField] private float rotationSpeed = 10f;

    private GameObject currentObject;
    private bool isRotating = false;
    private Vector2 lastMousePosition;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        {
            ChangesController.Instance.OnObjectChanged += HandleObjectChanged;
            ChangesController.Instance.OnViewModeChanged += HandleViewModeChanged;
        }
    }
    

    private void OnDisable()
    {
        ChangesController.Instance.OnObjectChanged -= HandleObjectChanged;
        ChangesController.Instance.OnViewModeChanged -= HandleViewModeChanged;
    }
    private void HandleObjectChanged(GameObject prefab)
    {
        if (ChangesController.Instance.CurrentViewMode != ViewMode.Mode3D) return;

        UpdateObject(prefab);
    }

    private void HandleViewModeChanged(ViewMode mode)
    {
        if (mode == ViewMode.Mode3D)
        {
            // При переключении в 3D режим обновляем объект
            UpdateObject(ChangesController.Instance.SelectedPrefab);
        }
        else
        {
            // При переключении в AR режим скрываем 3D объект
            if (currentObject != null)
            {
                currentObject.SetActive(false);
            }
        }
    }

    public void UpdateObject(GameObject prefab)
    {
        if (currentObject != null)
        {
            Destroy(currentObject);
        }

        if (prefab != null)
        {
            currentObject = Instantiate(prefab, objectContainer.position, Quaternion.identity, objectContainer);

        }
    }

    private void Update()
    {
        //HandleRotationInput();
        //RotateObject();
    }

    private void HandleRotationInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isRotating = true;
            lastMousePosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isRotating = false;
        }
    }

    private void RotateObject()
    {
        if (isRotating && currentObject != null)
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastMousePosition;
            currentObject.transform.Rotate(Vector3.up, -delta.x * rotationSpeed * Time.deltaTime, Space.World);
            currentObject.transform.Rotate(Vector3.right, delta.y * rotationSpeed * Time.deltaTime, Space.World);
            lastMousePosition = Input.mousePosition;
        }
    }
}