using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameMangager : MonoBehaviour
{
    private Selectable _selectedObject;
    [SerializeField] private Spawner spawner;
    [SerializeField] private float asteroidSpeed;
    [SerializeField] private float timeBetweenAsteroids;
    [SerializeField] private float spawnDistance;
    public Round[] rounds;

    public Selectable selectedObject {
        get {
            return _selectedObject;
        }
        set {
            if (_selectedObject != null)
                _selectedObject.Deselect();
            _selectedObject = value;
            if (_selectedObject != null)
                _selectedObject.Select();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartRounds());
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
                    for (int waveRepeat = 0; waveRepeat <= wave.waveRepeatCount; waveRepeat++)
                    {

                        yield return new WaitForSeconds(wave.startOfWaveTime);

                        for (int stream = 0; stream < wave.numberOfStreams; stream++)
                        {
                            
                            float angle = Random.Range(0, 360);
                            Vector3 position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f) * spawnDistance;

                            Spawner sp = Instantiate(spawner, position, Quaternion.identity);
                            sp.Init(wave.alertTime, timeBetweenAsteroids, wave.asteroidsPerStream, wave.asteroidGrowthLevel, wave.spawnNewPlanet, asteroidSpeed);
                        }

                        yield return new WaitForSeconds(round.timeBetweenWaves);
                    }
                }
            }
        }
    }
}
