using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class GameManager : SerializedMonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] public FlashyBox box;
    [SerializeField] private DayNightCycle dayNightCycle;

    // [SerializeField] private AudioSource audioSource;

    // public List<AudioClip> audioClips = new List<AudioClip>();
    [SerializeField] private GameObject endGameUI;
    [SerializeField] private CarSpawner carSpawner;
    public SnakeData snake {get; private set;}
    public CollectableSpawner collectableSpawner {get; private set;}

    [SerializeField] private GameObject snakePrefab;
    [SerializeField] private GameObject collectableSpawnerPrefab;
    [SerializeField] private BankedScore bankedScore;
    
    // private float lastIncraseTime;

    [OdinSerialize] public List<List<LightLoc>> lightLocs = new List<List<LightLoc>>();

    private float horizontalInput;
    public Transform lightContainer;
    [SerializeField] private GameObject streetLightPrefab;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1;
        Application.targetFrameRate = 60;
        SpawnLights();
    }

    private void SpawnLights()
    {
        List<LightLoc> locs = lightLocs.First();
        foreach(LightLoc loc in locs)
        {
            Instantiate(streetLightPrefab, loc.pos, Quaternion.Euler(0, 0, loc.zrot), lightContainer);
        }
    }

    public void Input(CallbackContext context) => horizontalInput = context.ReadValue<float>();

    private void FixedUpdate()
    {
        if(snake == null)
            return;
        // Game is over, don't allow input
        if(endGameUI.activeSelf)
            return;

        // Process player input
        snake.transform.RotateAround(snake.transform.position, snake.transform.forward, -horizontalInput * snake.turnSpeed);
        snake.rigidbody2D.rotation = snake.transform.rotation.eulerAngles.z;
        snake.rigidbody2D.velocity = snake.transform.up * snake.speed;
        snake.UpdateSnake();
    }

    public void StartGame()
    {
        collectableSpawner = Instantiate(collectableSpawnerPrefab).GetComponent<CollectableSpawner>();
        snake = Instantiate(snakePrefab).GetComponent<SnakeData>();
        snake.pickedUpCart += PickedUpCarts;
        snake.empiedCarts += EmptiedCarts;
        carSpawner.gameObject.SetActive(true);
        dayNightCycle.cycle = true;
    }

    private void EmptiedCarts()
    {
        box.flashing = false;
    }

    private void PickedUpCarts()
    {
        box.flashing = true;
    }

    public void LoseGame()
    {
        Debug.Log("Lose Game");

        carSpawner.gameObject.SetActive(false);
        collectableSpawner.gameObject.SetActive(false);

        int previousHighscore = PlayerPrefs.GetInt("Highscore", 0);
        if(bankedScore.score > previousHighscore)
            PlayerPrefs.SetInt("Highscore", bankedScore.score);

        endGameUI.SetActive(true);
    }

    private void OnDestroy() {
        if(snake != null)
        {
            snake.pickedUpCart -= PickedUpCarts;
            snake.empiedCarts -= EmptiedCarts;    
        }
    }
}