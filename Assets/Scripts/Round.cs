using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Round
{
    public float startOfRoundTime;
    public Wave[] waves;
    public float timeBetweenWaves;
    public int roundRepeatCount;
}
