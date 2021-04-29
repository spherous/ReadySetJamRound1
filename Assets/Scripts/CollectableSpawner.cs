using UnityEngine;
using System.Collections.Generic;

public class CollectableSpawner : MonoBehaviour
{
    // [SerializeField] private List<Sprite> sprites = new List<Sprite>();

    [SerializeField] private GameObject collectablePrefab;

    [SerializeField] private float spawnSpeed;
    private float spawnTime = 0;

    private void Update()
    {
        if(Time.timeSinceLevelLoad > spawnTime)
        {
            spawnTime = Time.timeSinceLevelLoad + spawnSpeed;
            SpawnCollectable();
        }
    }

    private void SpawnCollectable()
    {
        GameObject newCollectable = Instantiate(collectablePrefab, 
            new Vector3(
                Random.Range(-10f, 10f), 
                Random.Range(-6f, 6f), 
                0f
            ), Quaternion.Euler(0, 0, Random.Range(0, 360))
        );
        // Change the sprite
        // newCollectable.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Count - 1)];
        // randomize the size
        // newCollectable.transform.localScale *= Random.Range(0.75f, 1.5f);
        // randomize the value
        // newCollectable.GetComponent<ICollectable>().growthAmount = Random.Range(1,10);
    }
}