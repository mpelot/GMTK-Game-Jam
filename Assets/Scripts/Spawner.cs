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
    private int asteroidsOnTrajectoryLine;

    public void Init(float alertTime, float timeBetweenAsteroids, int count) {
        this.timeBetweenAsteroids = timeBetweenAsteroids;
        trajectoryLine.startingVelocity = (-transform.position).normalized * asteroidSpawnSpeed;
        trajectoryLine.Show();
        asteroidsOnTrajectoryLine = count;
        StartCoroutine(SpawnWave(alertTime, timeBetweenAsteroids, count));
    }

    IEnumerator SpawnWave(float alertTime, float timeBetweenAsteroids, int waveCount)
    {
        yield return new WaitForSeconds(alertTime);
        for (int i = 0; i < waveCount; i++)
        {
            Asteroid ast = Instantiate(asteroid, transform.position, Quaternion.identity);
            ast.GetComponent<Rigidbody2D>().velocity = (-ast.transform.position).normalized * asteroidSpawnSpeed;
            ast.parentTrajectoryLine = trajectoryLine;
            ast.spawner = this;

            yield return new WaitForSeconds(timeBetweenAsteroids);
        }
    }

    public void RemoveAsteroidFromTrajectoryLine()
    {
        asteroidsOnTrajectoryLine--;
        if (asteroidsOnTrajectoryLine <= 0)
        {
            Destroy(gameObject);
        }
    }
}
