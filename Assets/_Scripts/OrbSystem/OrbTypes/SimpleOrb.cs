using com.game;
using com.game.orbsystem.statsystemextensions;
using com.game.player;
using com.game.player.statsystemextensions;
using System;
using System.Collections;
using UnityEngine;
using com.game.enemysystem;
using com.absence.soundsystem;
using com.absence.soundsystem.internals;
using com.absence.attributes.experimental;
using com.game.orbsystem;
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
    [SerializeField, InlineEditor] protected OrbMovementData m_movementData;
    [SerializeField] private AnimationCurve movementCurve;
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
    [Header("Orb Throw Effects")]
    [Range(0f, 100f)] [SerializeField] private float throwSlowEffectOnHit = 100f;
    [Range(0f, 5f)] [SerializeField] private float throwSlowEffectOnHitSeconds = 1.5f;
    [SerializeField] private bool throwKnockbackEffectOnHit = true;
    [SerializeField] private bool throwKnockbackEffectOnPenetrationHit = false;
    [SerializeField] private float throwKnockbackEffectOnHitForce = 1f;
    [Space]
    [Header("Orb Call, Return Effects")]
    [Range(0f, 100f)] [SerializeField] private float returnSlowEffectOnHit = 100f;
    [Range(0f, 5f)] [SerializeField] private float returnSlowEffectOnHitSeconds = 1.5f;
    [SerializeField] private bool returnKnockbackEffectOnHit = true;
    [SerializeField] private float returnKnockbackEffectOnHitForce = 0.1f;
    [Space]
    [Header("Sound Asset References")]
    [SerializeField] private SoundAsset m_throwSoundAsset;
    [SerializeField] private SoundAsset m_recallSoundAsset;
    [SerializeField] private SoundAsset m_catchSoundAsset;

    //Movement
    private Transform startParent;
    private Vector3 startScale;
    private Vector3 currentTargetPos;
    private bool hasReachedTargetPos = false;
    private const float ellipseReachThreshold = 0.2f;
    private Transform stickedTransform;
    private Collider stickedCollider;

    //Throw
    private float distanceTraveled;
    private Vector3 throwStartPosition;
    private Vector3 throwVector;
    private int penetrationCount = 0;

    //Stats
    public OrbStats Stats => orbStats;
    private PlayerStats _playerStats;

    //Events
    public event Action OnThrown;
    public event Action OnCalled;
    public event Action OnStuck;
    public event Action OnReachedToEllipse;
    public event Action<OrbState> OnStateChanged;
    public event Action OnPenetrateHit;
    public event Action OnPhysicsHit;
    //Effects
    private SoundFXManager _soundFXManager;

    public void AssignPlayerStats(PlayerStats playerStats)
    {
        _playerStats = playerStats;
    }
    public void AssingSoundFXManager(SoundFXManager soundFXManager)
    {
        _soundFXManager = soundFXManager;
    }
    private void Reset()
    {
        currentState = OrbState.OnEllipse;
        if (m_light != null) m_light.SetActive(false);
        trailParticle.startLifetime = onEllipseLifetime;
        CheckStartVariables();
    }
    void PlaySFX(ISoundAsset asset)
    {
        if (SoundManager.Instance == null)
            return;

        Sound.Create(asset)
            .AtPosition(transform.position)
            .Play();
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
    private void Awake()
    {
        m_movementData = Instantiate(m_movementData);
    }
    private void Start()
    {
        CheckStartVariables();

        OnThrown += () => PlaySFX(m_throwSoundAsset);
        OnCalled += () => PlaySFX(m_recallSoundAsset);
        OnReachedToEllipse += () => PlaySFX(m_catchSoundAsset);

        if (m_light != null) m_light.SetActive(false);
        trailParticle.startLifetime = onEllipseLifetime;
    }
    private void Update()
    {
        HandleStateBehaviours();
    }
    private void FixedUpdate()
    {
        if (currentState == OrbState.Throwing)
        {
            float speedFromStats = (((orbStats.GetStat(OrbStatType.Speed) / 10) + 1) *
                ((_playerStats.GetStat(PlayerStatType.OrbThrowSpeed) / 10)) + 1);

            _rigidBody.linearVelocity = throwVector * Time.deltaTime
                * speedFromStats * m_movementData.movementSpeed * m_movementData.throwSpeedMultiplier;
        }
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
            if (m_light != null) m_light.SetActive(false);
            trailParticle.startLifetime = onEllipseLifetime;
            currentState = OrbState.OnEllipse;
            transform.parent = startParent;

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
        if (currentState == OrbState.Sticked)
        {
            if (stickedCollider != null)
                ApplyOrbReturnTriggerEffects(stickedCollider);

            stickedCollider = null;
            stickedTransform = null;
        }

        currentState = OrbState.Returning;

        SetNewDestination(returnPosition);
        ResetParent();
        _sphereCollider.isTrigger = true;
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
    public void Throw(Vector3 forceDirection)
    {
        currentState = OrbState.Throwing;

        // Distance Calculation
        throwStartPosition = transform.position;
        distanceTraveled = 0;

        if (m_light != null) m_light.SetActive(true);

        throwVector = forceDirection;

        trailParticle.startLifetime = normalLifetime;
        _rigidBody.isKinematic = false;
        _sphereCollider.isTrigger = false;
        
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
    private void MoveToTargetPosition()
    {
        float distanceToTarget = Vector3.Distance(transform.position, currentTargetPos);

        // AnimationCurve adjustments
        float dynamicMaxDistance = Mathf.Max(maxDistance + _playerStats.GetStat(PlayerStatType.Range), distanceToTarget + 10f);
        float curveValue = movementCurve.Evaluate(1 - (distanceToTarget / dynamicMaxDistance));
        float currentSpeed = m_movementData.movementSpeed * curveValue * m_movementData.recallSpeedMultiplier;

        // MoveTowards to the target
        transform.position = Vector3.MoveTowards(transform.position, currentTargetPos, currentSpeed * ((orbStats.GetStat(OrbStatType.Speed) / 10) + 1) * ((_playerStats.GetStat(PlayerStatType.OrbRecallSpeed) / 10) + 1) * Time.deltaTime);

        hasReachedTargetPos = distanceToTarget < ellipseReachThreshold;
    }
    
    private void OnCollisionEnter(Collision collisionObject)
    {
        if (currentState == OrbState.Throwing)
        {
            Game.Event = com.game.GameRuntimeEvent.OrbThrow;

            ApplyOrbThrowCollisionEffects(collisionObject);

            Game.Event = com.game.GameRuntimeEvent.Null;
        }
    }
    private void OnTriggerEnter(Collider triggerObject)
    {
        if (currentState == OrbState.Returning)
        {
            Game.Event = com.game.GameRuntimeEvent.OrbCall;

            if (stickedCollider == null || stickedCollider != triggerObject) 
                ApplyOrbReturnTriggerEffects(triggerObject);

            Game.Event = com.game.GameRuntimeEvent.Null;
        }
    }
    private void Stick(Collision stickCollider)
    {
        currentState = OrbState.Sticked;
        stickedCollider = stickCollider.collider;

        OnPhysicsHit?.Invoke();

        _rigidBody.isKinematic = true;
        transform.position = stickCollider.contacts[0].point;
        StickToTransform(stickCollider.transform);
    }
    
    protected virtual void ApplyOrbThrowCollisionEffects(Collision collisionObject)
    {
        if (collisionObject.gameObject.TryGetComponent(out IDamageable damageable))
        {
            bool penetrationCompleted = penetrationCount >= _playerStats.GetStat(PlayerStatType.Penetration);

            if (penetrationCompleted)
                Stick(collisionObject);

            ApplyCombatEffects(damageable, orbStats.GetStat(OrbStatType.Damage) + _playerStats.GetStat(PlayerStatType.OrbThrowDamage));

            if (collisionObject.gameObject.TryGetComponent(out Enemy hittedEnemy))
            {
                hittedEnemy.ApplySlowForSeconds(100f, 2f);

                if(penetrationCompleted)
                    hittedEnemy.ApplyKnockbackForce(transform.position, 1f);
            }

            penetrationCount++;
        }
        else
            Stick(collisionObject);

    }
    protected virtual void ApplyOrbReturnTriggerEffects(Collider triggerCollider)
    {
        if (triggerCollider.gameObject.TryGetComponent(out IDamageable damageable))
        {
            ApplyCombatEffects(damageable, orbStats.GetStat(OrbStatType.Damage) + _playerStats.GetStat(PlayerStatType.OrbRecallDamage));

            if (triggerCollider.gameObject.TryGetComponent(out Enemy hittedEnemy))
            {
                hittedEnemy.ApplySlowForSeconds(100f, 2f);
                hittedEnemy.ApplyKnockbackForce(transform.position, 0.1f);
            }
                
        }
    }
    protected virtual void ApplyCombatEffects(IDamageable damageableObject, float damage)
    {
        damageableObject.TakeDamage(damage);
    }
    private void StickToTransform(Transform stickTransform)
    {
        currentState = OrbState.Sticked;
        _rigidBody.isKinematic = true;
        transform.SetParent(stickTransform);
        stickedTransform = stickTransform;

        OnStuck?.Invoke();
        OnStateChanged?.Invoke(currentState);
    }
    
    public void IncreaseSpeedForSeconds(float speedIncrease, float duration)
    {
        StartCoroutine(IncreaseSpeed(speedIncrease, duration));
    }
    private IEnumerator IncreaseSpeed(float speedIncrease, float duration)
    {
        m_movementData.movementSpeed += speedIncrease;
        yield return new WaitForSeconds(duration);
        m_movementData.movementSpeed -= speedIncrease;
    }
}
