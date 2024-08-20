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
    [SerializeField] private Text year;

    [SerializeField] private Sprite planetName;
    [SerializeField] private Color planetColor;
    [SerializeField] private Sprite tetradonName;
    [SerializeField] private Color tetradonColor;
    [SerializeField] private Sprite sunName;
    [SerializeField] private Color sunColor;

    private int yearNum = 0;

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
        } else {
            selectedName.enabled = true;
            description.enabled = true;
            percentage.enabled = true;
            percentage.text = selectedObject.percentage + "%";
            if (selectedObject.gameObj.tag.Equals("Planet")) {
                selectedName.sprite = planetName;
                description.text = "CAN REDIRECT INCOMING ASTEROIDS";
                ChangeBorderColor(planetColor);
            } else if (selectedObject.gameObj.tag.Equals("Harvester")) {
                selectedName.sprite = tetradonName;
                description.text = "CAN CONVERT ASTEROIDS INTO ARDIUM";
                ChangeBorderColor(tetradonColor);
            } else if (selectedObject.gameObj.tag.Equals("Core")) {
                selectedName.sprite = sunName;
                description.text = "PROTECT AT ALL COSTS";
                ChangeBorderColor(sunColor);
            }
        }
    }

    private void ChangeBorderColor(Color c) {
        border.SetActive(true);
        Image[] imgs = border.GetComponentsInChildren<Image>();

        foreach (Image img in imgs) {
            img.color = c;
        }
    }

    public void IncrementYear() {
        yearNum++;
        year.text = "Year " + yearNum;
    }
}
