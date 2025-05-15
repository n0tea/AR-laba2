/*using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ObjectSelector : MonoBehaviour
{
    public static GameObject SelectedPrefab { get; private set; }

    [SerializeField] private Button cubeButton;
    [SerializeField] private Button cylinderButton;

    public GameObject cubePrefab;
    public GameObject cylinderPrefab;

    public static GameObject currentObject { get; private set; }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SelectedPrefab = cubePrefab;

        cubeButton.onClick.AddListener(() => SelectFigure(cubePrefab));
        cylinderButton.onClick.AddListener(() => SelectFigure(cylinderPrefab));

        UpdateSelectedObject();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "ARModule")
        {
            if (currentObject != null)
            {
                currentObject.SetActive(false);
            }
        }
        else if (scene.name == "3DModule")
        {
            UpdateSelectedObject();
        }
    }

    public void SelectFigure(GameObject prefab)
    {
        SelectedPrefab = prefab;
        Debug.Log($"{SelectedPrefab.name} Selected.");
        UpdateSelectedObject();
    }

    public void UpdateSelectedObject()
    {
        if (SceneManager.GetActiveScene().name == "3DModule")
        {
            if (currentObject != null)
            {
                Destroy(currentObject);
            }

            if (SelectedPrefab != null)
            {
                currentObject = Instantiate(SelectedPrefab, transform.position, transform.rotation);
                currentObject.transform.parent = transform;
            }
        }
    }
}*/

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class ObjectSelector : MonoBehaviour
{
    public static GameObject SelectedPrefab { get; private set; }

    [SerializeField] private Transform buttonContainer; // Контейнер для кнопок
    [SerializeField] private GameObject buttonPrefab;  // Префаб кнопки

    [System.Serializable]
    public struct FigureData
    {
        public string name;       // Название фигуры (для отладки)
        public GameObject prefab; // Префаб фигуры
        public Sprite icon;       // Иконка для кнопки
    }

    [SerializeField] private List<FigureData> figures; // Список фигур

    public static GameObject currentObject { get; private set; }

    [SerializeField] private Color selectedColor = Color.green; // Цвет для выбранной кнопки
    [SerializeField] private Color normalColor = Color.white;   // Цвет для невыбранной кнопки

    private Dictionary<Button, GameObject> buttonToPrefabMap = new Dictionary<Button, GameObject>();
    private Button selectedButton; // Текущая выбранная кнопка

    private void Start()
    {
        //SceneManager.sceneLoaded += OnSceneLoaded;
        CreateButtons(); // Создаем кнопки для всех фигур
        SelectFigure(figures[0].prefab); // Выбираем первую фигуру по умолчанию
    }

    /*private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }*/

    /*private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "ARModule")
        {
            if (currentObject != null)
            {
                currentObject.SetActive(false);
            }
        }
        else if (scene.name == "3DModule")
        {
            UpdateSelectedObject();
        }
    }*/

    private void CreateButtons()
    {
        foreach (var figure in figures)
        {
            // Создаем кнопку из префаба
            GameObject buttonGO = Instantiate(buttonPrefab, buttonContainer);
            Button button = buttonGO.GetComponent<Button>();
            Image buttonImage = buttonGO.GetComponent<Image>();

            // Устанавливаем иконку на кнопку
            if (buttonImage != null && figure.icon != null)
            {
                buttonImage.sprite = figure.icon;
            }

            // Подписываемся на событие клика
            button.onClick.AddListener(() => SelectFigure(figure.prefab));

            // Сохраняем связь между кнопкой и префабом
            buttonToPrefabMap[button] = figure.prefab;
        }
    }

    public void SelectFigure(GameObject prefab)
    {
        SelectedPrefab = prefab;
        Debug.Log($"{SelectedPrefab.name} Selected.");
        UpdateSelectedObject();
        UpdateButtonColors();
    }

    public void UpdateSelectedObject()
    {
        if (SceneManager.GetActiveScene().name == "3DModule")
        {
            if (currentObject != null)
            {
                Destroy(currentObject);
            }

            if (SelectedPrefab != null)
            {
                currentObject = Instantiate(SelectedPrefab, transform.position, transform.rotation);
                currentObject.transform.parent = transform;
                if (AppState.Instance.IsARModuleLoaded)
                {
                    currentObject.SetActive(false); 
                }
                else
                {
                    currentObject.SetActive(true);
                }

            }
        }
        else
        {
            if (currentObject != null)
            {
                Destroy(currentObject);
            }
        }
    }

    private void UpdateButtonColors()
    {
        // Сбрасываем цвет всех кнопок
        foreach (var button in buttonToPrefabMap.Keys)
        {
            button.image.color = normalColor;
        }

        // Находим кнопку, соответствующую выбранному префабу, и выделяем ее
        foreach (var pair in buttonToPrefabMap)
        {
            if (pair.Value == SelectedPrefab)
            {
                pair.Key.image.color = selectedColor;
                selectedButton = pair.Key;
                break;
            }
        }
    }
}