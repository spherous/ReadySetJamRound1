using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // [SerializeField] private AudioSource audioSource;

    // public List<AudioClip> audioClips = new List<AudioClip>();

    // public UI ui {get; private set;}
    public Camera activeCamera {get; private set;}
    public SnakeData snake {get; private set;}
    public CollectableSpawner collectableSpawner {get; private set;}

    [SerializeField] private GameObject snakePrefab;
    // [SerializeField] private GameObject uiPrefab;
    [SerializeField] private GameObject collectableSpawnerPrefab;
    
    private float lastIncraseTime;

    private float horizontalInput;

    private void Awake()
    {
        Instance = this;
        activeCamera = Camera.main;
        snake = Instantiate(snakePrefab).GetComponent<SnakeData>();
        // ui = Instantiate(uiPrefab).GetComponent<UI>();
        collectableSpawner = Instantiate(collectableSpawnerPrefab).GetComponent<CollectableSpawner>();
        Time.timeScale = 1;
        // audioSource.clip = audioClips[0];
        // audioSource.Play();
    }

    private void Update()
    {
        if(Time.timeSinceLevelLoad - lastIncraseTime > 5)
        {
            snake.speed *= 1.1f;
            snake.turnSpeed *= 1.02f;
            lastIncraseTime = Time.timeSinceLevelLoad;
        }
    }

    public void Input(CallbackContext context)
    {
        horizontalInput = context.ReadValue<float>();
    }

    private void FixedUpdate()
    {
        // Process player input
        snake.transform.RotateAround(snake.transform.position, snake.transform.forward, -horizontalInput * snake.turnSpeed);
        // Move snake forward
        MoveSnakeForward();
    }

    private void MoveSnakeForward()
    {
        snake.rigidbody2D.velocity = snake.transform.up * snake.speed;            
        snake.UpdateSnake();
    }

    public void LoseGame()
    {
        Debug.Log("Lose Game");
        Time.timeScale = 0;
        // ui?.EndGame();
        // audioSource.clip = audioClips[1];
        // audioSource.Play();
    }
}