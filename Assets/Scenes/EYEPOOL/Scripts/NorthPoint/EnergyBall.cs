using UnityEngine;

public class EnergyBall : MonoBehaviour
{
    // Read-only towards other scripts
    public GameObject sprite { get; private set; }
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
    private float speed;
    private float size;

    private bool _initialised = false;
    private AugmentaPickup personAttached;
    [SerializeField] private GhostSpawner spawner;
    private AudioManager audioManager;
    private Animator animator;

    public void Initialise(GameObject _sprite, int _targetSinkID, Color _ballColor, Sprite _captureSprite, GameObject _captureEffect, GhostSpawner ghostSpawner)
    {
        if (_initialised)
        {
            // Consider removing this (Kevin's code but better safe than sorry?)
            return;
        }
        sprite = _sprite;
        targetSinkID = _targetSinkID;
        ballColor = _ballColor;
        captureSprite = _captureSprite;
        captureEffect = _captureEffect;
        spawner = ghostSpawner;

        audioManager = FindAnyObjectByType<AudioManager>();
        animator = sprite.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_initialised) return; // again, consider removing
        switch (state)
        {
            case BallState.Hovering:
                break;
            case BallState.Planning:
                break;
            case BallState.Moving:
                break;
            case BallState.Attached:
                break;
        }
    }

    public void AttachTo(Transform parent)
    {
        // Attach to Player
    }

    public void Detach(bool reachedCorrectSink)
    {
        // Detach and destroy
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
