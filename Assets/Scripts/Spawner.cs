using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Asteroid asteroid;
    public Core core;
    public TrajectoryLine trajectoryLine;
    private int asteroidsOnTrajectoryLine;

    public void Init(float alertTime, int count, float asteroidGrowthLevel, float asteroidSpawnSpeed) {
        trajectoryLine.startingVelocity = (-transform.position).normalized * asteroidSpawnSpeed;
        trajectoryLine.Show();
        asteroidsOnTrajectoryLine = count;
        float timeBetweenAsteroids = 0.525f / asteroidSpawnSpeed;
        StartCoroutine(SpawnWave(alertTime, timeBetweenAsteroids, count, asteroidGrowthLevel, asteroidSpawnSpeed));
    }

    IEnumerator SpawnWave(float alertTime, float timeBetweenAsteroids, int count, float asteroidGrowthLevel, float asteroidSpawnSpeed)
    {
        yield return new WaitForSeconds(alertTime);
        for (int i = 0; i < count; i++)
        {
            Asteroid ast = Instantiate(asteroid, transform.position, Quaternion.identity);
            ast.GetComponent<Rigidbody2D>().velocity = (-ast.transform.position).normalized * asteroidSpawnSpeed;
            ast.parentTrajectoryLine = trajectoryLine;
            ast.spawner = this;
            ast.growthLevel = asteroidGrowthLevel;

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
