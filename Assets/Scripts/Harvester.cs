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
    public GameObject shrinkRockPrefab;
    public int growthLevel = 0;
    public int splitThreshold;
    private List<Collider2D> ignoredColliders;

    void Start()
    {
        gm = FindAnyObjectByType<GameMangager>();
        trajectoryLine.Hide();
        ignoredColliders = new List<Collider2D>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (selected && growthLevel > 0)
            {
                isAiming = true;
            }
        }
        if (isAiming)
        {
            if (growthLevel <= 0)
            {
                StopAiming();
            }
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
                    Fire(shotVelocity);
                }
                else if (!selected)
                {
                    StopAiming();
                }
            }
        }
    }

    void Fire(Vector2 shotVelocity)
    {
        GameObject spawnedShrinkRock = Instantiate(shrinkRockPrefab, transform.position, Quaternion.identity);
        spawnedShrinkRock.GetComponent<Rigidbody2D>().velocity = shotVelocity;
        ignoredColliders.Add(spawnedShrinkRock.GetComponent<Collider2D>());
        growthLevel--;

        StopAiming();
    }

    void StopAiming()
    {
        isAiming = false;
        trajectoryLine.Hide();
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

            if (growthLevel >= splitThreshold)
            {
                growthLevel = 0;
                GameObject spawnedHarvester = Instantiate(gameObject, transform.position, Quaternion.identity);
            }
        }
        if (collision.gameObject.CompareTag("ShrinkRock"))
        {
            Destroy(collision.gameObject);
            if (growthLevel > 0)
            {
                growthLevel--;
            }
            else
            {
                Destroy(this.gameObject);
            }
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
