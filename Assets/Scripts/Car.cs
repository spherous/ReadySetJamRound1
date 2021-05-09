using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Car : MonoBehaviour
{
    [SerializeField] private HonkDetector honkDetectorPrefab;
    [SerializeField] private AIPath ai;
    [SerializeField] private SpriteRenderer carBase;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SpriteRenderer lightsRenderer;

    public Material onLightsMat;
    public Material offLightsMat;

    private ParkingSpots allSpots;
    private float? leaveAtTime = null;
    public float? primeLeaveAtTime = null;
    private bool leaving = false;

    [MinMaxSlider(1, 200, true)]
    public Vector2 waitInSpotTime = new Vector2(10, 60);
    [MinMaxSlider(1, 3, true)]
    public Vector2 speedRange = new Vector2(1, 3);

    private SnakeData snake;

    public List<Light2D> lights = new List<Light2D>();
    private bool lightsOn = true;

    public AudioClip engineStart;
    public AudioClip shortBeepBeep;
    public AudioClip longBeepBeep;
    public AudioClip beep;
    public AudioClip wail;

    private float honkCDRemoveTime;

    ParkingSpot spot;

    private void Awake() 
    {
        allSpots = GameObject.FindObjectOfType<ParkingSpots>();
        spot = allSpots.GetRandomEmptySpot();
        if(spot == null || ai == null)
        {
            // screw the GC
            Destroy(gameObject);
            return;
        }

        snake = GameObject.FindObjectOfType<SnakeData>();

        ai.destination = spot.transform.position;
        spot.inUse = true;
        ai.maxSpeed = Extensions.RandomRange(speedRange);

        carBase.color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));

        HonkDetector honkDetector = Instantiate(honkDetectorPrefab, transform.position, transform.rotation);
        honkDetector.car = this;
    }
    
    private void Update() 
    {
        if(ai.reachedDestination && leaveAtTime == null)
            ArrivedAtParkingSpot();
        else if(Time.timeSinceLevelLoad >= primeLeaveAtTime && !leaving && !lightsOn)
            TurnCarOn();
        else if(Time.timeSinceLevelLoad >= leaveAtTime && !leaving)
            ExitSpace();
        else if(ai.reachedDestination && leaving)
            ExitsLot();
    }

    public void Honk()
    {
        if(leaveAtTime == null || leaving)
        {
            if(Time.timeSinceLevelLoad >= honkCDRemoveTime)
            {
                if(ai.maxSpeed <= 1.5f && shortBeepBeep)
                    audioSource?.PlayOneShot(shortBeepBeep);
                else if(ai.maxSpeed <= 2f && longBeepBeep)
                    audioSource?.PlayOneShot(longBeepBeep);
                else if(ai.maxSpeed <= 2.5f && beep)
                    audioSource?.PlayOneShot(beep);
                else if(wail)
                    audioSource?.PlayOneShot(wail);
                
                honkCDRemoveTime = Time.timeSinceLevelLoad + UnityEngine.Random.Range(2f, 4f);
            }
        }
    }

    private void TurnCarOn()
    {
        audioSource.PlayOneShot(engineStart);

        foreach(Light2D light in lights)
            light.enabled = true;
        
        lightsOn = true;

        lightsRenderer.material = onLightsMat;
    }

    public void ArrivedAtParkingSpot()
    {
        leaveAtTime = Time.timeSinceLevelLoad + Extensions.RandomRange(waitInSpotTime);
        primeLeaveAtTime = leaveAtTime - UnityEngine.Random.Range(2f, 6f);
        // When a car arrives, they take a cart from the cart return
        GameObject.FindObjectOfType<CartReturn>()?.TakeCart();
        // Turn off lights
        foreach(Light2D light in lights)
            light.enabled = false;
        lightsOn = false;
        lightsRenderer.material = offLightsMat;
    }
    
    public void ExitSpace()
    {
        // When a car leaves, they leave their cart behind near their location
        GameObject.FindObjectOfType<CollectableSpawner>()?.SpawnCollectableNearby(transform.position);
        ai.destination = allSpots.end.position;
        leaving = true;
        spot.inUse = false;
    }

    // Screw the GC
    public void ExitsLot() => Destroy(gameObject);

    private void OnTriggerEnter2D(Collider2D other) {
        // Debug.Log($"{name} collided with {other.name}");
        Segment segment = other.GetComponent<Segment>();
        if(segment != null)
        {
            // Debug.Log($"Car {name} hit {other.name}");
            snake.Crashed();
        }
    }
}
