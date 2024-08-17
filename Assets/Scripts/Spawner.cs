using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Asteroid asteroid;
    public Core core;
    private float timeBetweenAsteroids;

    public void Init(float timeBetweenAsteroids, int count) {
        this.timeBetweenAsteroids = timeBetweenAsteroids;
        StartCoroutine(SpawnAsteroid(count));
    }

    IEnumerator SpawnAsteroid(int count) {
        if (count <= 0) {
            Destroy(gameObject);
        } else {

            Asteroid ast = Instantiate(asteroid, transform.position, Quaternion.identity);
            ast.GetComponent<Rigidbody2D>().velocity = (-ast.transform.position).normalized * 0.8f;

            yield return new WaitForSeconds(timeBetweenAsteroids);

            StartCoroutine(SpawnAsteroid(count - 1));
        }
    }
}
