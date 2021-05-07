using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParkingSpots : MonoBehaviour
{
    public Transform start;
    public Transform end;
    public List<ParkingSpot> spots = new List<ParkingSpot>();

    public ParkingSpot GetRandomEmptySpot()
    {
        IEnumerable<ParkingSpot> emptySpots = spots.Where(spot => !spot.inUse);
        int numberOfEmptySpaces = emptySpots.Count();
        if(numberOfEmptySpaces == 0)
            return null;
        return emptySpots.Skip(Random.Range(0,numberOfEmptySpaces)).First();
    }
}
