using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class CollectableSpawner : MonoBehaviour
{
    [SerializeField] private GameObject collectablePrefab;
    [SerializeField] private float spawnSpeed;
    private float spawnTime = 0;
    public int prewarmAmount;
    public LayerMask mask;

    private void Start() {
        for(int i = 0; i < prewarmAmount; i++)
            SpawnCollectableRandom();
    }

    private void Update()
    {
        if(Time.timeSinceLevelLoad > spawnTime)
        {
            spawnTime = Time.timeSinceLevelLoad + spawnSpeed;
            SpawnCollectableRandom();
        }
    }

    public void SpawnCollectableRandom()
    {
        Vector3 spawnAtPos = new Vector3(
            Random.Range(-9f, 9f), 
            Random.Range(-5f, 5f), 
            0f
        );

        Collider2D occupied = Physics2D.OverlapCircle(spawnAtPos, .5f, mask);
        
        if(occupied != null)
        {
            Transform root = occupied.transform.root;
            GenericCollectable collectable = root.GetComponent<GenericCollectable>();
            if(collectable != null)
            {
                collectable.Grow();
                return;
            }
            CartReturn cartReturn = root.GetComponent<CartReturn>();
            if(cartReturn != null)
            {
                Debug.Log(cartReturn.name);
                cartReturn.AddCarts(1, true);
                return;
            }
        }

        SpawnCart(spawnAtPos);
    }

    private void SpawnCart(Vector3 spawnAtPos)
    {
        GameObject newCollectable = Instantiate(collectablePrefab,
            spawnAtPos,
            Quaternion.Euler(0, 0, Random.Range(0, 360))
        );
    }

    public void SpawnCollectableNearby(Vector3 position)
    {
        Vector3 pos = position + new Vector3(
            Random.Range(-1f, 1f), 
            Random.Range(-1f, 1f), 
            0f
        );
        
        Collider2D occupied = Physics2D.OverlapCircle(pos, .5f, mask);
        if(occupied != null)
        {
            Transform root = occupied.transform.root;
            GenericCollectable collectable = root.GetComponent<GenericCollectable>();
            if(collectable != null)
            {
                collectable.Grow();
                return;
            }
            CartReturn cartReturn = root.GetComponent<CartReturn>();
            if(cartReturn != null)
            {
                Debug.Log(cartReturn.name);
                cartReturn.AddCarts(1, true);
                return;
            }
        }

        SpawnCart(pos);
    }
}