using System.Collections.Generic;
using UnityEngine;

public class SnakeData : MonoBehaviour
{
    GameManager gm => GameManager.Instance;

    // [SerializeField] private AudioSource audioSource;

    public delegate void OnSizeChanged(int size);
    public OnSizeChanged onSizeChanged;

    public GameObject segmentPrefab;
    [SerializeField] public List<Segment> segments {get; private set;} = new List<Segment>();

    public int size => segments.Count;
    
    public float speed = 1f;
    public float turnSpeed = 3f;

    public new Rigidbody2D rigidbody2D {get; private set;}

    public float offset;

    private void Awake() {
        rigidbody2D = GetComponent<Rigidbody2D>();
        AddSegment();
    }

    public void AddSegment(int amount = 1)
    {
        Segment newSegment = Instantiate(
            original: segmentPrefab,
            position: size < 2 ? transform.position : segments[size - 1].lastPosition,
            rotation: size < 2 ? transform.rotation : segments[size - 1].lastRotation
        ).GetComponent<Segment>();

        SpriteRenderer renderer = newSegment.GetComponent<SpriteRenderer>();
        renderer.sortingOrder = size * -1;

        segments.Add(newSegment);
        // audioSource?.Play();
        onSizeChanged?.Invoke(segments.Count);
    }

    public void UpdateSnake()
    {
        for(int i = 0; i < size; i++)
        {
            segments[i].lastPosition = segments[i].transform.position;
            segments[i].lastRotation = segments[i].transform.rotation;

            segments[i].transform.position = i != 0 
                ? segments[i - 1].lastPosition
                : transform.position;
            segments[i].transform.rotation = i != 0
                ? segments[i - 1].lastRotation
                : transform.rotation;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {    
        if(other.gameObject.TryGetComponent<ICollectable>(out ICollectable collectable))
        {
            // AddSegment(collectable.growthAmount);    
            AddSegment();    
            Destroy(other.gameObject);
        }
        else if(other.gameObject.TryGetComponent<LoseGameOnCollision>(out LoseGameOnCollision loseGame))
        {
            gm.LoseGame();
        }
    }
}