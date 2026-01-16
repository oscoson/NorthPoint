using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Augmenta;

public class BallSpawner : MonoBehaviour
{
    [Header("Augmenta Manager Reference")]
    [SerializeField] private AugmentaManager augmentaManager;

    [Header("Augmenta Presence Variable(s)")]
    [SerializeField] private float minimumPresence = 1f;
    private Dictionary<int, Coroutine> presenceTimers = new Dictionary<int, Coroutine>();

    [Header("Containment Box Population")]
    public int redBallsSpawned = 0;
    public int yellowBallsSpawned = 0;
    public int greenBallsSpawned = 0;
    public int purpleBallsSpawned = 0;
    public int redBallsCaptured = 0;
    public int yellowBallsCaptured = 0;
    public int greenBallsCaptured = 0;
    public int purpleBallsCaptured = 0;

    [Header("Ball Spawn Settings")]
    public bool fastBallSpawned;
    [SerializeField] private BallPalette ballPalleteAsset;
    [SerializeField] private int ballsPerPerson = 4;
    [SerializeField] private int maxBallsInRoom = 20;
    [SerializeField] private int ballsLeftToSpawn;
    [SerializeField] private int ballCount;
    [SerializeField] private bool zeroFlag;
    [SerializeField] private bool isSpawning;
    [SerializeField] private List<string> availableOrbColors = new List<string> { "Red", "Green", "Purple", "Yellow" };
    private static BallPalette.Entry[] ballPalette;

    [Header("Spawn Area Settings")]
    [SerializeField] private Vector2 xRange = new Vector2(-13.9f, 13.9f);

    [SerializeField] private float xSpawnRange;
    [SerializeField] private float zSpawnRange;

    private AudioVisualizer audioVisualizer;
    private float deliveryRadius = 5f;
    private GameManager gameManager;

    private float sinkBoundary;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        ballPalette = ballPalleteAsset.GetEntries();
        sinkBoundary = Mathf.Abs(xRange.x) - deliveryRadius;
        ballsLeftToSpawn = maxBallsInRoom;
        gameManager = FindAnyObjectByType<GameManager>();
        audioVisualizer = FindAnyObjectByType<AudioVisualizer>();
    }

    void Start()
    {
        if (augmentaManager != null)
        {
            augmentaManager.augmentaObjectEnter += OnAugmentaObjectEnter;
            augmentaManager.augmentaObjectLeave += OnAugmentaObjectLeave;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ballCount <= 0 && !zeroFlag && ballsLeftToSpawn > 0)
        {
            zeroFlag = true;
            StartCoroutine(NewPlayerBallSpawn());
        }
        if (ballCount > 0)
        {
            zeroFlag = false;
        }
        if( ballsLeftToSpawn <= 0 && ballCount <= 0)
        {
            audioVisualizer.PlayVOClip(audioVisualizer.gameCompleteClips);
            gameManager.SetGameState("END");
            ResetTerminalGoals();
        }
    }

    private void SpawnEnergyBall()
    {
        if (availableOrbColors.Count == 0)
        {
            return; // All colors have reached max capacity
        }

        int ballSeed = 0;
        string ballColor = availableOrbColors[Random.Range(0, availableOrbColors.Count)];
        bool found = false;
        switch (ballColor)
        {
            case "Purple":
                if (purpleBallsSpawned < 5)
                {
                    purpleBallsSpawned++;
                    CheckMaxColourCapacity("Purple");
                    found = true;
                    ballSeed = 0;
                }
                break;
            case "Green":
                if (greenBallsSpawned < 5)
                {
                    greenBallsSpawned++;
                    CheckMaxColourCapacity("Green");
                    found = true;
                    ballSeed = 1;
                }
                break;
            case "Yellow":
                if (yellowBallsSpawned < 5)
                {
                    yellowBallsSpawned++;
                    CheckMaxColourCapacity("Yellow");
                    found = true;
                    ballSeed = 2;
                }
                break;
            case "Red":
                if (redBallsSpawned < 5)
                {
                    redBallsSpawned++;
                    CheckMaxColourCapacity("Red");
                    found = true;
                    ballSeed = 3;
                }
                break;
            default:
                break;
        }

        if (found)
        {
            ballCount++; // Confirmed new ball spawn
            ballsLeftToSpawn--;
            Vector3 pos = new(GetRandomXPos(), -0.25f, GetRandomZPos());
            GameObject newEnergyBall = Instantiate(ballPalette[ballSeed].prefab, pos, ballPalette[ballSeed].prefab.transform.rotation);
            newEnergyBall.GetComponent<EnergyBall>().Initialise(newEnergyBall, ballSeed, ballPalette[ballSeed].material.color, ballPalette[ballSeed].captureSprite, ballPalette[ballSeed].spawnFX, this, ballColor);
        }
    }

    public void DestroyBall(EnergyBall energyBall)
    {
        ballCount--;
        Destroy(energyBall.gameObject);
    }

    public void OnAugmentaObjectEnter(AugmentaObject obj, AugmentaDataType dataType)
    {
        int id = obj.id; // Assume unique per person

        if (!presenceTimers.ContainsKey(id))
        {
            Coroutine c = StartCoroutine(ConfirmPresenceAfterDelay(obj, id));
            presenceTimers[id] = c;
        }
    }

    public void OnAugmentaObjectLeave(AugmentaObject obj, AugmentaDataType dataType)
    {
        int id = obj.id;
        if (obj.GetComponentInChildren<EnergyBall>() != null)
        {
            RestorePopulationCount(obj.GetComponentInChildren<EnergyBall>().ballColorName);
            DestroyBall(obj.GetComponentInChildren<EnergyBall>());
        }

    }

    private IEnumerator ConfirmPresenceAfterDelay(AugmentaObject obj, int id)
    {
        yield return new WaitForSeconds(2f);

        // If we're still tracking the object after 2 seconds, they didn't leave
        if (presenceTimers.ContainsKey(id))
        {
            presenceTimers.Remove(id);
            if( ballsLeftToSpawn > 0)
            {
                StartCoroutine(NewPlayerBallSpawn());                   
            }
        }
    }

    public IEnumerator DelayedBallSpawn(float manDelay)
    {
        yield return new WaitForSeconds(minimumPresence);
        if (ballsLeftToSpawn > 0)
        {
            yield return new WaitForSeconds(Random.Range(1f, 5f)); // time between ball spawns
            SpawnEnergyBall();
        }
    }

    public IEnumerator NewPlayerBallSpawn()
    {
        for (int i = 0; i < ballsPerPerson; i++)
        {
            if (ballsLeftToSpawn > 0)
            {
                StartCoroutine(DelayedBallSpawn(0f));
            }
            else
            {
                yield return null;
            }
        }
    }

    private void ResetTerminalGoals()
    {
        fastBallSpawned = false;
        redBallsSpawned = 0;
        yellowBallsSpawned = 0;
        greenBallsSpawned = 0;
        purpleBallsSpawned = 0;
        redBallsCaptured = 0;
        yellowBallsCaptured = 0;
        greenBallsCaptured = 0;
        purpleBallsCaptured = 0;
        ballCount = 0;

        ballsLeftToSpawn = maxBallsInRoom;
        availableOrbColors = new List<string> { "Red", "Green", "Purple", "Yellow" };

        gameManager.DeactivateAllLights();
    }

    private void CheckMaxColourCapacity(string color)
    {
        switch(color)
        {
            case "Purple":
                if (purpleBallsSpawned >= 5)
                {
                    availableOrbColors.Remove("Purple");
                }
                break;
            case "Green":
                if (greenBallsSpawned >= 5)
                {
                    availableOrbColors.Remove("Green");
                }
                break;
            case "Yellow":
                if (yellowBallsSpawned >= 5)
                {
                    availableOrbColors.Remove("Yellow");
                }
                break;
            case "Red":
                if (redBallsSpawned >= 5)
                {
                    availableOrbColors.Remove("Red");
                }
                break;
            default:
                break;
        }
    }

    public void SetLight(string colorName)
    {
        switch(colorName)
        {
            case "Green":
                greenBallsCaptured++;
                gameManager.ActivateLights("Green", greenBallsCaptured - 1);
                break;
            case "Red":
                redBallsCaptured++;
                gameManager.ActivateLights("Red", redBallsCaptured - 1);
                break;
            case "Yellow":
                yellowBallsCaptured++;
                gameManager.ActivateLights("Yellow", yellowBallsCaptured - 1);
                break;
            case "Purple":
                purpleBallsCaptured++;
                gameManager.ActivateLights("Purple", purpleBallsCaptured - 1);
                break;
        }
    }

    public void RestorePopulationCount(string colorName)
    {
        if (!availableOrbColors.Contains(colorName))
        {
            availableOrbColors.Add(colorName);
        }
        switch(colorName)
        {
            case "Green":
                greenBallsSpawned--;
                break;
            case "Red":
                redBallsSpawned--;
                break;
            case "Yellow":
                yellowBallsSpawned--;
                break;
            case "Purple":
                purpleBallsSpawned--;
                break;
        }
        ballsLeftToSpawn++;
    }

    public float GetSinkBoundary()
    {
        return sinkBoundary;
    }

    public int GetBalls()
    {
        return ballCount;
    }

    public float GetMaxBallsInRoom()
    {
        return maxBallsInRoom;
    }

    public float GetBallsLeftToSpawn()
    {
        return ballsLeftToSpawn;
    }

    public float GetRandomXPos()
    {
        return Random.Range(-xSpawnRange, xSpawnRange);
    }

    public float GetRandomZPos()
    {
        return Random.Range(-zSpawnRange, zSpawnRange);
    }

    // Destructor
    void OnDestroy()
    {
        if (augmentaManager != null)
        {
            augmentaManager.augmentaObjectEnter -= OnAugmentaObjectEnter;
            augmentaManager.augmentaObjectLeave -= OnAugmentaObjectLeave;
        }
    }
}
