using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour, Selectable
{
    private bool selected;
    private GameMangager gm;
    private Vector3 startingScale;
    public int growthFromPlanet;
    public float growthRate;
    public int _growthLevel = 0;
    public int growthLevel
    {
        get
        {
            return _growthLevel;
        }
        set
        {
            if (value < 0)
            {
                value = 0;
            }
            _growthLevel = value;
            transform.localScale = new Vector3(startingScale.x + (_growthLevel * growthRate), startingScale.y + (_growthLevel * growthRate), 0f);
        }
    }
    void Start() {
        gm = FindAnyObjectByType<GameMangager>();
        startingScale = transform.localScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            Destroy(collision.gameObject);
            growthLevel++;
        }
        else if (collision.gameObject.CompareTag("Planet") || collision.gameObject.CompareTag("Harvester"))
        {
            Destroy(collision.gameObject);
            growthLevel += growthFromPlanet;
        }
        else if (collision.gameObject.CompareTag("ShrinkRock"))
        {
            Destroy(collision.gameObject);
            growthLevel--;
        }
    }

    private void OnMouseDown() {
        if (!selected)
            gm.selectedObject = this;
    }

    public void Select() {
        selected = true;
    }

    public void Deselect() {
        selected = false;
    }
}
