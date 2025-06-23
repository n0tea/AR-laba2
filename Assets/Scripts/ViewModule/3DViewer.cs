//using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using static AppController;

public class ThreeDViewer : MonoBehaviour
{
    [SerializeField] private Transform objectContainer;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float zoomSpeed = 0.01f;

    private Vector2 lastTouchPosition;
    private bool isRotating = false;
    private Camera mainCamera;
    private GameObject currentObject;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        {
            AppController.Instance.OnObjectChanged += HandleObjectChanged;
            AppController.Instance.OnViewModeChanged += HandleViewModeChanged;
        }
    }

    private void OnDisable()
    {
        AppController.Instance.OnObjectChanged -= HandleObjectChanged;
        AppController.Instance.OnViewModeChanged -= HandleViewModeChanged;
    }
    private void HandleObjectChanged(GameObject prefab)
    {
        if (AppController.Instance.CurrentViewMode != ViewMode.Mode3D) return;

        UpdateObject(prefab);
    }

    private void HandleViewModeChanged(ViewMode mode)
    {
        if (mode == ViewMode.Mode3D)
        {
            // При переключении в 3D режим обновляем объект
            UpdateObject(AppController.Instance.SelectedPrefab);
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
}