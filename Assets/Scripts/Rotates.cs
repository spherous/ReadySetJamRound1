using UnityEngine;

public class Rotates : MonoBehaviour
{
    public float rotationSpeed;
    void Update() => transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
}