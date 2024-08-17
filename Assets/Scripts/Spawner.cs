using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Asteroid asteroid;
    public Core core;

    // Start is called before the first frame update
    public void Init(int count)
    {
        StartCoroutine(SpawnAsteroid(count));
    }

    IEnumerator SpawnAsteroid(int count) {

        if (count <= 0) {
            Destroy(gameObject);
        }

        Asteroid ast = Instantiate(asteroid, transform.position, Quaternion.identity);
        ast.GetComponent<Rigidbody2D>().velocity = (core.transform.position - ast.transform.position).normalized * 0.8f;

        yield return new WaitForSeconds(3f);

        StartCoroutine(SpawnAsteroid(count - 1));
    }
}
