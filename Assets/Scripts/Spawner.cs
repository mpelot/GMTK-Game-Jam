using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Asteroid asteroid;
    public Core core;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn() {
        //float angle = Random.Range(0, 360);
        float angle = 50f;
        Vector3 position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f) * 10f;
        Asteroid ast = Instantiate(asteroid, position, Quaternion.identity);
        ast.GetComponent<Rigidbody2D>().velocity = (core.transform.position - ast.transform.position).normalized * 0.8f;

        angle = 250f;
        position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f) * 10f;
        Asteroid ast2 = Instantiate(asteroid, position, Quaternion.identity);
        ast2.GetComponent<Rigidbody2D>().velocity = (core.transform.position - ast2.transform.position).normalized * 0.8f;

        yield return new WaitForSeconds(3f);

        StartCoroutine(Spawn());
    }
}
