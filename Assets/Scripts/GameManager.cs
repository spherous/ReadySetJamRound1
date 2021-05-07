using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // [SerializeField] private AudioSource audioSource;

    // public List<AudioClip> audioClips = new List<AudioClip>();
    [SerializeField] private GameObject endGameUI;
    [SerializeField] private CarSpawner carSpawner;
    public SnakeData snake {get; private set;}
    public CollectableSpawner collectableSpawner {get; private set;}

    [SerializeField] private GameObject snakePrefab;
    [SerializeField] private GameObject collectableSpawnerPrefab;
    
    // private float lastIncraseTime;

    private float horizontalInput;

    private void Awake()
    {
        Instance = this;
        snake = Instantiate(snakePrefab).GetComponent<SnakeData>();
        collectableSpawner = Instantiate(collectableSpawnerPrefab).GetComponent<CollectableSpawner>();
        Time.timeScale = 1;
        Application.targetFrameRate = 60;
    }

    public void Input(CallbackContext context) => horizontalInput = context.ReadValue<float>();

    private void FixedUpdate()
    {
        // Game is over, don't allow input
        if(endGameUI.activeSelf)
            return;

        // Process player input
        snake.transform.RotateAround(snake.transform.position, snake.transform.forward, -horizontalInput * snake.turnSpeed);
        snake.rigidbody2D.rotation = snake.transform.rotation.eulerAngles.z;
        snake.rigidbody2D.velocity = snake.transform.up * snake.speed;
        snake.UpdateSnake();
    }

    public void LoseGame()
    {
        Debug.Log("Lose Game");
        carSpawner.gameObject.SetActive(false);
        collectableSpawner.gameObject.SetActive(false);
        endGameUI.SetActive(true);
    }
}