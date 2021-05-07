﻿using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class SnakeData : MonoBehaviour
{
    GameManager gm => GameManager.Instance;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource cartRollingSource;
    public AudioClip loadingClip;

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

    public new Rigidbody2D rigidbody2D {get; private set;}

    private OnHandScore onHandScore;

    private Segment cartPusher;

    public float cartOffset;

    public Vector3 lastPosition;

    private void Awake() {
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
    }

    public void UpdateSnake()
    {
        for(int i = 0; i < size; i++)
        {
            segments[i].lastPosition = segments[i].transform.position;
            segments[i].lastRotation = segments[i].transform.rotation;

            // segments[i].transform.position = i != 0 
            //     ? segments[i - 1].lastPosition
            //     : transform.position;
            // segments[i].transform.rotation = i != 0
            //     ? segments[i - 1].lastRotation
            //     : transform.rotation;
        }

        cartPusher.lastPosition = cartPusher.transform.position;
        cartPusher.lastRotation = cartPusher.transform.rotation;
        // cartPusher.transform.position = size == 0 ? transform.position : segments[size - 1].lastPosition;
        // cartPusher.transform.rotation = size == 0 ? transform.rotation : segments[size - 1].lastRotation;
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

        return segCount;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {    
        if(other.gameObject.TryGetComponent<ICollectable>(out ICollectable collectable))
        {
            // Carts must be picked up from roughly behind
            if(Vector3.Dot(transform.up, other.transform.up) >= 1f/3f)
            {
                AddSegment(collectable.growthAmount);
                audioSource.PlayOneShot(loadingClip);

                // Screw the GC
                Destroy(other.gameObject);
            }
        }
        else if(other.gameObject.TryGetComponent<LoseGameOnCollision>(out LoseGameOnCollision loseGame))
        {
            Debug.Log($"Ran into {other.name}");
            Crashed();
        }
    }

    public void Crashed()
    {
        cartRollingSource.Stop();
        speed = 0;
        rigidbody2D.velocity = Vector3.zero;
        gm.LoseGame();
    }
}