using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetradon : MonoBehaviour, Selectable
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
    public GameObject ardiumPrefab;
    public GameObject warningSymbol;
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
    private float doubleClickTimer = 0f;
    private CircleCollider2D circleCollider;
    public Arrow arrow;
    private Rigidbody2D rb;
    public GameObject chargeIndicator;
    public bool isInvincible = false;
    public bool allowSplitting = true;
    public AudioSource audioSource;
    [SerializeField] Tetradon tetradon;

    private float consumeTimer = 0;
    private int consumeStreak = 0;
    
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
                if (isInvincible)
                {
                    value = 0;
                }
                else
                {
                    value = 0;
                    Destroy(gameObject);
                }
            }
            _growthLevel = value;
            transform.localScale = new Vector3(startingScale.x + (_growthLevel * scaleRate), startingScale.y + (_growthLevel * scaleRate), 0f);
            spriteRenderer.color = new Color32(startingColor.r, (byte)(startingColor.g + (_growthLevel * 10)), startingColor.b, startingColor.a);
            movable.dragSpeedMultiplier = 1f - (_growthLevel * slowdownPerGrowthLevel);

            if (_growthLevel >= unstableGrowthLevel)
            {
                movable.isBeingPulledToCore = true;
                warningSymbol.SetActive(true);
            }
            else
            {
                movable.isBeingPulledToCore = false;
                warningSymbol.SetActive(false);

                float chargeIndicatorScale = growthLevel / splitThreshold;
                chargeIndicatorScale = Mathf.Clamp(chargeIndicatorScale, 0, 0.9f);
                chargeIndicator.transform.localScale = new Vector3(chargeIndicatorScale, chargeIndicatorScale, 1);
            }
            _percentage = (int)(growthLevel / splitThreshold * 100);
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
        warningSymbol.SetActive(false);
        rb = GetComponent<Rigidbody2D>();
        chargeIndicator.transform.localScale = new Vector3(0, 0, 1);
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
        doubleClickTimer += Time.deltaTime;
        consumeTimer += Time.deltaTime;

        if (growthLevel >= splitThreshold)
        {
            float size = (Mathf.PingPong(Time.time, 0.5f) / 2.0f) + 0.8f;
            chargeIndicator.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f) * size;
        }

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
            if (mouseOver)
            {
                if (doubleClickTimer < 0.5f)
                {
                    if (growthLevel >= splitThreshold)
                    {
                        SplitTetradon();
                    }
                }
                doubleClickTimer = 0f;
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

    void SplitTetradon()
    {
        if (!allowSplitting)
            return;
        growthLevel = (growthLevel - splitThreshold) / 2;
        Tetradon spawnedTetradon = Instantiate(tetradon, transform.position, Quaternion.identity);
        spawnedTetradon.GetComponent<Rigidbody2D>().velocity = new Vector2(1, 0);
        spawnedTetradon.growthLevel = growthLevel;
        spawnedTetradon.Deselect();

        if (gm.selectedObject.Equals(this))
        {
            gm.selectedObject = null;
            Deselect();
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
            GameObject spawnedArdium = Instantiate(ardiumPrefab, transform.position, Quaternion.identity);
            spawnedArdium.GetComponent<Rigidbody2D>().velocity = shotVelocity;
            ignoredColliders.Add(spawnedArdium.GetComponent<Collider2D>());
            if (growthLevel < 1.0)
            {
                growthLevel = 0;
            }
            else
            {
                growthLevel -= 1.0f;
            }
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
            if (asteroid.growthLevel <= 5.0f)
            {
                Destroy(collision.gameObject);
                growthLevel += asteroid.growthLevel;
                PlayConsumedSound();
            }
            else
            {
                asteroid.growthLevel -= 5.0f;
                Destroy(this.gameObject);
            }
            
        }
        else if (collision.gameObject.CompareTag("Ardium"))
        {
            Ardium ardium = collision.GetComponent<Ardium>();
            Destroy(collision.gameObject);
            growthLevel += ardium.shinkAmount;
            PlayConsumedSound();
        }
        else if (collision.gameObject.CompareTag("Planet"))
        {
            Planet planet = collision.GetComponent<Planet>();
            growthLevel += planet.growthLevel + 15;
            Destroy(planet.gameObject);
            PlayConsumedSound();
        }
    }

    private void PlayConsumedSound()
    {
        if (consumeTimer < 0.5f)
        {
            consumeStreak++;
        }
        else
        {
            consumeStreak = 0;
        }

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        audioSource.pitch = (0.10f) + (0.05f * consumeStreak);
        audioSource.Play();
        consumeTimer = 0.0f;
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
        animator.SetBool("Selected", true);
    }

    public void Deselect() {
        selected = false;
        movable.selected = false;
        animator.SetBool("Selected", false);
    }
}
