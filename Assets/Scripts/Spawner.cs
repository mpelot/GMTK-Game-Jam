using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Asteroid asteroid;
    public Core core;
    public TrajectoryLine trajectoryLine;
    private float timeBetweenAsteroids;
    public float asteroidSpawnSpeed;
    public float lingerTime;

    public void Init(float alertTime, float timeBetweenAsteroids, int count) {
        this.timeBetweenAsteroids = timeBetweenAsteroids;
        trajectoryLine.startingVelocity = (-transform.position).normalized * asteroidSpawnSpeed;
        trajectoryLine.Show();
        StartCoroutine(SpawnWave(alertTime, timeBetweenAsteroids, count));
    }

    IEnumerator SpawnWave(float alertTime, float timeBetweenAsteroids, int waveCount)
    {
        yield return new WaitForSeconds(alertTime);
        for (int i = 0; i < waveCount; i++)
        {
            Asteroid ast = Instantiate(asteroid, transform.position, Quaternion.identity);
            ast.GetComponent<Rigidbody2D>().velocity = (-ast.transform.position).normalized * asteroidSpawnSpeed;

            yield return new WaitForSeconds(timeBetweenAsteroids);
        }
        yield return new WaitForSeconds(lingerTime);
        Destroy(gameObject);
    }
}
