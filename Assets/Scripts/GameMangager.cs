using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameMangager : MonoBehaviour
{
    private Selectable _selectedObject;
    [SerializeField] private Spawner spawner;
    public Planet planetPrefab;
    [SerializeField] private float asteroidSpeed;
    [SerializeField] private float spawnDistance;
    public Round[] rounds;
    [SerializeField] private GUIController guiController;

    public Selectable selectedObject {
        get {
            return _selectedObject;
        }
        set {
/*            if (_selectedObject != null)
                _selectedObject.Deselect();*/
            _selectedObject = value;
            if (_selectedObject != null)
                _selectedObject.Select();
            guiController.changeSelected(_selectedObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartRounds());
    }

    IEnumerator IntroSequence()
    {
        Harvester harvester = FindFirstObjectByType<Harvester>();
        harvester.GetComponent<Movable>().enabled = false;
        Camera.main.GetComponent<CameraMovement>().enabled = false;

        yield return new WaitForSeconds(2f);
        SpawnAsteroidStream(270f, 10f, 0f, 3, 1.0f);

        yield return new WaitForSeconds(5f);

        SpawnAsteroidStream(90f, 15f, 0f, 5, 1.0f);

        yield return new WaitForSeconds(20f);

        
        while (harvester.growthLevel > 0)
        {
            yield return null;
        }
        harvester.ClearFiringPath();

        SpawnAsteroidStream(90f, 15f, 0f, 5, 1.0f);
        Planet planet = SpawnPlanet(135f, 15f);

        yield return new WaitForSeconds(2f);
        Camera.main.GetComponent<CameraMovement>().enabled = true;

        while (planet.growthLevel == planet.unstableGrowthThreshold)
        {
            yield return null;
        }

        while (harvester.growthLevel > 0)
        {
            yield return null;
        }

        harvester.GetComponent<Movable>().enabled = true;

        yield return new WaitForSeconds(2.5f);

        while (harvester.growthLevel == 0)
        {
            Spawner sp = SpawnAsteroidStream(180f, spawnDistance, 0f, 5, 1.0f);

            while (sp != null)
            {
                yield return null;
            }

            yield return new WaitForSeconds(2.0f);
        }

        StartCoroutine(StartRounds());
    }

    // angle of 0 is to the right, 90 is above
    private Spawner SpawnAsteroidStream(float angle, float spawnDistance, float alertTime, int count, float asteroidGrowthLevel)
    {
        Vector3 position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f) * spawnDistance;

        Spawner sp = Instantiate(spawner, position, Quaternion.identity);
        sp.Init(alertTime, count, asteroidGrowthLevel, asteroidSpeed);

        return sp;
    }

    private Planet SpawnPlanet(float angle, float spawnDistance)
    {
        Vector3 position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f) * spawnDistance;

        Planet planet = Instantiate(planetPrefab, position, Quaternion.identity);
        planet.growthLevel = planet.unstableGrowthThreshold;

        return planet;
    }

    IEnumerator StartRounds()
    {
        foreach (Round round in rounds)
        {
            for (int roundRepeat = 0; roundRepeat <= round.roundRepeatCount; roundRepeat++)
            {
                yield return new WaitForSeconds(round.startOfRoundTime);
                foreach (Wave wave in round.waves)
                {
                    if (! wave.spawnNewPlanet)
                    {
                        for (int waveRepeat = 0; waveRepeat <= wave.waveRepeatCount; waveRepeat++)
                        {

                            yield return new WaitForSeconds(wave.startOfWaveTime);

                            for (int stream = 0; stream < wave.numberOfStreams; stream++)
                            {

                                float angle = Random.Range(0, 360);
                                Vector3 position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f) * spawnDistance;

                                Spawner sp = Instantiate(spawner, position, Quaternion.identity);
                                sp.Init(wave.alertTime, wave.asteroidsPerStream, wave.asteroidGrowthLevel, asteroidSpeed);
                            }

                            yield return new WaitForSeconds(wave.endOfWaveTime);
                        }
                    }
                    else
                    {
                        yield return new WaitForSeconds(wave.startOfWaveTime);

                        float angle = Random.Range(0, 360);
                        Vector3 position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f) * spawnDistance;

                        Planet planet = Instantiate(planetPrefab, position, Quaternion.identity);
                        planet.growthLevel = planet.unstableGrowthThreshold;

                        yield return new WaitForSeconds(wave.endOfWaveTime);
                    }
                    
                }
            }
        }
    }
}
