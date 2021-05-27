using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class SnakeData : MonoBehaviour
{
    GameManager gm => GameManager.Instance;

    public delegate void PickedUpCart();
    public PickedUpCart pickedUpCart;
    public delegate void EmpiedCarts();
    public EmpiedCarts empiedCarts;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource cartRollingSource;
    public AudioClip loadingClip;
    public AudioClip crashClip;

    public delegate void OnSizeChanged(int size);
    public OnSizeChanged onSizeChanged;

    public GameObject segmentPrefab;
    public GameObject cartPusherPrefab;
    [SerializeField] public List<Segment> segments {get; private set;} = new List<Segment>();

    public int size => segments.Count;

    public float defaultSpeed;
    public float defaultTurnSpeed;
    
    [ReadOnly] public float speed;
    [ReadOnly] public float turnSpeed;
    [SerializeField] private BoxCollider2D col2D;
    public new Rigidbody2D rigidbody2D {get; private set;}

    private OnHandScore onHandScore;

    private Segment cartPusher;

    public float cartOffset;
    public bool poweredUp {get; private set;} = false;
    public bool recentlyReflected {get; private set;} = false;
    private float clearRecentReflectionAtTime;
    public GameObject reflectedOff {get; private set;} = null;
    private float powerupUntilTime;

    public Material pusherOff;
    public Material pusherOn;
    public Material cartOff;
    public Material cartOn;

    [SerializeField] private ParticleSystem bumper;

    private void Awake() {
        bumper.Stop();
        rigidbody2D = GetComponent<Rigidbody2D>();
        onHandScore = GameObject.FindObjectOfType<OnHandScore>();

        cartPusher = Instantiate(
            cartPusherPrefab,
            transform.position,
            Quaternion.identity
        ).GetComponent<Segment>();

        cartPusher.leader = transform;
        cartPusher.SetLag(0);

        speed = defaultSpeed;
        turnSpeed = defaultTurnSpeed;
    }

    public void AddSegment(int amount = 1)
    {
        onHandScore.Inc(amount);

        for(int i = 0; i < amount; i++)
        {
            Segment newSegment = Instantiate(
                original: segmentPrefab,
                position: transform.position,
                rotation: transform.rotation
            ).GetComponent<Segment>();


            newSegment.leader = transform;

            newSegment.spriteRenderer.sortingOrder = size * -1;
            cartPusher.spriteRenderer.sortingOrder = (size + 1) * -1;
            if(poweredUp)
                newSegment.spriteRenderer.material = cartOn;
                // newSegment.spriteRenderer.material.SetFloat("_HighlightOn", 1);

            speed *= 0.95f;
            turnSpeed *= 0.98f;

            float adjustedOffset = cartOffset * 0.05f;

            foreach(Segment segment in segments)
            {
                if(segment.lagSeconds == 0)
                    continue;
                    
                segment.SetLag(segment.lagSeconds + adjustedOffset);
            }

            segments.Add(newSegment);
            newSegment.SetLag((size - 1) * (cartOffset + adjustedOffset));
            cartPusher.SetLag(size * (cartOffset + adjustedOffset) + 0.1f);

            onSizeChanged?.Invoke(segments.Count);
        }

        pickedUpCart?.Invoke();
    }

    public void UpdateSnake()
    {
        for(int i = 0; i < size; i++)
        {
            segments[i].lastPosition = segments[i].transform.position;
            segments[i].lastRotation = segments[i].transform.rotation;
        }

        cartPusher.lastPosition = cartPusher.transform.position;
        cartPusher.lastRotation = cartPusher.transform.rotation;
    }

    public int TakeAllSegments()
    {
        int segCount = segments.Count;
        for(int i = segCount - 1; i >= 0; i--)
            Destroy(segments[i].gameObject);

        onHandScore.Reset();

        segments.Clear();
        speed = defaultSpeed;
        turnSpeed = defaultTurnSpeed;

        cartPusher.SetLag(0);

        empiedCarts?.Invoke();

        return segCount;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {    
        Debug.Log($"Ran into {other.name}");
        if(other.gameObject.TryGetComponent<ICollectable>(out ICollectable collectable))
        {
            // Carts must be picked up from roughly behind
            float forwardDot = Vector3.Dot(transform.up, other.transform.up);
            float dirDot = Vector3.Dot(other.transform.position - transform.position, other.transform.up);
            if(dirDot >= 2f/3f && forwardDot >= 1f/3f)
            {
                AddSegment(collectable.growthAmount);
                audioSource.PlayOneShot(loadingClip);

                // Screw the GC
                Destroy(other.gameObject);
            }
        }
        else if(other.gameObject.TryGetComponent<LoseGameOnCollision>(out LoseGameOnCollision loseGame))
        {
            // ignore the first few carts and the cart pusher.
            if(other.gameObject == cartPusher.gameObject)
                return;
            if(segments.Count > 1 && other.gameObject == segments[0].gameObject)
                return;
            if(segments.Count > 2 && other.gameObject == segments[1].gameObject)
                return;

            if(!poweredUp && !recentlyReflected && other.gameObject != reflectedOff)
                Crashed();
            else
            {
                Vector2 origin = transform.position + (-transform.up * 0.2f);
                RaycastHit2D[] hits = Physics2D.BoxCastAll(origin, col2D.size, transform.rotation.eulerAngles.z, transform.up, 1);

                foreach(RaycastHit2D hit in hits)
                {
                    if(hit.collider.TryGetComponent<LoseGameOnCollision>(out LoseGameOnCollision lgoc))
                    {
                        if(hit.collider != null && lgoc == loseGame)
                        {
                            poweredUp = false;

                            SetHighlights(0);
                            bumper.Stop();

                            if(!recentlyReflected)
                            {
                                recentlyReflected = true;
                                clearRecentReflectionAtTime = Time.timeSinceLevelLoad + 2;
                                reflectedOff = hit.collider.gameObject;
                            }
                            transform.up = Vector2.Reflect(transform.up, hit.normal);
                        }
                    }
                    else
                        continue;
                }
            }
        }
    }

    public void Crashed()
    {
        if(!cartPusher.follow)
            return;

        audioSource.PlayOneShot(crashClip);
        cartRollingSource.Stop();
        
        speed = 0;
        rigidbody2D.velocity = Vector3.zero;

        bumper.Pause();

        foreach(Segment segment in segments)
            segment.follow = false;
        cartPusher.follow = false;
        
        gm.LoseGame();
    }

    public void Powerup(float seconds)
    {
        if(seconds < 1)
            return;

        poweredUp = true;
        powerupUntilTime = Time.timeSinceLevelLoad + seconds;

        SetHighlights(1);
        bumper.Play();
    }

    private void Update()
    {
        if(recentlyReflected && Time.timeSinceLevelLoad >= clearRecentReflectionAtTime)
        {
            recentlyReflected = false;
            reflectedOff = null;
        }
        if(poweredUp && Time.timeSinceLevelLoad >= powerupUntilTime)
        {
            poweredUp = false;
            bumper.Stop();
            SetHighlights(0);
        }
    }

    private void SetHighlights(float val)
    {
        // cartPusher.spriteRenderer.material.SetFloat("_HighlightOn", val);
        cartPusher.spriteRenderer.material = val > 0.5f ? pusherOn : pusherOff;
        foreach(Segment segment in segments)
            segment.spriteRenderer.material = val > 0.5f ? cartOn : cartOff;
            // segment.spriteRenderer.material.SetFloat("_HighlightOn", val);
    }
}