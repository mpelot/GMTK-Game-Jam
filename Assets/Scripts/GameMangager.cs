using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameMangager : MonoBehaviour
{
    private int _money;
    [SerializeField] private Spawner spawner;
    [SerializeField] private float timeBetweenWaves;
    [SerializeField] private float alertTime;
    [SerializeField] private float timeBetweenAsteroids;
    [SerializeField] private int waveCount;
    [SerializeField] private float asteroidSpeed;

    public int money
    {
        get
        {
            return _money;
        }
        set
        {
            _money = value;
            moneyText.text = "$" + _money;
        }
    }
    public TextMeshProUGUI moneyText;

    // Start is called before the first frame update
    void Start()
    {
        money = 0;
        StartCoroutine(SpawnWave());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SpawnWave() {

        float angle = Random.Range(0, 360);
        Vector3 position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f) * 10f;

        //Alert

        yield return new WaitForSeconds(3f);

        Spawner sp = Instantiate(spawner, position, Quaternion.identity);
        sp.Init(timeBetweenAsteroids, waveCount);

        yield return new WaitForSeconds(timeBetweenWaves);

        StartCoroutine(SpawnWave());
    }
}
