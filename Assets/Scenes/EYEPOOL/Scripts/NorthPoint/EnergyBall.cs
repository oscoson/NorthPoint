using UnityEngine;

public class EnergyBall : MonoBehaviour
{
    // Read-only towards other scripts
    public GameObject ballObject { get; private set; }
    public int targetSinkID { get; private set; }
    public Color ballColor { get; private set; }
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

    private float size;

    private AugmentaPickup personAttached;
    [SerializeField] private float speed;
    [SerializeField] private Vector3 newPos;
    [SerializeField] private BallSpawner spawner;
    private AudioManager audioManager;
    private Animator animator;

    public void Initialise(GameObject _ballObject, int _targetSinkID, Color _ballColor, Sprite _captureSprite, GameObject _captureEffect, BallSpawner ballSpawner)
    {
        ballObject = _ballObject;
        targetSinkID = _targetSinkID;
        ballColor = _ballColor;
        captureSprite = _captureSprite;
        captureEffect = _captureEffect;
        spawner = ballSpawner;

        audioManager = FindAnyObjectByType<AudioManager>();
        animator = ballObject.GetComponent<Animator>();

        speed = Random.Range(2, 5);
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
                newPos = new Vector3(spawner.GetRandomXPos(), 1f, spawner.GetRandomZPos());
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
        transform.SetParent(parent, true);
        // transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false; // hide ball sprite
        personAttached = parent.GetComponent<AugmentaPickup>();
        personAttached.AttachBallRing(this);
        // Instantiate(splat, transform.position + new Vector3(-0.0125f, 0f, 0f), Quaternion.Euler(90, -90, 0)); // run the splat with offset
        // PlayPickupSound();
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
            // PlayDropOffSound(targetSinkID);
            float delay = Random.Range(1f, 8f);
            spawner.DestroyBall(this);
        }
    }

    private void PlayPickupSound()
    {
        int randInt = Random.Range(1, 4);
        switch (randInt)
        {
            case 1:
                audioManager.Play("Ghost Pick Up 1");
                break;
            case 2:
                audioManager.Play("Ghost Pick Up 2");
                break;
            case 3:
                audioManager.Play("Ghost Pick Up 3");
                break;
            default:
                audioManager.Play("Ghost Pick Up 3");
                break;
        }
    }
    private void PlayDropOffSound(int portalID)
    {
        switch (portalID)
        {
            case 0:
                audioManager.Play("Drop Ghost Purple");
                break;
            case 1:
                audioManager.Play("Drop Ghost Green");
                break;
            case 2:
                audioManager.Play("Drop Ghost Yellow");
                break;
            case 3:
                audioManager.Play("Drop Ghost Blue");
                break;
            default:
                audioManager.Play("Drop Ghost Blue");
                break;
        }
    }


    private void PlayRandomMovementSound()
    {
        int randInt = Random.Range(0, 2);

        switch (randInt)
        {
            case 0:
                audioManager.Play("Ghost Movement 1");
                break;
            case 1:
                audioManager.Play("Ghost Movement 2");
                break;
            default:
                audioManager.Play("Ghost Movement 2");
                break;
        }
    }
}
