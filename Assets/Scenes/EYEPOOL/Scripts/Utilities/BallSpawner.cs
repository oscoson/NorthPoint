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

    [Header("Contaiment Box Population")]
    public int redBallCount = 0;
    public int yellowBallCount = 0;
    public int greenBallCount = 0;
    public int purpleBallCount = 0;

    [Header("Ball Spawn Settings")]
    [SerializeField] private BallPalette ballPalleteAsset;
    [SerializeField] private int ballsPerPerson = 4;
    [SerializeField] private int maxBallsInRoom = 20;
    [SerializeField] private int ballsLeftToSpawn;
    [SerializeField] private int ballCount;
    [SerializeField] private bool zeroFlag;
    private static BallPalette.Entry[] ballPalette;

    [Header("Spawn Area Settings")]
    [SerializeField] private Vector2 xRange = new Vector2(-13.9f, 13.9f);

    [SerializeField] private float xSpawnRange;
    [SerializeField] private float zSpawnRange;
    // [SerializeField] private Vector2 yRange = new Vector2(-13.9f, 13.9f);

    private GameObject spawnEffect;
    private AudioManager audioManager;

    private float sinkBoundary;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        ballPalette = ballPalleteAsset.GetEntries();
        sinkBoundary = Mathf.Abs(xRange.x) - 5f;
        ballsLeftToSpawn = maxBallsInRoom;
        audioManager = FindAnyObjectByType<AudioManager>();
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
        // if( ballsLeftToSpawn <= 0 && ballCount <= 0)
        // {
        //     ResetTerminalGoals();
        // }
    }

    private void SpawnEnergyBall()
    {
        int ballSeed = Random.Range(0, ballPalette.Length);
        bool found = false;
        switch (ballSeed)
        {
            case 0:
                if (purpleBallCount < 5)
                {
                    purpleBallCount++;
                    found = true;
                }
                break;
            case 1:
                if (greenBallCount < 5)
                {
                    greenBallCount++;
                    found = true;
                }
                break;
            case 2:
                if (yellowBallCount < 5)
                {
                    yellowBallCount++;
                    found = true;
                }
                break;
            case 3:
                if (redBallCount < 5)
                {
                    redBallCount++;
                    found = true;
                }
                break;
            default:
                if(ballsLeftToSpawn > 0 && ballCount < maxBallsInRoom)
                {
                    SpawnEnergyBall();   
                }
                break;
        }

        if(found)
        {
            ballCount++; // Confirmed new ball spawn
            ballsLeftToSpawn--;
            Vector3 pos = new(GetRandomXPos(), -0.25f, GetRandomZPos());
            GameObject newEnergyBall = Instantiate(ballPalette[ballSeed].prefab, pos, ballPalette[ballSeed].prefab.transform.rotation);
            newEnergyBall.AddComponent<EnergyBall>();
            newEnergyBall.GetComponent<EnergyBall>().Initialise(newEnergyBall, ballSeed, ballPalette[ballSeed].material.color, ballPalette[ballSeed].captureSprite, ballPalette[ballSeed].spawnFX, this);   
        }


        // Audio Manager Play
        // Instantiate Spawn Effect
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
            Debug.Log("DELETING");
            DestroyBall(obj.GetComponentInChildren<EnergyBall>());
        }
        // can put else statement here if we want Balls to despawn when player leaves
        // Cancel Ball spawn if they left early
        // if (presenceTimers.TryGetValue(id, out Coroutine c))
        // {
        //     StopCoroutine(c);
        //     presenceTimers.Remove(id);
        //     // Debug.Log($"Cancelled spawn for object {id} due to early exit");
        // }

    }

    private IEnumerator ConfirmPresenceAfterDelay(AugmentaObject obj, int id)
    {
        yield return new WaitForSeconds(5f);

        // If we're still tracking the object after 5 seconds, they didn't leave
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
        redBallCount = 0;
        yellowBallCount = 0;
        greenBallCount = 0;
        purpleBallCount = 0;
        ballCount = 0;
        ballsLeftToSpawn = maxBallsInRoom;
    }

    public float GetSinkBoundary()
    {
        return sinkBoundary;
    }

    public int GetBalls()
    {
        return ballCount;
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
