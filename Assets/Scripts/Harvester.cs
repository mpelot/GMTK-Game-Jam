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

    void Start()
    {
        gm = FindAnyObjectByType<GameMangager>();
        trajectoryLine.Hide();
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

        StopAiming();
    }

    void StopAiming()
    {
        isAiming = false;
        trajectoryLine.Hide();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("Asteroid"))
        {
            Destroy(collision.gameObject);
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
