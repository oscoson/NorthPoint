using UnityEngine;
using Augmenta;
using UnityEngine.Rendering;

/// Adds interactivity to an Augmenta object:
/// Pulsing influence ring and changes into respective Ghosts Sprite when captured
public class AugmentaPickup : MonoBehaviour
{
    /* ───────── Inspector Params ───────── */

    [Header("Orbit")]
    [SerializeField] float ringRadius = 1.0f;
    // [SerializeField] float velocity = 1.0f;     // radians per second
    public float speedToRingRadiusFactor = 0.5f;  // Degree to which orbit gets bigger upon speed change.
    public float speedDifferenceThreshold = 0.1f; // Saves computation

    [Header("Ring Look")]
    [SerializeField] float ringStroke = 0.20f;
    [SerializeField] int ringSegments = 64;
    [SerializeField] float pulseAmplitude = 0.25f;  // +/-25 % width
    [SerializeField] float pulseSpeed = 2.0f;   // Hz

    [Header("Delays")]
    [SerializeField] float pickupDelay = 0.5f;

    /* ───────── Private state ───────── */
    private AugmentaObject myAugmentaObject;
    private GameObject captureRingSprite;
    private CapsuleCollider myCollider;

    private EnergyBall carriedBall;
    private LineRenderer ring;
    private Material ringMat;
    private Color currentClr = Color.white;
    private Color targetClr = Color.white;
    private float baseWidth;

    private EnergyBall overlappingBall;
    private float pickupTimer;
    private bool isOverlapping = false;

    private float lastSpeed = -1f;

    void Awake()
    {
        myAugmentaObject = GetComponent<AugmentaObject>();

    }

    public void Initialise(float _ringRadius)
    {
        ringRadius = _ringRadius;
        myCollider = gameObject.AddComponent<CapsuleCollider>();
        myCollider.radius = _ringRadius;
        myCollider.direction = 1; // 0 = X, 1 = Y, 2 = Z 
        myCollider.height = 3.0f;
        myCollider.isTrigger = false;
        gameObject.AddComponent<SortingGroup>();
        gameObject.GetComponent<SortingGroup>().sortingLayerName = "Foreground";

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;                  // no forces, just follows pivot
        rb.useGravity = false;

        gameObject.AddComponent<SnowPathDrawer>();
        gameObject.GetComponent<SnowPathDrawer>().snowComputeShader = Resources.Load<ComputeShader>("SnowComputeShader");

        captureRingSprite = new GameObject("CaptureRingSprite");
        captureRingSprite.transform.SetParent(transform, false);
        captureRingSprite.transform.rotation = Quaternion.Euler(90, -90, 0);
        captureRingSprite.AddComponent<SpriteRenderer>();

        BuildRing();
    }

    void OnValidate()
    {
        if (myCollider == null)
        {
            return;
        }
        if (ringRadius != myCollider.radius) // only change if there's been a radius change
        {
            myCollider.radius = ringRadius;
            UpdateRingRadius(ringRadius);
        }
    }

    // Create aura ring
    void BuildRing()
    {
        var go = new GameObject("InfluenceRing");
        go.transform.SetParent(transform, false);

        ring = go.AddComponent<LineRenderer>();
        ring.useWorldSpace = false;
        ring.loop = true;
        ring.positionCount = ringSegments;
        baseWidth = ringStroke;
        ring.startWidth = ring.endWidth = baseWidth;

        ringMat = new Material(Shader.Find("Sprites/Default"));
        ringMat.color = currentClr;
        ring.material = ringMat;
        ring.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        ring.receiveShadows = false;

        Vector3[] pts = new Vector3[ringSegments];
        float step = 2 * Mathf.PI / ringSegments;
        for (int i = 0; i < ringSegments; i++)
        {
            float a = i * step;
            pts[i] = new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * ringRadius;
        }
        ring.SetPositions(pts);
    }

    // Called in Update() for orbit logic (radius change)
    void UpdateRingRadius(float newRadius)
    {
        ringRadius = newRadius;
        if (captureRingSprite.GetComponent<SpriteRenderer>() != null)
        {
            captureRingSprite.transform.localScale = ringRadius * 0.6f * Vector3.one; // scale capture sprite to fit inside ring
        }

        Vector3[] pts = new Vector3[ringSegments];
        float step = 2 * Mathf.PI / ringSegments;
        for (int i = 0; i < ringSegments; i++)
        {
            float a = i * step;
            pts[i] = new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * ringRadius;
        }
        ring.SetPositions(pts);
    }

    // Entered ball collider
    void OnTriggerEnter(Collider other)
    {
        if (carriedBall != null) return;

        if (other.TryGetComponent(out EnergyBall ball) && ball.state != EnergyBall.BallState.Attached)
        {
            overlappingBall = ball;
            pickupTimer = 0f;
            isOverlapping = true;
        }
    }

    // Exited Ball collider
    void OnTriggerExit(Collider other)
    {
        if (overlappingBall != null && other.gameObject == overlappingBall.gameObject)
        {
            overlappingBall = null;
            isOverlapping = false;
        }
    }

    void Update()
    {
        // Orbit motion
        if (carriedBall != null) // "I am already holding a ball"
        {
            float speed = myAugmentaObject.worldVelocity3D.magnitude;
            // Update only if speed changed significantly
            if (Mathf.Abs(speed - lastSpeed) > speedDifferenceThreshold)
            {
                UpdateRingRadius(1f + speed * speedToRingRadiusFactor);
                lastSpeed = speed;
            }
        }
        else if (isOverlapping && overlappingBall != null) // "I am colliding with a ball"
        {
            pickupTimer += Time.deltaTime;
            if (pickupTimer >= pickupDelay)
            {
                carriedBall = overlappingBall;
                targetClr = carriedBall.ballColor;
                carriedBall.AttachTo(transform);

                overlappingBall = null;
                isOverlapping = false;
            }
        }
        else
        {
            targetClr = Color.white;
        }

        // Pulsing ring width
        float pulse = 1 + Mathf.Sin(Time.time * Mathf.PI * pulseSpeed) * pulseAmplitude;
        ring.startWidth = ring.endWidth = baseWidth * pulse;

        // Smooth colour fade
        currentClr = Color.Lerp(currentClr, targetClr, Time.deltaTime * 5f);
        currentClr.a = 1f; // force opaque
        ringMat.color = currentClr;
    }

    // called externally by another class to help drop the ball possession
    public void DropBall()
    {
        DetachBallRing();
        UpdateRingRadius(1.0f); // return ring to original size
        if (carriedBall == null) return;

        carriedBall = null;           // Update() will fade back to white
    }

    public void AttachBallRing(EnergyBall ball)
    {
        // ball colour spawns with 0 alpha, and cannot modify component without getting a variable to copy first
        Color color = ball.ballColor;
        color.a = 1f;
        // captureRingSprite.GetComponent<SpriteRenderer>().sprite = ball.captureSprite; // to be added
        captureRingSprite.GetComponent<SpriteRenderer>().color = color;
    }
    public void DetachBallRing()
    {
        captureRingSprite.GetComponent<SpriteRenderer>().sprite = null;
    }
}
