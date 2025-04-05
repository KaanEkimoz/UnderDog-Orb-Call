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

    public void AssignPlayerStats(PlayerStats playerStats)
    {
        _playerStats = playerStats;
    }
    private void OnEnable()
    {
        Reset();
    }
    private void OnDisable()
    {
        
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

        //SetTrigger(true);
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
    public void Throw(Vector3 forceDirection)
    {
        currentState = OrbState.Throwing;

        // Distance Calculation
        throwStartPosition = transform.position;
        distanceTraveled = 0;

        if (m_light != null) m_light.SetActive(true);
        trailParticle.startLifetime = normalLifetime;
        _rigidBody.isKinematic = false;
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
    private void OnTriggerEnter(Collider triggerObject)
    {
        if (currentState == OrbState.Throwing)
        {
            Game.Event = com.game.GameRuntimeEvent.OrbThrow;

            ApplyOrbThrowTriggerEffects(triggerObject);

            Game.Event = com.game.GameRuntimeEvent.Null;
        }
        else if (currentState == OrbState.Returning)
        {
            Game.Event = com.game.GameRuntimeEvent.OrbCall;

            ApplyOrbReturnTriggerEffects(triggerObject);

            Game.Event = com.game.GameRuntimeEvent.Null;
        }
    }
    /*
    private void OnCollisionEnter(Collision collisionObject)
    {
        if(currentState != OrbState.Throwing)
            return;

        Game.Event = com.game.GameRuntimeEvent.OrbThrow;

        //ApplyOrbCollisionEffects(collisionObject);

        Game.Event = com.game.GameRuntimeEvent.Null;
    }*/
    private void Stick(Collider stickCollider, Vector3 stickPoint)
    {
        currentState = OrbState.Sticked;

        _rigidBody.linearVelocity = Vector3.zero;
        _rigidBody.isKinematic = true;
        //TO DO: FIND HOW TO GET FIRST TRIGGER CONTACT POINT
        //Disable Physics and Stick to Surface
        //Vector3 point = GetComponent<Collider>().ClosestPoint(stickCollider.transform.position);
        //Vector3 point = stickCollider.ClosestPoint(transform.position);
        //transform.position = point;
        //Debug.DrawRay(point, Vector3.up * 0.5f, Color.red, 2f);
        //transform.position = stickCollider.ClosestPoint(transform.position); // DOESN'T WORK
        if (stickPoint != Vector3.zero)
            transform.position = stickPoint;

        StickToTransform(stickCollider.transform);
    }
    
    protected virtual void ApplyOrbThrowTriggerEffects(Collider collider)
    {
        Vector3 hitPoint = Vector3.zero;

        // orb’un geldiði yön
        Vector3 direction = -_rigidBody.linearVelocity.normalized;

        // merkezden ray baþlat (biraz dýþarýdan baþlatmak istersen offset ekleyebilirsin)
        Vector3 rayStart = transform.position;

        RaycastHit hit;
        if (Physics.Raycast(rayStart, direction, out hit, 0.1f))
        {
            if (hit.collider == collider)
            {
                hitPoint = hit.point;
                Debug.DrawRay(rayStart, direction * hit.distance, Color.green, 2f);
                Debug.Log("Ýlk temas noktasý: " + hitPoint);
            }
        }

        // devam eden hasar iþlemleri
        if (collider.gameObject.TryGetComponent(out IDamageable damageable))
        {
            if (penetrationCount >= _playerStats.GetStat(PlayerStatType.Penetration))
                Stick(collider, hitPoint);

            ApplyCombatEffects(damageable, orbStats.GetStat(OrbStatType.Damage) + _playerStats.GetStat(PlayerStatType.OrbThrowDamage));

            if (collider.TryGetComponent(out Enemy hittedEnemy))
            {
                hittedEnemy.ApplySlowForSeconds(100f, 2f);
                hittedEnemy.ApplyKnockbackForce(transform.position, 1f);
            }

            penetrationCount++;
        }
        else
            Stick(collider, hitPoint);

        /*Vector3 hitPoint = Vector3.zero;
        Vector3 direction = _rigidBody.linearVelocity.normalized;
        Ray ray = new Ray(collider.transform.position, direction);
        RaycastHit hit;

        // Bu objenin collider'ý üzerinden ray atýyoruz
        if (_sphereCollider.Raycast(ray, out hit, 2f))
        {
            hitPoint = hit.point;
            Debug.Log("Raycast temas noktasý: " + hitPoint);
            Debug.DrawRay(ray.origin, direction * hit.distance, Color.red, 2f);
        }


        if (collider.gameObject.TryGetComponent(out IDamageable damageable))
        {
            if (penetrationCount >= _playerStats.GetStat(PlayerStatType.Penetration))
                Stick(collider,hitPoint);

            ApplyCombatEffects(damageable, orbStats.GetStat(OrbStatType.Damage) + _playerStats.GetStat(PlayerStatType.OrbThrowDamage));

            if (collider.gameObject.TryGetComponent(out Enemy hittedEnemy))
            {
                hittedEnemy.ApplySlowForSeconds(100f, 2f);
                hittedEnemy.ApplyKnockbackForce(transform.position, 1f);
            }

            penetrationCount++;
        }
        else
            Stick(collider, hitPoint);*/
    }
    protected virtual void ApplyOrbReturnTriggerEffects(Collider trigger)
    {
        if (trigger.gameObject.TryGetComponent(out IDamageable damageable))
        {
            ApplyCombatEffects(damageable, orbStats.GetStat(OrbStatType.Damage) + _playerStats.GetStat(PlayerStatType.OrbRecallDamage));

            if (trigger.gameObject.TryGetComponent(out Enemy hittedEnemy))
            {
                hittedEnemy.ApplySlowForSeconds(100f, 2f);
                hittedEnemy.ApplyKnockbackForce(transform.position, 1f);
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

        OnStuck?.Invoke();
        OnStateChanged?.Invoke(currentState);
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
