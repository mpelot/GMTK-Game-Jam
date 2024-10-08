using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Planet : MonoBehaviour, Selectable
{
    public GameObject ring;
    public GameObject warningSymbol;
    private Rigidbody2D rb;
    private bool selected = false;
    private GameMangager gm;
    private Movable movable;
    private Animator animator;

    public GameObject gameObj {
        get {
            return gameObject;
        }
    }

    private Vector3 startingScale;
    public float unstableGrowthThreshold;
    public float slowdownPerGrowthLevel;
    public float scaleRate;
    public float _growthLevel = 0;
    public int _percentage = 0;
    private bool mouseOver = false;
    private float mouseDownTimer = 0f;
    private float ignoreDeselectTimer = 0f;
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
                warningSymbol.SetActive(true);
            }
            else
            {
                movable.isBeingPulledToCore = false;
                warningSymbol.SetActive(false);
            }
            _percentage = (int)(growthLevel / unstableGrowthThreshold * 100);
            if (selected)
                gm.updateUI(this);
        }
    }

    public int percentage {
        get {
            return _percentage;
        }
    }

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        gm = FindAnyObjectByType<GameMangager>();
        movable = GetComponent<Movable>();
        animator = GetComponent<Animator>();
        startingScale = transform.localScale;
        warningSymbol.SetActive(false);
    }

    private void Update() {
        mouseDownTimer += Time.deltaTime;
        ignoreDeselectTimer += Time.deltaTime;
        if (Input.GetMouseButtonUp(0))
        {
            if (mouseDownTimer < 0.2f && ignoreDeselectTimer > 0.3f)
            {
                if (selected)
                {
                    if (gm.selectedObject.Equals(this))
                        gm.selectedObject = null;
                    Deselect();
                }
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            mouseDownTimer = 0f;
            if (selected && !mouseOver)
            {
                if (gm.selectedObject.Equals(this))
                    gm.selectedObject = null;
                Deselect();
            }
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
        else if (collision.gameObject.CompareTag("Ardium"))
        {
            Ardium ardium = collision.GetComponent<Ardium>();
            Destroy(collision.gameObject);
            growthLevel -= ardium.shinkAmount;
        }
    }

    private void OnMouseDown() {
        if (!selected)
        {
            gm.selectedObject = this;
            ignoreDeselectTimer = 0.0f;
        }
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
