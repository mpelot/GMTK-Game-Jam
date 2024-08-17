using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvester : MonoBehaviour
{
    private GameMangager gameManager;
    public int moneyPerAsteroid;

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
}
