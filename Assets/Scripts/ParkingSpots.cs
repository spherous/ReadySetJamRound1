using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParkingSpots : MonoBehaviour
{
    public List<Transform> start;
    public List<Transform> end;
    public List<ParkingSpot> spots = new List<ParkingSpot>();

    public ParkingSpot GetRandomEmptySpot()
    {
        IEnumerable<ParkingSpot> emptySpots = spots.Where(spot => !spot.inUse);
        int numberOfEmptySpaces = emptySpots.Count();
        if(numberOfEmptySpaces == 0)
            return null;
        return emptySpots.Skip(Random.Range(0,numberOfEmptySpaces)).First();
    }

    public Transform GetRandomStart() => start[UnityEngine.Random.Range(0, start.Count)];
    public Transform GetRandomEnd() => end[UnityEngine.Random.Range(0, end.Count)];
}
