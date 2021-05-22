using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
    [SerializeField] private Powerup powerupPrefab;
    public float spawnDelay;
    float spawnAtTime;

    private void Update() 
    {
        if(Time.timeSinceLevelLoad >= spawnAtTime)
        {
            spawnAtTime = Time.timeSinceLevelLoad + spawnDelay;
            SpawnPowerup();
        }
    }

    public void SpawnPowerup()
    {
        Vector3 spawnAtPos = new Vector3(
            UnityEngine.Random.Range(-9f, 9f),
            UnityEngine.Random.Range(-5f, 5f), 
            0f
        );

        Powerup newPowerup = Instantiate(powerupPrefab, spawnAtPos, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360)));
    }

}
