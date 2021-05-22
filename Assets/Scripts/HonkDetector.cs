using UnityEngine;

public class HonkDetector : MonoBehaviour
{
    public Car car;
    [SerializeField] private CircleCollider2D col2D;
    SnakeData snake;
    private void Awake() 
    {
        col2D.radius *= Random.Range(1f, 2f);
        snake = GameObject.FindObjectOfType<SnakeData>();
    } 
    private void Update() {
        if(car == null)
            return;
            
        if(transform.position != car.transform.position)
            transform.position = car.transform.position;
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        Segment segment = other.gameObject.GetComponent<Segment>();
        if(segment == null)
            return;
        
        car?.Honk();
        car?.SlowDown();
        if(snake.poweredUp)
            car?.FleeFromSnake(snake.transform.position);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Segment segment = other.gameObject.GetComponent<Segment>();
        if(segment == null)
            return;

        car?.SpeedUp();
    }
}