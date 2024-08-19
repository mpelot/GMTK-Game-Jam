using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMangager : MonoBehaviour
{
    private Selectable _selectedObject;
    [SerializeField] private Spawner spawner;
    public Planet planetPrefab;
    public Harvester harvesterPrefab;
    [SerializeField] private float asteroidSpeed;
    [SerializeField] private float spawnDistance;
    public Round[] rounds;
    [SerializeField] private GUIController guiController;
    public int startingRound;
    public Text tutorialText;
    private Coroutine currentTutorialTextCoroutine;
    public Planet tutorialPlanet;
    public Harvester tutorialHarvester;

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
        tutorialText.text = "";
        tutorialPlanet = GetComponentInChildren<Planet>();
        tutorialHarvester = GetComponentInChildren<Harvester>();
        if (startingRound < 0)
        {
            tutorialPlanet.gameObject.SetActive(true);
            tutorialHarvester.gameObject.SetActive(true);
            startingRound = 0;
            StartCoroutine(IntroSequence());
        }
        else
        {
            tutorialPlanet.gameObject.SetActive(false);
            tutorialHarvester.gameObject.SetActive(false);
            SpawnInMissingObjects();
            StartCoroutine(StartRounds());
        }
    }

    private void SpawnInMissingObjects()
    {
        int planetsToSpawn = 1;
        float totalGrowth = 0;

        for (int round = 0; round < startingRound; round++)
        {
            for (int roundRepeat = 0; roundRepeat <= rounds[round].roundRepeatCount; roundRepeat++)
            {
                for (int wave = 0; wave < rounds[round].waves.Length; wave++)
                {
                    for (int waveRepeat = 0; waveRepeat <= rounds[round].waves[wave].waveRepeatCount; waveRepeat++)
                    {
                        if (rounds[round].waves[wave].spawnNewPlanet)
                        {
                            planetsToSpawn++;
                        }
                        else
                        {
                            totalGrowth += rounds[round].waves[wave].asteroidGrowthLevel * rounds[round].waves[wave].asteroidsPerStream * rounds[round].waves[wave].numberOfStreams;
                        }
                    }
                }
            }
        }

        int harvestersToSpawn = Mathf.FloorToInt((totalGrowth / harvesterPrefab.splitThreshold) * 0.5f) + 2;

        for (int i = 0; i < planetsToSpawn; i++)
        {
            SpawnPlanet(Random.Range(0, 360), Random.Range(4, 7), false);
        }

        for (int i = 0; i < harvestersToSpawn; i++)
        {
            SpawnHarvester(Random.Range(0, 360), Random.Range(4, 7));
        }




    }

    public void updateUI(Selectable s) {
        guiController.changeSelected(s);
    }

    IEnumerator IntroSequence()
    {
        tutorialHarvester.GetComponent<Movable>().disableInteraction = true;
        Camera.main.GetComponent<CameraMovement>().enabled = false;

        SetTutorialText("INCOMING ASTEROID\nSTREAMS ARE\nTARGETING THE SUN\nOF A NEARBY\nSOLAR SYSTEM...");

        yield return new WaitForSeconds(4f);
        SpawnAsteroidStream(270f, 10f, 0f, 3, 1.0f);

        yield return new WaitForSeconds(5f);

        SpawnAsteroidStream(315f, 10f, 2f, 2, 1.0f);
        SpawnAsteroidStream(225f, 10f, 2f, 2, 1.0f);


        yield return new WaitForSeconds(2f);

        SetTutorialText("THE INCREASED\nMASS IS GROWING\nTHE SUN'S CORE,\nPULLING IN NEARBY\nPLANETS...");

        yield return new WaitForSeconds(5f);
        
        tutorialPlanet.growthLevel = tutorialPlanet.unstableGrowthThreshold;

        yield return new WaitForSeconds(10f);

        SetTutorialText("YOU MUST MAKE USE OF THE ALIENTECH TETRADON MACHINES TO INTERCEPT THE ASTEROIDS AND PROTECT THE SUN");

        tutorialHarvester.GetComponent<Movable>().targetPos = new Vector3(0, 4, 0);
        tutorialHarvester.GetComponent<Movable>().destinationSet = true;
        tutorialHarvester.GetComponent<Movable>().dragSpeedMultiplier = 8f;

        yield return new WaitForSeconds(11f);

        SetTutorialText("THE ADVANCED TECHNOLOGY OF THE TETRADON MACHINES ALLOWS THEM TO GROW IN SIZE AND POWER AS THEY ABSORB ASTEROIDS");

        yield return new WaitForSeconds(4f);

        tutorialHarvester.GetComponent<Movable>().dragSpeedMultiplier = 1f;
        SpawnAsteroidStream(90f, 15f, 0f, 5, 1.0f);

        while (tutorialHarvester.growthLevel == 0)
        {
            yield return null;
        }

        SetTutorialText("LEFT CLICK TO SELECT THE TETRADON...");

        while (selectedObject == null || selectedObject.GetType() != typeof(Harvester))
        {
            yield return null;
        }

        SetTutorialText("...THEN DRAG WITH RIGHT CLICK TO FIRE ARDIUM INTO THE SUN'S CORE");
        
        while (tutorialHarvester.growthLevel > 0)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2.0f);
        tutorialHarvester.ClearFiringPath();

        SpawnAsteroidStream(90f, 15f, 0f, 5, 1.0f);
        tutorialPlanet = SpawnPlanet(135f, 15f);

        yield return new WaitForSeconds(2f);
        SetTutorialText("FIRE ARDIUM INTO THE INCOMING PLANET TO SAVE IT FROM THE SUN!\n(PAN THE CAMERA WITH MIDDLE MOUSE CLICK).");
        Camera.main.GetComponent<CameraMovement>().enabled = true;

        while (tutorialPlanet.growthLevel == tutorialPlanet.unstableGrowthThreshold)
        {
            yield return null;
        }

        while (tutorialHarvester.growthLevel > 0)
        {
            yield return null;
        }

        SetTutorialText("GREAT JOB! YOU'VE SAVED THE PLANET. YOU CAN NOW CONTROL THE PLANET BY DRAGGING IT.");

        while (selectedObject == null || selectedObject.GetType() != typeof(Planet))
        {
            yield return null;
        }

        yield return new WaitForSeconds(4f);

        SetTutorialText("YOU CAN POSITION THE GRAVITATIONAL PULL OF THE PLANET TO REDIRECT INCOMING ASTEROIDS INTO THE TETRADON!");
        yield return new WaitForSeconds(4f);
        FindAnyObjectByType<Core>().disableGrowing = true;

        while (tutorialHarvester.growthLevel == 0)
            {
            Spawner sp = SpawnAsteroidStream(180f, spawnDistance, 10f, 5, 1.0f);

            while (sp != null)
            {
                yield return null;
            }

            yield return new WaitForSeconds(2.0f);
        }

        SetTutorialText("CONTINUE TO FEED ASTEROIDS INTO THE TETRADON TO GROW IT TO FULL SIZE");

        while (FindObjectsByType<Harvester>(FindObjectsSortMode.None).Length < 2)
        {
            Spawner sp = SpawnAsteroidStream(180f, spawnDistance, 0f, 5, 1.0f);

            while (sp != null)
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(1.0f);

        SetTutorialText("TETRADON ARE BIOLOGICAL MACHINES AND HAVE THE ABILITY TO SELF-REPLICATE. THEY WILL GROW IN NUMBER AS THEY ABSORB ASTEROIDS");

        yield return new WaitForSeconds(7f);

        SetTutorialText("BALANCING THE GROWTH OF TETRADONS AND KEEPING THE SUN'S CORE STABLE IS KEY TO SAVING THE SOLAR SYSTEM.");

        yield return new WaitForSeconds(8f);

        SetTutorialText("TO PREPARE FOR THE INCOMING ASTEROID STORM, POSITION YOUR TETRADONS ON EITHER SIDE OF THE SUN TO COVER THE MOST GROUND.");


        Harvester[] harvesters = FindObjectsByType<Harvester>(FindObjectsSortMode.None);

        foreach (Harvester harvester in harvesters)
        {
            harvester.GetComponent<Movable>().disableInteraction = false;
        }
        while (! isPassingThroughOrigin(harvesters[0].transform.position, harvesters[1].transform.position, 1f))
        {
            yield return null;
        }
        FindAnyObjectByType<Core>().disableGrowing = false;

        SetTutorialText("WARNING: INCOMING ASTEROID STORM DETECTED. PREPARE FOR IMPACT.");

        yield return new WaitForSeconds(5f);

        SetTutorialText("");

        yield return new WaitForSeconds(2f);

        StopCoroutine(currentTutorialTextCoroutine);
        tutorialText.text = "";

        StartCoroutine(StartRounds());
    }

    void SetTutorialText(string text)
    {
        if (currentTutorialTextCoroutine != null)
        {
            StopCoroutine(currentTutorialTextCoroutine);
        }
        currentTutorialTextCoroutine = StartCoroutine(SetTutorialTextCoroutine(text));
    }

    IEnumerator SetTutorialTextCoroutine(string text)
    {
        string currentText = tutorialText.text;
        for (int i = 0; i < currentText.Length; i++)
        {
            tutorialText.text = currentText.Substring(0, currentText.Length - 1) + "_";
            yield return new WaitForSeconds(0.01f);
        }
        
        for (int i = 0; i < text.Length; i++)
        {
            tutorialText.text = text.Substring(0, i + 1) + "_";
            yield return new WaitForSeconds(0.02f);
        }

        
        // Blinking cursor
        while (true)
        {
            tutorialText.text = text + "_";
            yield return new WaitForSeconds(0.5f);
            tutorialText.text = text;
            yield return new WaitForSeconds(0.5f);
        }
        

    }

    bool isPassingThroughOrigin(Vector2 startPos, Vector2 endPos, float distanceThreshold)
    {
        Vector2 lineDir = endPos - startPos;
        float t = -Vector2.Dot(startPos, lineDir) / Vector2.Dot(lineDir, lineDir);
        t = Mathf.Clamp01(t);

        Vector2 closestPoint = startPos + t * lineDir;
        float distance = Vector2.Distance(Vector2.zero, closestPoint);

        return distance <= distanceThreshold;
    }

    // angle of 0 is to the right, 90 is above
    private Spawner SpawnAsteroidStream(float angle, float spawnDistance, float alertTime, int count, float asteroidGrowthLevel)
    {
        Vector3 position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f) * spawnDistance;

        Spawner sp = Instantiate(spawner, position, Quaternion.identity);
        sp.Init(alertTime, count, asteroidGrowthLevel, asteroidSpeed);

        return sp;
    }

    private Planet SpawnPlanet(float angle, float spawnDistance, bool isUnstable = true)
    {
        Vector3 position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f) * spawnDistance;

        Planet planet = Instantiate(planetPrefab, position, Quaternion.identity);
        if (isUnstable)
        {
            planet.growthLevel = planet.unstableGrowthThreshold;
        }
        else
        {
            planet.growthLevel = 0f;
        }
            

        return planet;
    }

    private Harvester SpawnHarvester(float angle, float spawnDistance)
    {
        Vector3 position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f) * spawnDistance;

        Harvester harvester = Instantiate(harvesterPrefab, position, Quaternion.identity);

        return harvester;
    }

    IEnumerator StartRounds()
    {
        for (int round = startingRound; round < rounds.Length; round++)
        {
            for (int roundRepeat = 0; roundRepeat <= rounds[round].roundRepeatCount; roundRepeat++)
            {
                yield return new WaitForSeconds(rounds[round].startOfRoundTime);
                for (int currentWave = 0; currentWave < rounds[round].waves.Length; currentWave++)
                {
                    Wave wave = rounds[round].waves[currentWave];
                    if (! wave.spawnNewPlanet)
                    {
                        for (int waveRepeat = 0; waveRepeat <= wave.waveRepeatCount; waveRepeat++)
                        {
                            Debug.Log("Round: " + round + "(" + roundRepeat + "/" + rounds[round].roundRepeatCount + ")" +
                            ", Wave: " + currentWave + "(" + waveRepeat + "/" + wave.waveRepeatCount + ")");
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
                        for (int waveRepeat = 0; waveRepeat <= wave.waveRepeatCount; waveRepeat++)
                        {
                            Debug.Log("Round: " + round + "(" + roundRepeat + "/" + rounds[round].roundRepeatCount + ")" +
                            ", Wave: " + currentWave + "(" + waveRepeat + "/" + wave.waveRepeatCount + ") (New Planet!)");
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
}
