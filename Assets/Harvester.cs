using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harvester : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.name.Contains("Asteroid")) {
            Destroy(collision.gameObject);
        }
    }
}
