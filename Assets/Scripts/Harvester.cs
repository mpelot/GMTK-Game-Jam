using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvester : MonoBehaviour, Selectable
{
    private GameMangager gameManager;
    public int moneyPerAsteroid;
    private bool selected;
    private GameMangager gm;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameMangager>();
        gm = FindAnyObjectByType<GameMangager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("Asteroid"))
        {
            Destroy(collision.gameObject);
            gameManager.money += moneyPerAsteroid;
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
