using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartReturn : MonoBehaviour
{
    [SerializeField] private SpriteRenderer returnedCartPrefab;
    [SerializeField] private CarSpawner carSpawner;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Transform return1;
    [SerializeField] private Transform return2;
    public AudioClip separateNoise;
    public Stack<GameObject> returnedCarts = new Stack<GameObject>();
    public float cartDistance;
    private BankedScore bankedScore;

    public float powerupSecondsPerCart;

    [SerializeField] private Transform startLoc;
    private void Awake() => bankedScore = GameObject.FindObjectOfType<BankedScore>();

    private void Start() => AddCarts(10);

    private void OnTriggerEnter2D(Collider2D other)
    {
        SnakeData snake = other.GetComponent<SnakeData>();
        if(snake == null)
            return;

        // snake.transform.up *= -1;
        snake.transform.SetPositionAndRotation(startLoc.position, startLoc.rotation);

        int segCount = snake.TakeAllSegments();
        snake.Powerup(segCount * powerupSecondsPerCart);
        audioSource.Play();

        if(segCount == 0)
            return;

        AddCarts(segCount, true);
    }

    public void AddCarts(int cartCount, bool addPoints = false)
    {
        if(addPoints)
            bankedScore.Inc(cartCount);

        for(int i = 0; i < cartCount; i++)
        {
            GameObject lastCart = null;
            if(returnedCarts.Count > 1)
                lastCart = returnedCarts.Peek();

            Transform toUse = returnedCarts.Count % 2 == 0 ? return1 : return2;

            SpriteRenderer newCart = Instantiate(
                original: returnedCartPrefab,
                position: lastCart == null 
                    ? toUse.position + (-toUse.right * cartDistance) 
                    : toUse.position + (-toUse.right * cartDistance * returnedCarts.Count),
                rotation: Quaternion.Euler(0,0, -90)
            );
            newCart.sortingOrder = returnedCarts.Count * -1;
            returnedCarts.Push(newCart.gameObject);
        }

        // Creates difficulty when returning carts
        carSpawner.SpeedUp();
    }

    public void TakeCart()
    {
        if(returnedCarts.Count > 0)
        {
            GameObject cart = returnedCarts.Pop();
            audioSource.PlayOneShot(separateNoise);
            // Screw the GC
            Destroy(cart);
        }
    }
}