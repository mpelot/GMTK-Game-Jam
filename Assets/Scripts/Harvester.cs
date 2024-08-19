using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvester : MonoBehaviour, Selectable
{
    private bool selected;
    private bool isAiming;
    private GameMangager gm;
    public TrajectoryLine trajectoryLine;
    public float minimumShotVelocity;
    public float maximumShotVelocity;
    public float shotForce;
    public float shotCooldown;
    private bool isFiringPathSet;
    public GameObject shrinkRockPrefab;
    private Movable movable;
    public float splitThreshold;
    private List<Collider2D> ignoredColliders;
    private SpriteRenderer spriteRenderer;
    private Vector3 startingScale;
    private Color32 startingColor;
    private bool mouseOver = false;
    public float unstableGrowthLevel;
    public float slowdownPerGrowthLevel;
    public float scaleRate;
    public float _growthLevel = 0;
    private int _percentage = 0;
    private Animator animator;
    private float mouseDownTimer = 0f;
    private float ignoreDeselectTimer = 0f;
    private CircleCollider2D circleCollider;
    public Arrow arrow;
    public float growthLevel
    {
        get
        {
            return _growthLevel;
        }
        set
        {
            bool spawnHarvester = false;
            if (value < 0)
            {
                value = 0;
                Destroy(gameObject);
            }
            else if (value >= splitThreshold)
            {
                value = 0;
                spawnHarvester = true;
            }
            _growthLevel = value;
            transform.localScale = new Vector3(startingScale.x + (_growthLevel * scaleRate), startingScale.y + (_growthLevel * scaleRate), 0f);
            spriteRenderer.color = new Color32(startingColor.r, (byte)(startingColor.g + (_growthLevel * 10)), startingColor.b, startingColor.a);
            movable.dragSpeedMultiplier = 1f - (_growthLevel * slowdownPerGrowthLevel);

            if (_growthLevel >= unstableGrowthLevel)
            {
                movable.isBeingPulledToCore = true;
            }
            else
            {
                movable.isBeingPulledToCore = false;
            }
            if (spawnHarvester)
            {
                GameObject spawnedHarvester = Instantiate(gameObject, transform.position, Quaternion.identity);
                spawnedHarvester.GetComponent<Rigidbody2D>().velocity = new Vector2(0.1f, 0.1f);
            }
            _percentage = (int)(growthLevel / unstableGrowthLevel * 100);
            if (selected)
                gm.updateUI(this);
        }
    }

    public GameObject gameObj {
        get {
            return gameObject;
        }
    }

    public int percentage {
        get {
            return _percentage;
        }
    }

    void Awake()
    {
        gm = FindAnyObjectByType<GameMangager>();
        trajectoryLine.Hide();
        ignoredColliders = new List<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startingScale = transform.localScale;
        startingColor = spriteRenderer.color;
        movable = GetComponent<Movable>();
        animator = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
        arrow.gameObject.SetActive(false);
    }

    private void OnMouseEnter() {
        mouseOver = true;
    }

    private void OnMouseExit() {
        mouseOver = false;
    }

    void Update()
    {
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


        if (Input.GetMouseButtonDown(1))
        {
            if (selected)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (circleCollider.OverlapPoint(mousePos))
                {
                    StartAiming();
                }
                else
                {
                    StopAiming();
                }
            }
        }
        if (isAiming)
        {
            isFiringPathSet = false;
            Vector2 shotVelocity = (transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)) * shotForce;
            if (shotVelocity.magnitude < minimumShotVelocity)
            {
                ClearFiringPath();
                arrow.gameObject.SetActive(false);
            }
            else
            {
                if (shotVelocity.magnitude > maximumShotVelocity)
                {
                    shotVelocity = shotVelocity.normalized * maximumShotVelocity;
                }
                trajectoryLine.Show();
                trajectoryLine.startingVelocity = shotVelocity;

                arrow.gameObject.SetActive(true);
                arrow.SetSize(shotVelocity.magnitude / 4.25f);
                arrow.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(shotVelocity.y, shotVelocity.x) * Mathf.Rad2Deg);

                float shotVelcoityScale = (shotVelocity.magnitude - minimumShotVelocity) / (maximumShotVelocity - minimumShotVelocity);
                trajectoryLine.lineRenderer.textureScale = new Vector2((-5f * shotVelcoityScale) + 8, 1f);

                if (Input.GetMouseButtonUp(1))
                {
                    SetFiringPath(shotVelocity);
                }
                else if (!selected)
                {
                    StopAiming();
                }
            }
        }
    }

    void SetFiringPath(Vector2 shotVelocity)
    {
        isFiringPathSet = true;
        StopAiming();
        StartCoroutine(FireLoop(shotVelocity));
    }

    public IEnumerator FireLoop(Vector2 shotVelocity)
    {
        while (growthLevel > 0 && isFiringPathSet)
        {
            GameObject spawnedShrinkRock = Instantiate(shrinkRockPrefab, transform.position, Quaternion.identity);
            spawnedShrinkRock.GetComponent<Rigidbody2D>().velocity = shotVelocity;
            ignoredColliders.Add(spawnedShrinkRock.GetComponent<Collider2D>());
            growthLevel--;
            yield return new WaitForSeconds(shotCooldown);
        }
        ClearFiringPath();
    }

    void StartAiming()
    {
        isAiming = true;
    }

    void StopAiming()
    {
        isAiming = false;
        if (!isFiringPathSet)
        {
            trajectoryLine.Hide();
        }
        arrow.gameObject.SetActive(false);
    }

    public void ClearFiringPath()
    {
        isFiringPathSet = false;

        trajectoryLine.Hide();
        if (Input.GetMouseButtonUp(1))
        {
            StopAiming();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ignoredColliders.Contains(collision))
        {
            ignoredColliders.Remove(collision);
            return;
        }
        if (collision.gameObject.CompareTag("Asteroid"))
        {
            Asteroid asteroid = collision.GetComponent<Asteroid>();
            Destroy(collision.gameObject);
            growthLevel += asteroid.growthLevel;
        }
        if (collision.gameObject.CompareTag("ShrinkRock"))
        {
            ShrinkRock shrinkRock = collision.GetComponent<ShrinkRock>();
            Destroy(collision.gameObject);
            growthLevel -= shrinkRock.shinkAmount;
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
        animator.SetBool("Selected", true);
    }

    public void Deselect() {
        selected = false;
        animator.SetBool("Selected", false);
    }
}
