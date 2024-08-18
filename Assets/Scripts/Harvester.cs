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
    public float unstableGrowthLevel;
    public float slowdownPerGrowthLevel;
    public float growthRate;
    private float _growthLevel = 0;
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
            transform.localScale = new Vector3(startingScale.x + (_growthLevel * growthRate), startingScale.y + (_growthLevel * growthRate), 0f);
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
        }
    }

    void Start()
    {
        gm = FindAnyObjectByType<GameMangager>();
        trajectoryLine.Hide();
        ignoredColliders = new List<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startingScale = transform.localScale;
        startingColor = spriteRenderer.color;
        movable = GetComponent<Movable>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (selected)
            {
                isAiming = true;
            }
        }
        if (isAiming)
        {
            isFiringPathSet = false;
            Vector2 shotVelocity = (transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)) * shotForce;
            if (shotVelocity.magnitude < minimumShotVelocity)
            {
                trajectoryLine.Hide();
                if (Input.GetMouseButtonUp(1))
                {
                    StopAiming();
                }
            }
            else
            {
                if (shotVelocity.magnitude > maximumShotVelocity)
                {
                    shotVelocity = shotVelocity.normalized * maximumShotVelocity;
                }
                trajectoryLine.Show();
                trajectoryLine.startingVelocity = shotVelocity;

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
        while (isFiringPathSet)
        {
            if (growthLevel > 0)
            {
                GameObject spawnedShrinkRock = Instantiate(shrinkRockPrefab, transform.position, Quaternion.identity);
                spawnedShrinkRock.GetComponent<Rigidbody2D>().velocity = shotVelocity;
                ignoredColliders.Add(spawnedShrinkRock.GetComponent<Collider2D>());
                growthLevel--;
            }
            yield return new WaitForSeconds(shotCooldown);
        }
    }

    void StopAiming()
    {
        isAiming = false;
        if (!isFiringPathSet)
        {
            trajectoryLine.Hide();
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
            Destroy(collision.gameObject);
            growthLevel++;
        }
        if (collision.gameObject.CompareTag("ShrinkRock"))
        {
            Destroy(collision.gameObject);
            growthLevel--;
        }
    }

    private void OnMouseDown() {
        if (selected)
            Deselect();
        else
            gm.selectedObject = this;
    }

    public void Select() {
        selected = true;
    }

    public void Deselect() {
        selected = false;
    }
}
