using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public float startOfWaveTime;
    public float alertTime;
    public int numberOfStreams = 1;
    public int asteroidsPerStream = 5;
    public float asteroidGrowthLevel = 1.0f;
    public bool spawnNewPlanet = false;
    public int waveRepeatCount;
}
