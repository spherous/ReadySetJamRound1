using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteInEditMode]
public class Lot : MonoBehaviour
{
    [SerializeField] private GameObject spotPrefab;

    [Range(0, 20)] public int length = 0;

    public List<GameObject> spots = new List<GameObject>();

    private void OnValidate()
    {
#if UNITY_EDITOR
        int dif = length - spots.Count;

        if(dif > 0)
        {
            for(int i = 0; i < dif; i++)
            {
                Vector3 loc = spots.Count > 0 ? spots[spots.Count - 1].transform.position + new Vector3(0, -1.04f, 0) : transform.position;
                GameObject newSpot = Instantiate(spotPrefab, loc, Quaternion.identity, transform);
                spots.Add(newSpot);
            }
        }
        else if(dif < 0)
        {
            int removeUntilIndex = spots.Count + dif - 1;
            for(int i = spots.Count - 1; i > removeUntilIndex; i--)
            {
                GameObject spot = spots[i];
                spots.RemoveAt(i);
                DestroyImmediate(spot);
            }
        }
#endif
    }
}
