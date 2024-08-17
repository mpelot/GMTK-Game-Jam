using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvester : MonoBehaviour, Selectable
{
    private bool selected;
    private bool isAiming;
    private GameMangager gm;
    public TrajectoryLine trajectoryLine;

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
            trajectoryLine.Show();
            trajectoryLine.startingVelocity = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position) * 0.5f;

            if (Input.GetMouseButtonUp(1) || !selected)
            {
                isAiming = false;
                trajectoryLine.Hide();
            }
        }
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
