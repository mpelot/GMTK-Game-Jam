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
    private Movable movable;
    private Animator animator;

    private Vector3 startingScale;
    public float unstableGrowthThreshold;
    public float slowdownPerGrowthLevel;
    public float scaleRate;
    public float _growthLevel = 0;
    private bool mouseOver = false;
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
            movable.dragSpeedMultiplier = 1f - (_growthLevel * slowdownPerGrowthLevel);
            if (_growthLevel >= unstableGrowthThreshold)
            {
                movable.isBeingPulledToCore = true;
            }
            else
            {
                movable.isBeingPulledToCore = false;
            }
        }
    }

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        gm = FindAnyObjectByType<GameMangager>();
        movable = GetComponent<Movable>();
        animator = GetComponent<Animator>();
        startingScale = transform.localScale;
    }

    private void Update() {
        if (selected && Input.GetMouseButtonDown(0) && !mouseOver) {
            gm.selectedObject = null;
            Deselect();
        }
    }

    private void OnMouseEnter() {
        mouseOver = true;
    }

    private void OnMouseExit() {
        mouseOver = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            Asteroid asteroid = collision.GetComponent<Asteroid>();
            Destroy(collision.gameObject);
            growthLevel += asteroid.growthLevel;
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
        /*else
            gm.selectedObject = null;*/
    }

    public void Select() {
        selected = true;
        movable.selected = true;
        ring.SetActive(true);
        animator.SetBool("Selected", true);
    }

    public void Deselect() {
        selected = false;
        movable.selected = false;
        ring.SetActive(false);
        animator.SetBool("Selected", false);
    }

    private void OnDestroy()
    {
        if (gm.selectedObject == (Selectable) this)
        {
            gm.selectedObject = null;
            Deselect();
        }
    }
}
