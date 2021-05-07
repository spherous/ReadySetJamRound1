using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class GenericCollectable : MonoBehaviour, ICollectable
{
    [SerializeField] private GameObject cartPrefab;
    public int growthAmount {get{return GrowthAmount;} set{}}
    private int GrowthAmount = 1;

    public float timeTillDestroy = 10f;
    private float destoryAtTime;
    private float enableObstacleTime;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private DynamicGridObstacle obstacle;
    List<DynamicGridObstacle> obstacles = new List<DynamicGridObstacle>();

    private void Awake()
    {
        obstacles.Add(obstacle);
        enableObstacleTime = Time.timeSinceLevelLoad + 2;
        destoryAtTime = Time.timeSinceLevelLoad + timeTillDestroy;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(Time.timeSinceLevelLoad >= enableObstacleTime && !obstacle.enabled)
        {
            foreach(DynamicGridObstacle DGO in obstacles)
                DGO.enabled = true;
        }
        else if(Time.timeSinceLevelLoad >= destoryAtTime)
            Destroy(gameObject);
        else
        {
            Color color = spriteRenderer.color;
            color.a = Mathf.Lerp(0f, 1f, (destoryAtTime - Time.timeSinceLevelLoad) / timeTillDestroy);
            spriteRenderer.color = color;
        }
    }

    public void Grow()
    {
        GameObject newCart = Instantiate(cartPrefab, transform.position + (-transform.up * growthAmount * 0.15f), transform.rotation, transform.root);
        SpriteRenderer newSprite = newCart.GetComponent<SpriteRenderer>();
        obstacles.Add(newCart.GetComponentInChildren<DynamicGridObstacle>());
        newSprite.sortingOrder = growthAmount * -1;
        enableObstacleTime = Time.timeSinceLevelLoad + 2;
        obstacle.enabled = false;
        GrowthAmount++;
    }
}