using UnityEngine;

public class ParkingSpot : MonoBehaviour
{
    public bool inUse;
    public Transform parkingLoc;
    public SpotType spotType;
    public GameObject handicapped;
    public GameObject EV;
    
    private void OnValidate()
    {
        switch(spotType)
        {
            case SpotType.None:
                handicapped.SetActive(false);
                EV.SetActive(false);
                break;
            case SpotType.Handicapped:
                handicapped.SetActive(true);
                EV.SetActive(false);
                break;
            case SpotType.EV:
                EV.SetActive(true);
                handicapped.SetActive(false);
                break;
        }
    }

}

public enum SpotType:int {None = 0, Handicapped = 1, EV = 2}