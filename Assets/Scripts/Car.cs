using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] private AIPath ai;
    [SerializeField] private SpriteRenderer carBase;
    [SerializeField] private AudioSource audioSource;

    private ParkingSpots allSpots;
    private float? leaveAtTime = null;
    private bool leaving = false;

    [MinMaxSlider(1, 200, true)]
    public Vector2 waitInSpotTime = new Vector2(10, 60);
    [MinMaxSlider(1, 3, true)]
    public Vector2 speedRange = new Vector2(1, 3);

    private SnakeData snake;

    private void Awake() 
    {
        allSpots = GameObject.FindObjectOfType<ParkingSpots>();
        ParkingSpot emptySpot = allSpots.GetRandomEmptySpot();
        if(emptySpot == null || ai == null)
        {
            // screw the GC
            Destroy(gameObject);
            return;
        }

        snake = GameObject.FindObjectOfType<SnakeData>();

        ai.destination = emptySpot.transform.position;
        emptySpot.inUse = true;
        ai.maxSpeed = Extensions.RandomRange(speedRange);

        carBase.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    }
    
    private void Update() 
    {
        if(ai.reachedDestination && leaveAtTime == null)
            ArrivedAtParkingSpot();
        else if(Time.timeSinceLevelLoad >= leaveAtTime && !leaving)
            ExitSpace();
        else if(ai.reachedDestination && leaving)
            ExitsLot();
    }

    public void ArrivedAtParkingSpot()
    {
        leaveAtTime = Time.timeSinceLevelLoad + Extensions.RandomRange(waitInSpotTime);
        // When a car arrives, they take a cart from the cart return
        GameObject.FindObjectOfType<CartReturn>()?.TakeCart();
    }
    
    public void ExitSpace()
    {
        // When a car leaves, they leave their cart behind near their location
        GameObject.FindObjectOfType<CollectableSpawner>()?.SpawnCollectableNearby(transform.position);
        ai.destination = allSpots.end.position;
        leaving = true;
        audioSource.Play();
    }

    // Screw the GC
    public void ExitsLot() => Destroy(gameObject);

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log($"{name} collided with {other.name}");
        Segment segment = other.GetComponent<Segment>();
        if(segment != null)
        {
            Debug.Log($"Car {name} hit {other.name}");
            snake.Crashed();
        }
        
    }

    // private void OnCollisionEnter2D(Collision2D other) {
    //     Segment segment = other.collider.GetComponent<Segment>();
    //     if(segment != null)
    //         snake.Crashed();
    // }
}
