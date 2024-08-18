using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Planet : MonoBehaviour, Selectable
{
    public GameObject ring;
    private Rigidbody2D rb;
    private bool selected = false;
    private GameMangager gm;
    private Movable moveable;

    private Vector3 startingScale;
    public int unstableGrowthThreshold;
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
            if (_growthLevel >= unstableGrowthThreshold)
            {
                moveable.isBeingPulledToCore = true;
            }
            else
            {
                moveable.isBeingPulledToCore = false;
            }
        }
    }

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        gm = FindAnyObjectByType<GameMangager>();
        moveable = GetComponent<Movable>();
        startingScale = transform.localScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            Destroy(collision.gameObject);
            growthLevel++;
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
        moveable.selected = true;
        ring.SetActive(true);
    }

    public void Deselect() {
        selected = false;
        moveable.selected = false;
        ring.SetActive(false);
    }

    private void OnDestroy()
    {
        if (gm.selectedObject == this)
        {
            gm.selectedObject = null;
            Deselect();
        }
    }
}
