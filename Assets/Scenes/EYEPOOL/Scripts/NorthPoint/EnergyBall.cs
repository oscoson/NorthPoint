using UnityEngine;

public class EnergyBall : MonoBehaviour
{
    // Read-only towards other scripts
    public GameObject ballObject { get; private set; }
    public int targetSinkID { get; private set; }
    public Color ballColor { get; private set; }
    public string ballColorName { get; private set; }
    public Sprite captureSprite { get; private set; }
    public GameObject captureEffect { get; private set; }

    public enum BallState
    {
        Hovering,
        Planning,
        Moving,
        Attached
    }

    public BallState state = BallState.Hovering;
    public float dropoffDelay { get; private set; } = 1.0f;
    private float dropoffTimer = 0f;
    private float hoverCountDown = 0.1f;

    private AugmentaPickup personAttached;
    [SerializeField] private float speed;
    [SerializeField] private Vector3 newPos;
    [SerializeField] private BallSpawner spawner;
    [SerializeField] private GameObject unattachedModel;
    [SerializeField] private GameObject fastUnattachedModel;
    [SerializeField] private GameObject attachedModel;
    private bool isFastBall = false;
    private AudioManager audioManager;
    private AudioVisualizer audioVisualizer;
    public void Initialise(GameObject _ballObject, int _targetSinkID, Color _ballColor, Sprite _captureSprite, GameObject _captureEffect, BallSpawner ballSpawner, string ballColorString)
    {
        ballObject = _ballObject;
        targetSinkID = _targetSinkID;
        ballColor = _ballColor;
        captureSprite = _captureSprite;
        captureEffect = _captureEffect;
        spawner = ballSpawner;
        ballColorName = ballColorString;

        audioManager = FindAnyObjectByType<AudioManager>();
        audioVisualizer = FindAnyObjectByType<AudioVisualizer>();
        
        float rate = Random.Range(0.05f, 1f);
        // Gradually increase chance of fast ball spawning as more balls are in the room
        if (!spawner.fastBallSpawned && (spawner.GetBallsLeftToSpawn() / spawner.GetMaxBallsInRoom()) <= rate)
        {
            unattachedModel.SetActive(false);
            fastUnattachedModel.SetActive(true);
            spawner.fastBallSpawned = true;
            speed = Random.Range(8, 10);
            isFastBall = true;
        }
        else
        {
            speed = Random.Range(2, 5);
        }

        PlayRandomSpawnSound();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case BallState.Hovering:
                hoverCountDown -= Time.deltaTime;
                if (hoverCountDown <= 0f)
                {
                    state = BallState.Planning;
                }
                break;
            case BallState.Planning:
                newPos = new Vector3(spawner.GetRandomXPos(), -0.25f, spawner.GetRandomZPos());
                state = BallState.Moving;
                break;
            case BallState.Moving:
                // Handled In FixedUpdate
                break;
            case BallState.Attached:
                int sinkHere = Util.GetSinkID(transform.position, spawner.GetSinkBoundary());
                if (sinkHere != targetSinkID) dropoffTimer = 0f;

                dropoffTimer += Time.deltaTime;
                if (dropoffTimer >= dropoffDelay)
                {
                    Detach(true);
                }
                break;
        }
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case BallState.Hovering:
                break;
            case BallState.Planning:
                break;
            case BallState.Moving:
                if (newPos != transform.position)
                {
                    Vector3 pos = Vector3.MoveTowards(transform.position, newPos, speed * Time.fixedDeltaTime);
                    gameObject.GetComponent<Rigidbody>().MovePosition(pos);
                }
                else
                {
                    hoverCountDown = 0.1f;
                    state = BallState.Hovering;
                }
                break;
            case BallState.Attached:
                break;
        }
    }

    public void AttachTo(Transform parent)
    {
        if (state == BallState.Attached) return;

        state = BallState.Attached;

        if (isFastBall)
        {
            fastUnattachedModel.SetActive(false);
            attachedModel.SetActive(true);
        }
        else
        {
            unattachedModel.SetActive(false);
            attachedModel.SetActive(true);
        }

        transform.SetParent(parent, true);
        transform.localPosition = Vector3.zero;
        personAttached = parent.GetComponent<AugmentaPickup>();
        personAttached.AttachBallRing(this);
        PlayPickupSound();
        // Instantiate(splat, transform.position + new Vector3(-0.0125f, 0f, 0f), Quaternion.Euler(90, -90, 0)); // run the splat with offset
        // if (personAttached == null)
        // {
        //     // Debug.Log("unable to pick up person properly");
        // }
    }

    public void Detach(bool reachedCorrectSink)
    {
        if (state != BallState.Attached) return;

        if (reachedCorrectSink)
        {
            state = BallState.Hovering;
            transform.SetParent(null, true);
            personAttached.DropBall();
            spawner.SetLight(ballColorName);
            PlayDropOffSound(targetSinkID);
            spawner.DestroyBall(this);
        }
    }

    private void PlayPickupSound()
    {
        int randInt = Random.Range(1, 3);
        switch (randInt)
        {
            case 1:
                audioManager.Play("ORB_LOCKON_1");
                break;
            case 2:
                audioManager.Play("ORB_LOCKON_2");
                break;
            default:
                audioManager.Play("ORB_LOCKON_1");
                break;
        }
        audioVisualizer.PlayVOClip(audioVisualizer.energyBallCaptureClips);
    }
    private void PlayDropOffSound(int portalID)
    {
        switch (portalID)
        {
            case 0:
                audioManager.Play("DROP_ORB_1");
                break;
            case 1:
                audioManager.Play("DROP_ORB_2");
                break;
            case 2:
                audioManager.Play("DROP_ORB_3");
                break;
            case 3:
                audioManager.Play("DROP_ORB_4");
                break;
            default:
                audioManager.Play("DROP_ORB_1");
                break;
        }
        if(spawner.GetBallsLeftToSpawn() > 0 && spawner.GetBalls() > 1)
        {
            audioVisualizer.PlayVOClip(audioVisualizer.energyBallDropClips);
            return;
        }
    }


    private void PlayRandomSpawnSound()
    {
        int randInt = Random.Range(1, 7);

        switch (randInt)
        {
            case 1:
                audioManager.Play("ORB_APPEAR_1");
                break;
            case 2:
                audioManager.Play("ORB_APPEAR_2");
                break;
            case 3:
                audioManager.Play("ORB_APPEAR_3");
                break;
            case 4:
                audioManager.Play("ORB_APPEAR_4");
                break;
            case 5: 
                audioManager.Play("ORB_APPEAR_5");
                break;
            case 6:
                audioManager.Play("ORB_APPEAR_6");
                break;
            default:
                audioManager.Play("ORB_APPEAR_1");
                break;
        }
    }
}
