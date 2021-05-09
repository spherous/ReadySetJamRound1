using UnityEngine;

public class HonkDetector : MonoBehaviour
{
    public Car car;
    [SerializeField] private CircleCollider2D col2D;
    private void Awake() => col2D.radius *= Random.Range(1f, 2f);
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
    } 
}