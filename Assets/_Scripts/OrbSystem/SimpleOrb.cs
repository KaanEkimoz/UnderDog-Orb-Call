using com.game;
using com.game.orbsystem.statsystemextensions;
using System;
using System.Collections;
using UnityEngine;
public enum OrbState
{
    OnEllipse,
    Sticked,
    Throwing,
    Returning
}
[RequireComponent(typeof(Rigidbody), typeof(SphereCollider), typeof(MeshRenderer))]
public class SimpleOrb : MonoBehaviour
{
    public OrbState currentState = OrbState.OnEllipse;

    [Header("Orb Movement")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private AnimationCurve movementCurve;
    [Header("Orb Throw")]
    [SerializeField] private float maxDistance = 20f;
    [Space]
    [Header("Orb Stats")]
    [SerializeField] private OrbStats orbStats;
    [Space]
    [Header("Orb Material")]
    [SerializeField] private Material startMaterial;
    [Space]
    [Header("Components")]
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private SphereCollider _sphereCollider;
    [SerializeField] private Renderer _renderer;

    //Movement
    private Transform startParent;
    private Vector3 startScale;
    private Vector3 currentTargetPos;
    private bool hasReachedTargetPos = false;
    private const float ellipseReachThreshold = 0.15f;

    //Throw
    private float distanceTraveled;
    private Vector3 throwStartPosition;

    //Events
    public event Action OnThrown;
    public event Action OnCalled;
    public event Action OnStuck;
    public event Action OnReachedToEllipse;
    public event Action<OrbState> OnStateChanged;

    private void OnEnable()
    {
        currentState = OrbState.OnEllipse;
        CheckStartVariables();
    }
    private void Start()
    {
        currentState = OrbState.OnEllipse;
        CheckStartVariables();
    }
    private void CheckStartVariables()
    {
        if(_rigidBody == null)
            _rigidBody = GetComponent<Rigidbody>();
        if(_sphereCollider == null)
            _sphereCollider = GetComponent<SphereCollider>();
        if(_renderer == null)
            _renderer = GetComponent<MeshRenderer>();
        if (orbStats == null)
            orbStats = GetComponent<OrbStats>();

        startParent = transform.parent;
        startScale = transform.localScale;
    }
    private void Update()
    {
        HandleTransformMovement();

        if (currentState == OrbState.Throwing)
        {
            CalculateDistanceTraveled();
            if (distanceTraveled >= maxDistance)
                Stick(startParent);
        }
        if (currentState == OrbState.Returning && hasReachedTargetPos)
        {
            _sphereCollider.isTrigger = false;
            currentState = OrbState.OnEllipse;

            OnReachedToEllipse?.Invoke();
            OnStateChanged?.Invoke(currentState);
        }
    }
    private void HandleTransformMovement()
    {
        if (currentState == OrbState.Returning || currentState == OrbState.OnEllipse)
            MoveTargetPos();
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }
    public void Return()
    {
        currentState = OrbState.Returning;

        _sphereCollider.isTrigger = true;
        _rigidBody.isKinematic = true;
        ResetParent();

        OnCalled?.Invoke();
        OnStateChanged?.Invoke(currentState);
    }
    public void Return(Vector3 returnPosition)
    {
        currentState = OrbState.Returning;

        _sphereCollider.isTrigger = true;
        _rigidBody.isKinematic = true;
        SetNewDestination(returnPosition);
        ResetParent();

        OnCalled?.Invoke();
        OnStateChanged?.Invoke(currentState);
    }
    public void ResetParent()
    {
        transform.SetParent(startParent);
        transform.localScale = startScale;
    }
    public void ResetMaterial()
    {
        _renderer.material = startMaterial;
    }
    public void SetMaterial(Material newMaterial)
    {
        _renderer.material = newMaterial;
    }
    public void Throw(Vector3 force)
    {
        currentState = OrbState.Throwing;

        // Distance Calculation
        throwStartPosition = transform.position;
        distanceTraveled = 0;

        //Enable Collision and Physics
        _rigidBody.isKinematic = false;
        _sphereCollider.isTrigger = false;
        _rigidBody.AddForce(force,ForceMode.Impulse);

        OnThrown?.Invoke();
        OnStateChanged?.Invoke(currentState);
    }
    private void CalculateDistanceTraveled()
    {
        distanceTraveled = Vector3.Distance(throwStartPosition, transform.position);
    }
    public void SetNewDestination(Vector3 newPos)
    {
        currentTargetPos = newPos;
    }
    public void SetNewDestination(Vector3 newPos, float multiplier)
    {
        currentTargetPos = newPos;
        speedMultiplier = multiplier;
    }
    private void MoveTargetPos()
    {
        float distanceToTarget = Vector3.Distance(transform.position, currentTargetPos);

        // AnimationCurve adjustments
        float curveValue = movementCurve.Evaluate(1 - (distanceToTarget / maxDistance));
        float currentSpeed = movementSpeed * curveValue * speedMultiplier;

        // MoveTowards to the target
        transform.position = Vector3.MoveTowards(transform.position, currentTargetPos, currentSpeed * Time.deltaTime);

        hasReachedTargetPos = distanceToTarget < ellipseReachThreshold;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(currentState != OrbState.Throwing)
            return;

        currentState = OrbState.Sticked;

        //Disable Physics and Stick to Surface
        transform.position = collision.contacts[0].point;
        Stick(collision.transform);

        if (collision.gameObject.TryGetComponent(out IDamageable damageable))
            damageable.TakeDamage(orbStats.GetStat(OrbStatType.Damage));
    }
    private void Stick(Transform transform)
    {
        currentState = OrbState.Sticked;
        _rigidBody.isKinematic = true;
        _sphereCollider.isTrigger = true;
        transform.SetParent(transform);

        OnStuck?.Invoke();
        OnStateChanged?.Invoke(currentState);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (currentState != OrbState.Returning)
            return;

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
