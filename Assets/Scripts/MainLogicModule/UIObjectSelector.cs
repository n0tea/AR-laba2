using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIObjectSelector : MonoBehaviour
{
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color normalColor = Color.white;

    [System.Serializable]
    public struct FigureData
    {
        public string name;
        public GameObject prefab;
        public Sprite icon;
    }

    [SerializeField] private List<FigureData> figures;

    private Dictionary<Button, GameObject> buttonToPrefabMap = new Dictionary<Button, GameObject>();
    private Button selectedButton;

    private void Start()
    {
        CreateButtons();
        //SelectFigure(figures[0].prefab);
    }

    private void CreateButtons()
    {
        foreach (var figure in figures)
        {
            GameObject buttonGO = Instantiate(buttonPrefab, buttonContainer);
            Button button = buttonGO.GetComponent<Button>();
            Image buttonImage = buttonGO.GetComponent<Image>();

            if (buttonImage != null && figure.icon != null)
            {
                buttonImage.sprite = figure.icon;
            }

            button.onClick.AddListener(() => SelectFigure(figure.prefab));
            buttonToPrefabMap[button] = figure.prefab;
        }
    }

    private void SelectFigure(GameObject prefab)
    {
        AppController.Instance.SetSelectedObject(prefab);
        UpdateButtonColors(prefab);
    }

    private void UpdateButtonColors(GameObject selectedPrefab)
    {
        foreach (var button in buttonToPrefabMap.Keys)
        {
            button.image.color = normalColor;
        }

        foreach (var pair in buttonToPrefabMap)
        {
            if (pair.Value == selectedPrefab)
            {
                pair.Key.image.color = selectedColor;
                selectedButton = pair.Key;
                break;
            }
        }
    }
}