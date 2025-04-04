using com.game.enemysystem;
using com.game.enemysystem.statsystemextensions;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    public static void DoFakify(Enemy target)
    {
        target.Fakify();
    }

    public static void DoVirtualize(Enemy target)
    {
        target.IsVirtual = true;
        target.enabled = false;
    }

    [Header("Movement")]
    [SerializeField] public EnemyMovementData enemyMovementData;
    [Header("Slow")]
    [SerializeField] public float slowPercentPerOrb = 25f;
    [Header("Stats")]
    [SerializeField] protected EnemyStats enemyStats;

    //AI
    protected NavMeshAgent navMeshAgent;
    protected GameObject target;

    //Movement
    private float defaultSpeed;

    public bool IsFake { get; set; } = false;
    public bool IsVirtual { get; protected set; } = false;

    //Slow
    private float currentSlowAmount = 0;

    //Dummy Mode
    [Header("Dummy Mode")]
    [SerializeField] protected bool isDummyModeActive = false;

    protected virtual void Start()
    {
        if (isDummyModeActive) return;

        if(navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();

        if (target == null)
            target = GameObject.FindWithTag("Player");

        if (enemyStats == null)
            GetComponent<EnemyStats>();


        defaultSpeed = enemyMovementData.speed;
        navMeshAgent.updateRotation = false;

        AdjustSpeed(defaultSpeed + enemyStats.GetStat(EnemyStatType.WalkSpeed));
        navMeshAgent.angularSpeed = enemyMovementData.angularSpeed;
        navMeshAgent.acceleration = enemyMovementData.acceleration;
        navMeshAgent.stoppingDistance = enemyMovementData.stoppingDistance;

        navMeshAgent.SetDestination(target.transform.position);
    }
    public void AdjustSpeed(float newSpeed)
    {
        navMeshAgent.speed = newSpeed;
    }
    protected virtual void Update()
    {
        if (isDummyModeActive || target == null) return;

        AdjustSpeed((defaultSpeed + enemyStats.GetStat(EnemyStatType.WalkSpeed)) * (1 - (currentSlowAmount/100)));
        
        RotateTowardsTarget();
        CustomUpdate();
    }
    protected virtual void CustomUpdate() { }
    protected virtual void Fakify() { }

    public void ApplySlowForSeconds(float slowPercent, float duration)
    {
        StartCoroutine(SlowForSeconds(slowPercent, duration));
    }
    public void ApplyKnockbackForce(Vector3 orbPosition, float knocbackForce)
    {
        Vector3 forceDirection = new Vector3(transform.position.x - orbPosition.x, 0,transform.position.z - orbPosition.z).normalized;
        Vector3 force = forceDirection * knocbackForce;
        transform.DOMove(transform.position + force, 0.4f).SetEase(Ease.OutBack);
    }
    private IEnumerator StopKnockback()
    {
        yield return new WaitForSeconds(0.2f);
        navMeshAgent.isStopped = false;
    }
    public void ApplySlowForOrbsOnEnemy(int orbCount)
    {
        currentSlowAmount = slowPercentPerOrb * orbCount;

        if (currentSlowAmount > 100)
            currentSlowAmount = 100;
    }
    private IEnumerator SlowForSeconds(float slowPercent, float duration)
    {
        currentSlowAmount = slowPercent;
        yield return new WaitForSeconds(duration);
        currentSlowAmount = 0;
    }
    protected bool GetDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, target.transform.position) <= enemyMovementData.stoppingDistance;
    }
    private void RotateTowardsTarget()
    {
        if (!navMeshAgent.hasPath) return;

        Vector3 direction = (navMeshAgent.steeringTarget - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * navMeshAgent.angularSpeed);
        }
    }

}

