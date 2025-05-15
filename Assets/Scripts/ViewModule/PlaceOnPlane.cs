using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARSubsystems;
using static ChangesController;

namespace UnityEngine.XR.ARFoundation.Samples
{
    /// <summary>
    /// Listens for touch events and performs an AR raycast from the screen touch point.
    /// AR raycasts will only hit detected trackables like feature points and planes.
    ///
    /// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
    /// and moved to the hit position.
    /// </summary>
    [RequireComponent(typeof(ARRaycastManager))]
    public class PlaceOnPlane : PressInputBase
    {
        
        //private ARObjectData arObjectData; // ������ �� ScriptableObject �������� ����� ����������
        //[SerializeField]
        //[Tooltip("Instantiates this prefab on a plane at the touch location.")]
        //GameObject m_PlacedPrefab;

        /// <summary>
        /// The prefab to instantiate on touch.
        /// </summary>
        /*public GameObject placedPrefab
        {
            get { return m_PlacedPrefab; }
            set { m_PlacedPrefab = value; }
        }*/

        /// <summary>
        /// The object instantiated as a result of a successful raycast intersection with a plane.
        /// </summary>
        private GameObject spawnedObject; //{ get; private set; }
        //private GameObject currentPrefab; // ������� ��������� ������ �� �����, ������ ��� � ObjectSelector

        bool m_Pressed;

        protected override void Awake()
        {

            base.Awake();
            m_RaycastManager = GetComponent<ARRaycastManager>();
            //currentPrefab = ObjectSelector.SelectedPrefab;
            //placedPrefab = currentPrefab;

            //Debug.Log($"artn AR {ObjectSelector.SelectedPrefab.name} Selected.");
        }

        private void OnEnable()
        {
            ChangesController.Instance.OnObjectChanged += HandleObjectChanged;
        }

        private void HandleObjectChanged(GameObject prefab)
        {
            // ���������� ���������� ������, ���� �� ����
            if (spawnedObject != null)
            {
                Destroy(spawnedObject);
            }

            // ������� ����� ������, �� ���� �� ���������� � �� ���������
            if (prefab != null)
            {
                spawnedObject = Instantiate(prefab);
                spawnedObject.SetActive(false); // ������ ��������� �� ����������
            }
        }
        void Update()
        {
            /*if (EventSystem.current.IsPointerOverGameObject())
            {
                return; // ���� ��, ���������� �������������� �� ������, ������� ������ UI
            }*/
            Debug.Log(m_Pressed);
            if (Pointer.current == null || m_Pressed == false)
                return;

            var touchPosition = Pointer.current.position.ReadValue();

            if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                // Raycast hits are sorted by distance, so the first one
                // will be the closest hit.
                var hitPose = s_Hits[0].pose;

                if (spawnedObject != null)
                {
                    //spawnedObject = Instantiate(currentPrefab/*ObjectSelector.SelectedPrefab*/, hitPose.position, hitPose.rotation);
                    //spawnedObject = ObjectSelector.currentObject;
                    //spawnedObject.SetActive(true);
                    //spawnedObject.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation); 
                    // SetLocalPositionAndRotation - �����
                    if (!spawnedObject.activeSelf)
                    {
                        spawnedObject.SetActive(true); // ���������� ��� ������ ����������
                    }

                    spawnedObject.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
                }
                /*else
                {

                    spawnedObject.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
                }*/
            }
        }

        protected override void OnPress(Vector3 position) => m_Pressed = true;

        protected override void OnPressCancel() => m_Pressed = false;

        static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

        ARRaycastManager m_RaycastManager;
    }
}

