using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private Car carPrefab;
    [SerializeField] ParkingSpots parkingSpots;

    private float spawnAtTime;

    [MinMaxSlider(1, 60, true)]
    public Vector2 carSpawnDelay = new Vector2(5, 10);

    private void Start() {
        parkingSpots = GameObject.FindObjectOfType<ParkingSpots>();
    }

    private void Update()
    {
        if(Time.timeSinceLevelLoad >= spawnAtTime)    
        {
            SpawnCar();
            spawnAtTime = Time.timeSinceLevelLoad + Extensions.RandomRange(carSpawnDelay);
        }
    }

    public void SpawnCar() => Instantiate(carPrefab, parkingSpots.GetRandomStart().position, Quaternion.identity);

    public void SpeedUp() => carSpawnDelay *= 0.95f;
}
