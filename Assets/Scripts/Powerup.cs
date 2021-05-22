using UnityEngine;

public class Powerup : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent<SnakeData>(out SnakeData snake))
        {
            // snake.Powerup();
            Destroy(gameObject);
        }
    }
}