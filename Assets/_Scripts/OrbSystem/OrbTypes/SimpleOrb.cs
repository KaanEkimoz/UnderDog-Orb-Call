using com.game;
using com.game.orbsystem.statsystemextensions;
using com.game.player;
using com.game.player.statsystemextensions;
using System;
using System.Collections;
using UnityEngine;
public enum OrbState
{
    OnEllipse,
    OnEllipseMovement,
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
    [Space]
    [Header("Orb Effects")]
    [SerializeField] private float onEllipseLifetime = 0.1f;
    [SerializeField] private float normalLifetime = 0.5f;
    [SerializeField] private GameObject m_light;
    [SerializeField] private ParticleSystem trailParticle;

    //Movement
    private Transform startParent;
    private Vector3 startScale;
    private Vector3 currentTargetPos;
    private bool hasReachedTargetPos = false;
    private const float ellipseReachThreshold = 0.2f;

    //Throw
    private float distanceTraveled;
    private Vector3 throwStartPosition;

    //Stats
    public OrbStats Stats => orbStats;
    private PlayerStats _playerStats;

    //Events
    public event Action OnThrown;
    public event Action OnCalled;
    public event Action OnStuck;
    public event Action OnReachedToEllipse;
    public event Action<OrbState> OnStateChanged;

    public void AssignPlayerStats(PlayerStats playerStats)
    {
        _playerStats = playerStats;
    }
    private void OnEnable()
    {
        Reset();
    }
    private void Reset()
    {
        currentState = OrbState.OnEllipse;
        if (m_light != null) m_light.SetActive(false);
        trailParticle.startLifetime = onEllipseLifetime;
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
        HandleStateBehaviours();
    }
    private void HandleStateBehaviours()
    {
        if (currentState == OrbState.Returning || currentState == OrbState.OnEllipse)
            MoveToTargetPosition();
        if (currentState == OrbState.Throwing)
        {
            CalculateDistanceTraveled();
            if (distanceTraveled >= maxDistance + _playerStats.GetStat(PlayerStatType.Range))
                StickToTransform(startParent);
        }
        if (currentState == OrbState.Returning && hasReachedTargetPos)
        {
            _sphereCollider.isTrigger = false;
            if (m_light != null) m_light.SetActive(false);
            trailParticle.startLifetime = onEllipseLifetime;
            currentState = OrbState.OnEllipse;

            OnReachedToEllipse?.Invoke();
            OnStateChanged?.Invoke(currentState);
        }
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }
    public void ReturnToPosition(Vector3 returnPosition)
    {
        currentState = OrbState.Returning;

        SetTrigger(true);
        SetNewDestination(returnPosition);
        ResetParent();

        OnCalled?.Invoke();
        OnStateChanged?.Invoke(currentState);
    }
    private void SetTrigger(bool isEnabled)
    {
        _sphereCollider.isTrigger = isEnabled;
        _rigidBody.isKinematic = isEnabled;
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
    public void Throw(Vector3 forceDirection)
    {
        currentState = OrbState.Throwing;

        // Distance Calculation
        throwStartPosition = transform.position;
        distanceTraveled = 0;

        if (m_light != null) m_light.SetActive(true);
        trailParticle.startLifetime = normalLifetime;
        SetTrigger(false);
        ApplyForce(forceDirection);
        
        OnThrown?.Invoke();
        OnStateChanged?.Invoke(currentState);
    }
    private void ApplyForce(Vector3 direction)
    {
        Vector3 force = direction * (((orbStats.GetStat(OrbStatType.Speed) / 10)) * ((_playerStats.GetStat(PlayerStatType.OrbThrowSpeed) / 10)) + 1);

        _rigidBody.AddForce(force, ForceMode.Impulse);
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
    private void MoveToTargetPosition()
    {
        float distanceToTarget = Vector3.Distance(transform.position, currentTargetPos);

        // AnimationCurve adjustments
        float dynamicMaxDistance = Mathf.Max(maxDistance + _playerStats.GetStat(PlayerStatType.Range), distanceToTarget + 10f);
        float curveValue = movementCurve.Evaluate(1 - (distanceToTarget / dynamicMaxDistance));
        float currentSpeed = movementSpeed * curveValue * speedMultiplier;

        // MoveTowards to the target
        transform.position = Vector3.MoveTowards(transform.position, currentTargetPos, currentSpeed * ((_playerStats.GetStat(PlayerStatType.OrbRecallSpeed) / 10) + 1) * Time.deltaTime);

        hasReachedTargetPos = distanceToTarget < ellipseReachThreshold;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(currentState != OrbState.Throwing)
            return;

        currentState = OrbState.Sticked;

        //Disable Physics and Stick to Surface
        transform.position = collision.contacts[0].point;
        StickToTransform(collision.transform);

        ApplyCollisionEffects(collision);
    }
    protected virtual void ApplyCollisionEffects(Collision collisionObject)
    {
        if (collisionObject.gameObject.TryGetComponent(out IDamageable damageable))
            ApplyCombatEffects(damageable, orbStats.GetStat(OrbStatType.Damage) + _playerStats.GetStat(PlayerStatType.Damage));
    }
    protected virtual void ApplyCombatEffects(IDamageable damageableObject, float damage)
    {
        damageableObject.TakeDamage(damage);
    }
    private void StickToTransform(Transform stickTransform)
    {
        currentState = OrbState.Sticked;
        _rigidBody.isKinematic = true;
        _sphereCollider.isTrigger = true;
        transform.SetParent(stickTransform);

        OnStuck?.Invoke();
        OnStateChanged?.Invoke(currentState);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (currentState != OrbState.Returning)
            return;

        if (other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(orbStats.GetStat(OrbStatType.Damage) + _playerStats.GetStat(PlayerStatType.Damage));

            if (other.gameObject.TryGetComponent(out Enemy hittedEnemy))
                hittedEnemy.ApplySlowForSeconds(100f,2f);
        }
            
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
}
