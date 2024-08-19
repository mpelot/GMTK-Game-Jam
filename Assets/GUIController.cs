using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    [SerializeField] private Image selectedName;
    [SerializeField] private Text description;
    [SerializeField] private Text percentage;
    [SerializeField] private GameObject border;

    [SerializeField] private Sprite planetName;
    [SerializeField] private Color planetColor;
    [SerializeField] private Sprite tetradonName;
    [SerializeField] private Color tetradonColor;
    [SerializeField] private Sprite sunName;
    [SerializeField] private Color sunColor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void changeSelected(Selectable selectedObject) {
        if (selectedObject == null) {
            selectedName.enabled = false;
            description.enabled = false;
            percentage.enabled = false;
            description.text = "";
            percentage.text = "";
            border.SetActive(false);
        } else if (selectedObject.gameObj.tag.Equals("Planet")) {
            selectedName.sprite = planetName;
            selectedName.enabled = true;
            description.enabled = true;
            percentage.enabled = true;
            description.text = "CAN REDIRECT INCOMING ASTEROIDS";
            percentage.text = "";
            ChangeBorderColor(planetColor);
        } else if (selectedObject.gameObj.tag.Equals("Harvester")) {
            selectedName.sprite = tetradonName;
            selectedName.enabled = true;
            description.enabled = true;
            percentage.enabled = true;
            description.text = "CAN HARVEST ARDIUM";
            percentage.text = "";
            ChangeBorderColor(tetradonColor);
        } else if (selectedObject.gameObj.tag.Equals("Core")) {
            selectedName.sprite = sunName;
            selectedName.enabled = true;
            description.enabled = true;
            percentage.enabled = true;
            description.text = "PROTECT AT ALL COSTS";
            percentage.text = "";
            ChangeBorderColor(sunColor);
        }
    }

    private void ChangeBorderColor(Color c) {
        border.SetActive(true);
        Image[] imgs = border.GetComponentsInChildren<Image>();

        foreach (Image img in imgs) {
            img.color = c;
        }
    }

}
