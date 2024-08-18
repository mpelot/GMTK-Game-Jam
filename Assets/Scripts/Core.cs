using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : MonoBehaviour, Selectable
{
    private bool selected;
    private GameMangager gm;
    private Vector3 startingScale;
    public float growthFromPlanet;
    public float scaleRate;

    // Every x seconds, a growth event occurs and the growth level increases
    public float secondsBetweenGrowthEvent;
    public float growthPerGrowthEvent;
    public float growthEventDuration;
    private bool isGrowthEventOccuring;
    private float growthEventTimer;
    
    public float _growthLevel = 0;
    public float growthLevel
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
            transform.localScale = new Vector3(startingScale.x + (_growthLevel * scaleRate), startingScale.y + (_growthLevel * scaleRate), 0f);
        }
    }
    void Start() {
        gm = FindAnyObjectByType<GameMangager>();
        startingScale = transform.localScale;
        growthEventTimer = secondsBetweenGrowthEvent;
        isGrowthEventOccuring = false;
    }

    void Update()
    {
        growthEventTimer -= Time.deltaTime;
        if (!isGrowthEventOccuring)
        {
            if (growthEventTimer <= 0)
            {
                isGrowthEventOccuring = true;
                growthEventTimer = growthEventDuration;
            }
        }
        else
        {
            growthLevel += growthPerGrowthEvent / growthEventDuration * Time.deltaTime;
            if (growthEventTimer <= 0)
            {
                isGrowthEventOccuring = false;
                growthEventTimer = secondsBetweenGrowthEvent;
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            Asteroid asteroid = collision.GetComponent<Asteroid>();
            Destroy(collision.gameObject);
            growthLevel += asteroid.growthLevel;
        }
        else if (collision.gameObject.CompareTag("Planet") || collision.gameObject.CompareTag("Harvester"))
        {
            Destroy(collision.gameObject);
            growthLevel += growthFromPlanet;
        }
        else if (collision.gameObject.CompareTag("ShrinkRock"))
        {
            ShrinkRock shrinkRock = collision.GetComponent<ShrinkRock>();
            Destroy(collision.gameObject);
            growthLevel -= shrinkRock.shinkAmount;
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
