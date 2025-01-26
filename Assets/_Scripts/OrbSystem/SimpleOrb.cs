using com.game;
using com.game.orbsystem.statsystemextensions;
using System.Collections;
using UnityEngine;
public class SimpleOrb : MonoBehaviour
{
    [Header("Orb Movement")]
    [SerializeField] private float movementSpeed = 5f;
    [Space]
    
    [Space]
    [Header("Orb Stats")]
    [SerializeField] private OrbStats orbStats;


    //Flags
    [SerializeField] private bool isOnEllipse = false;
    [SerializeField] private bool isSticked = false;
    [SerializeField] private bool isThrowing = false;
    [SerializeField] private bool isReturning = false;

    //Movement
    private Transform startParent;
    private Vector3 startScale;
    private Vector3 currentTargetPos;
    private bool hasReachedTargetPos = false;
    private const float distanceThreshold = 0.1f;

    //Components
    private Rigidbody _rigidBody;
    private SphereCollider _sphereCollider;

    private void Start()
    {
        isOnEllipse = true;
        isReturning = false;

        if (orbStats == null)
            orbStats = GetComponent<OrbStats>();

        _rigidBody = GetComponent<Rigidbody>();
        _sphereCollider = GetComponent<SphereCollider>();
        //swayOffset = Random.Range(0f, Mathf.PI * 2);
        startParent = transform.parent;
        startScale = transform.localScale;
    }
    private void Update()
    {
        HandleTransformMovement();

        if (isOnEllipse && isReturning && hasReachedTargetPos)
        {
            //TO DO: BUGFIX 
            isReturning = false;
            _sphereCollider.isTrigger = false;
        }

    }
    private void HandleTransformMovement()
    {
        if (!isSticked && !isThrowing)
            MoveTargetPos();
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }
    public void Return()
    {
        isReturning = true;

        isSticked = false;
        isThrowing = false;
        isOnEllipse = true;

        _sphereCollider.isTrigger = true;
        _rigidBody.isKinematic = true;
        ResetParent();
    }
    private void TogglePhysics()
    {
        _rigidBody.isKinematic = !_rigidBody.isKinematic;
    }
    public void ResetParent()
    {
        transform.SetParent(startParent);
        transform.localScale = startScale;
        isSticked = false;
    }
    public void Throw(Vector3 force)
    {
        isThrowing = true;
        isReturning = false;
        isOnEllipse = false;

        _rigidBody.isKinematic = false;
        _sphereCollider.isTrigger = false;
        _rigidBody.AddForce(force,ForceMode.Impulse);
    }
    public void SetNewDestination(Vector3 newPos)
    {
        currentTargetPos = newPos;
        isOnEllipse = false;
    }
    private void MoveTargetPos()
    {
        // Move towards target position
        Vector3 posToMove = currentTargetPos;
        transform.position = Vector3.Lerp(transform.position, posToMove, Time.deltaTime * movementSpeed);

        // Check if the orb has reached the target
        if (Vector3.Distance(transform.position, posToMove) < distanceThreshold)
            hasReachedTargetPos = true;
        else
            hasReachedTargetPos = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable damageable))
            damageable.TakeDamage(orbStats.GetStat(OrbStatType.Damage));

        if (!isReturning)
        {
            isSticked = true;
            isThrowing = false;
            _rigidBody.isKinematic = true;
            transform.position = collision.contacts[0].point;
            transform.SetParent(collision.transform);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamageable damageable))
            damageable.TakeDamage(orbStats.GetStat(OrbStatType.Damage));
    }
    public void IncreaseSpeedForSeconds(float speedIncrease, float duration)
    {
        StartCoroutine(IncreaseSpeed(speedIncrease, duration));
    }
    private IEnumerator IncreaseSpeed(float speedIncrease, float duration)
    {
        movementSpeed += speedIncrease;
        yield return new WaitForSeconds(duration);
        movementSpeed -= speedIncrease;
    }

    /*
    
    [Header("Orb Sway")]
    [SerializeField] private float swayRange = 0.1f;
    [SerializeField] private float swayOffset;
    [SerializeField] private float swaySpeed = 2f; 
     
    private void Sway()
    {
        Vector3 basePosition = currentTargetPos;
        float sway = Mathf.Sin(Time.time * swaySpeed + swayOffset) * swayRange;
        Vector3 swayPosition = new Vector3(basePosition.x, basePosition.y + sway, basePosition.z);

        transform.localPosition = Vector3.Lerp(transform.localPosition, swayPosition, Time.deltaTime * movementSpeed);
    }
    */
}
