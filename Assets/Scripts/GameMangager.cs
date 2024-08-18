using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameMangager : MonoBehaviour
{
    private Selectable _selectedObject;
    [SerializeField] private Spawner spawner;
    [SerializeField] private float timeBetweenWaves;
    [SerializeField] private float alertTime;
    [SerializeField] private float timeBetweenAsteroids;
    [SerializeField] private int waveCount;
    [SerializeField] private float asteroidSpeed;

    public Selectable selectedObject {
        get {
            return _selectedObject;
        }
        set {
            if (_selectedObject != null)
                _selectedObject.Deselect();
            _selectedObject = value;
            _selectedObject.Select();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnWave());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnWave() {

        float angle = Random.Range(0, 360);
        Vector3 position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f) * 10f;

        Spawner sp = Instantiate(spawner, position, Quaternion.identity);
        sp.Init(alertTime, timeBetweenAsteroids, waveCount);

        yield return new WaitForSeconds(alertTime);
        yield return new WaitForSeconds(timeBetweenWaves);

        StartCoroutine(SpawnWave());
    }
}
