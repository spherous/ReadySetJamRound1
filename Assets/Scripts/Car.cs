using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Car : MonoBehaviour
{
    [SerializeField] private HonkDetector honkDetectorPrefab;
    private HonkDetector honkDetector;
    [SerializeField] private AIPath ai;
    [SerializeField] private Seeker seeker;
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

    private bool destroy = false;

    public List<Sprite> baseAlternates = new List<Sprite>();
    float defaultMaxSpeed;
    public float speedAdjustmentTime;
    float speedAdjustmentEllapsedTime;
    bool adjustSpeed = false;
    float targetMaxSpeed;
    float startMaxSpeed;
    bool reversing = false;

    public List<SpriteRenderer> renderers = new List<SpriteRenderer>();
    public Transform light2DContainer;

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

        ai.destination = spot.parkingLoc.position;
        spot.inUse = true;
        defaultMaxSpeed = Extensions.RandomRange(speedRange);
        ai.maxSpeed = defaultMaxSpeed;

        carBase.sprite = baseAlternates[UnityEngine.Random.Range(0, baseAlternates.Count)];
        carBase.color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));

        honkDetector = Instantiate(honkDetectorPrefab, transform.position, transform.rotation);
        honkDetector.car = this;
    }
    
    private void Update() 
    {
        if(destroy)
        {
            Destroy(honkDetector.gameObject);
            Destroy(gameObject);
        }
        if(!ai.reachedDestination && leaveAtTime == null && !reversing && !leaving)
        {
            // On our way to parking spot
            // if(seeker.GetCurrentPath().path.Any(node => !node.Walkable))
            // {
            //     Debug.Log("invalid path");
            //     spot.inUse = false;
            //     spot = allSpots.GetRandomEmptySpot();
            //     ai.destination = spot.parkingLoc.position;
            //     spot.inUse = true;
            // }
            // else
            // {
            //     spot.inUse = false;
            //     Transform end = allSpots.GetRandomEnd();
            //     ai.destination = end.position;
            // }
        }
        else if(ai.reachedDestination && leaveAtTime == null && !reversing)
            ArrivedAtParkingSpot();
        else if(Time.timeSinceLevelLoad >= primeLeaveAtTime && !leaving && !lightsOn && !reversing)
            TurnCarOn();
        else if(Time.timeSinceLevelLoad >= leaveAtTime && !leaving && !reversing)
            ExitSpace();
        else if(ai.reachedDestination && reversing)
            EndBackup();
        else if(ai.reachedDestination && leaving && !reversing)
            ExitsLot();

        if(adjustSpeed)
        {
            speedAdjustmentEllapsedTime += Time.deltaTime;
            ai.maxSpeed = Mathf.Lerp(startMaxSpeed, targetMaxSpeed, Mathf.Clamp01(speedAdjustmentEllapsedTime/speedAdjustmentTime));

            if(ai.maxSpeed == targetMaxSpeed)
            {
                adjustSpeed = false;
                speedAdjustmentEllapsedTime = 0;
            }
        }
    }

    public void Honk()
    {
        if(!leaveAtTime.HasValue || leaving)
        {
            if(Time.timeSinceLevelLoad >= honkCDRemoveTime)
            {
                if(ai.maxSpeed <= 1.5f && shortBeepBeep != null)
                    audioSource?.PlayOneShot(shortBeepBeep);
                else if(ai.maxSpeed <= 2f && longBeepBeep != null)
                    audioSource?.PlayOneShot(longBeepBeep);
                else if(ai.maxSpeed <= 2.5f && beep != null)
                    audioSource?.PlayOneShot(beep);
                else if(wail != null)
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
        // leaving = true;
        NNInfo info = AstarPath.active.GetNearest(transform.position + (-transform.up * 2.5f), NNConstraint.Default);
        ai.destination = info.position;
        reversing = true;

        RotateForBackup();
    }

    private void RotateForBackup()
    {
        // transform.up *= -1;
        Vector3 rot = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z + 180f);

        foreach(SpriteRenderer spriteRenderer in renderers)
            spriteRenderer.transform.Rotate(Vector3.forward, 180);
        
        light2DContainer.Rotate(Vector3.forward, 180);
    }

    private void EndBackup()
    {
        RotateForBackup();
        leaving = true;
        reversing = false;
        spot.inUse = false;
        
        NNInfo info = AstarPath.active.GetNearest(allSpots.GetRandomEnd().position, NNConstraint.Default);
        ai.destination = info.position;    
    }

    // Screw the GC
    public void ExitsLot() => destroy = true;

    private void OnTriggerEnter2D(Collider2D other) {
        // Debug.Log($"{name} collided with {other.name}");
        if(other.TryGetComponent<Segment>(out Segment segment))
        {
            SnakeData snake = GameObject.FindObjectOfType<SnakeData>();
            if(snake != null && snake.recentlyReflected && snake.reflectedOff == gameObject)
                return;
            // Debug.Log($"Car {name} hit {other.name}");
            snake.Crashed();
        }
    }

    public void SlowDown()
    {
        startMaxSpeed = defaultMaxSpeed;
        adjustSpeed = true;
        speedAdjustmentEllapsedTime = 0;
        targetMaxSpeed = defaultMaxSpeed * 0.5f;
    }

    public void SpeedUp()
    {
        startMaxSpeed = ai.maxSpeed;
        adjustSpeed = true;
        speedAdjustmentEllapsedTime = 0;
        targetMaxSpeed = defaultMaxSpeed;
    }

    public void FleeFromSnake(Vector3 snakePos)
    {
        if(!leaving && !reversing && lightsOn && !ai.reachedDestination)
        {
            ParkingSpot furthestSpot = allSpots.spots.Where(s => !s.inUse).OrderByDescending(s => (s.parkingLoc.position - snakePos).sqrMagnitude).FirstOrDefault();
            spot.inUse = false;
            ai.destination = furthestSpot.parkingLoc.position;
            furthestSpot.inUse = true;
            spot = furthestSpot;
        }
    }
}