using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvester : MonoBehaviour, Selectable
{
    private GameMangager gameManager;
    public int moneyPerAsteroid;
    private bool selected;

    public void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameMangager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("Asteroid"))
        {
            Destroy(collision.gameObject);
            gameManager.money += moneyPerAsteroid;
        }
    }
    public void Select() {
        selected = true;
    }

    public void Deselect() {
        selected = false;
    }
}
