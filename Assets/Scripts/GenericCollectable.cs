using UnityEngine;

public class GenericCollectable : MonoBehaviour, ICollectable
{
    public int growthAmount {get{return GrowthAmount;} set{}}
    [SerializeField] private int GrowthAmount;

    // private float roationSpeed;

    private float timeTillDestroy = 10f;
    private float destoryAtTime;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        destoryAtTime = Time.timeSinceLevelLoad + timeTillDestroy;
        // roationSpeed = Random.Range(-45, 45);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(Time.timeSinceLevelLoad >= destoryAtTime)
            Destroy(gameObject);
        else
        {
            Color color = spriteRenderer.color;
            color.a = Mathf.Lerp(0f, 1f, (destoryAtTime - Time.timeSinceLevelLoad) / timeTillDestroy);
            spriteRenderer.color = color;
        }
    }
    
    // private void FixedUpdate() =>
    //     transform.RotateAround(transform.position, transform.forward, roationSpeed * Time.fixedDeltaTime);
}