using UnityEngine;

public class ParkingSpot : MonoBehaviour
{
    [SerializeField] private SpriteRenderer laneRenderer;
    public bool inUse;
    public Transform parkingLoc;
    public SpotCarType spotCarType;
    public SpotType spotType;
    public GameObject handicapped;
    public GameObject EV;
    public Sprite end1;
    public Sprite end2;
    public Sprite middle;
    
    private void OnValidate()
    {
        switch(spotCarType)
        {
            case SpotCarType.None:
                handicapped.SetActive(false);
                EV.SetActive(false);
                break;
            case SpotCarType.Handicapped:
                handicapped.SetActive(true);
                EV.SetActive(false);
                break;
            case SpotCarType.EV:
                EV.SetActive(true);
                handicapped.SetActive(false);
                break;
        }

        switch(spotType)
        {
            case SpotType.Middle:
                laneRenderer.sprite = middle;
                break;
            case SpotType.End1:
                laneRenderer.sprite = end1;
                break;
            case SpotType.End2:
                laneRenderer.sprite = end2;
                break;
        }
    }
}

public enum SpotCarType:int {None = 0, Handicapped = 1, EV = 2}
public enum SpotType:int {Middle = 0, End1 = 1, End2 = 2}