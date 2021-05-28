using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map", menuName = "ScriptableObjects/Map", order = 1)]
public class Map : ScriptableObject
{
    public List<LightLoc> lightLocs = new List<LightLoc>();
    public GameObject parkingSpotsPrefab;
}