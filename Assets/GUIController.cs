using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    [SerializeField] private Image selectedName;
    [SerializeField] private TMPro.TextMeshProUGUI description;
    [SerializeField] private TMPro.TextMeshProUGUI percentage;

    [SerializeField] private Sprite planetName;
    [SerializeField] private Sprite tetradonName;
    [SerializeField] private Sprite sunName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void changeSelected(Selectable selectedObject) {
        if (selectedObject == null) {
            selectedName.enabled = false;
            description.text = "";
            percentage.text = "";
        } else if (selectedObject.gameObj.tag.Equals("Planet")) {
            selectedName.sprite = planetName;
            selectedName.enabled = true;
            description.text = "";
            percentage.text = "";
        } else if (selectedObject.gameObj.tag.Equals("Harvester")) {
            selectedName.sprite = tetradonName;
            selectedName.enabled = true;
            description.text = "";
            percentage.text = "";
        } else if (selectedObject.gameObj.tag.Equals("Core")) {
            selectedName.sprite = sunName;
            selectedName.enabled = true;
            description.text = "";
            percentage.text = "";
        }
    }
}
