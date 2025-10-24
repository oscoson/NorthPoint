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


    [Header("Ball Spawn Settings")]
    [SerializeField] private BallPalette ballPalleteAsset;
    [SerializeField] private int ballsPerPerson = 4;
    [SerializeField] private int maxBallsInRoom = 20;
    [SerializeField] private int ballCount;
    [SerializeField] private bool zeroFlag;
    private static BallPalette.Entry[] ballPalette;

    [Header("Spawn Area Settings")]
    [SerializeField] private Vector2 xRange;  // = new Vector2(-13.9f, 13.9f);
    [SerializeField] private Vector2 zRange;  // = new Vector2(-13.9f, 13.9f);

    [Header("Non-related Asset References")]
    [SerializeField] private GameObject spawnEffect;
    [SerializeField] private AudioManager audioManager;

    private float sinkBoundary;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        ballPalette = ballPalleteAsset.GetEntries();
        sinkBoundary = Mathf.Abs(xRange.x) - 3.5f;
        audioManager = FindAnyObjectByType<AudioManager>();
    }
    
    void Start()
    {
        if(augmentaManager != null)
        {
            augmentaManager.augmentaObjectEnter += OnAugmentaObjectEnter;
            augmentaManager.augmentaObjectLeave += OnAugmentaObjectLeave;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ballCount <= 0 && !zeroFlag)
        {
            zeroFlag = true;
            // Start Coroutine for Spawning Energy Ball
        }
        if (ballCount > 0)
        {
            zeroFlag = false;
        }
    }
    // TODO: Implement Spawn Function (think over whether we want prefabs instead of what Kevin wrote)
    // private EnergyBall SpawnEnergyBall()
    // {
        
    // }
    
    private void DestroyBall(EnergyBall energyBall)
    {
        ballCount--;
        Destroy(energyBall.gameObject);
    }

    public void OnAugmentaObjectEnter(AugmentaObject obj, Augmenta.AugmentaDataType dataType)
    {
        int id = obj.id; // Assume unique per person
        // Debug.Log($"Object {id} is entering");
        
        if (!presenceTimers.ContainsKey(id))
        {
            Coroutine c = StartCoroutine(ConfirmPresenceAfterDelay(obj, id));
            presenceTimers[id] = c;
        }
    }

    public void OnAugmentaObjectLeave(AugmentaObject obj, Augmenta.AugmentaDataType dataType)
    {
        int id = obj.id;
        // Debug.Log($"Object {id} is leaving");
        if (obj.GetComponentInChildren<Ghost>() != null)
        {
            // Destroy Ghost on Leave
        }
        // can put else statement here if we want ghosts to despawn when player leaves
        // Cancel ghost spawn if they left early
        // if (presenceTimers.TryGetValue(id, out Coroutine c))
        // {
        //     StopCoroutine(c);
        //     presenceTimers.Remove(id);
        //     // Debug.Log($"Cancelled spawn for object {id} due to early exit");
        // }

    }

    private IEnumerator ConfirmPresenceAfterDelay(AugmentaObject obj, int id)
    {
        yield return new WaitForSeconds(2f);

        // If we're still tracking the object after 2.5 seconds, they didn't leave
        if (presenceTimers.ContainsKey(id))
        {
            // Debug.Log($"Object {id} confirmed present after {minimumPresence} seconds");
            presenceTimers.Remove(id);
            // StartCoroutine(NewPlayerGhostSpawn());
        }
    }

    public IEnumerator DelayedBallSpawn()
    {
        //TODO Implement Delayed Ball Spawn
        yield return 0f;
    }

    public IEnumerator NewPlayerBallSpawn()
    {
        // TODO Implement New Set of Energy Balls to Spawn
        yield return 0f;
    }
    
    public float GetSinkBoundary()
    {
        return sinkBoundary;
    }
    
    public int GetGhosts()
    {
        return ballCount;
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
