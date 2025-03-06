using com.game.enemysystem;
using com.game.enemysystem.statsystemextensions;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private EnemyMovementData enemyMovementData;
    [Header("Slow")]
    [SerializeField] public float slowPercentPerOrb = 25f;
    [Header("Stats")]
    [SerializeField] protected EnemyStats enemyStats;

    //AI
    protected NavMeshAgent navMeshAgent;
    protected GameObject target;

    //Movement
    private float defaultSpeed;

    //Slow
    private float currentSlowAmount = 0;
    protected virtual void Start()
    {
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

    protected virtual void Update()
    {
        if (target == null) return;

        AdjustSpeed((defaultSpeed + enemyStats.GetStat(EnemyStatType.WalkSpeed)) * (1 - (currentSlowAmount/100)));
        
        RotateTowardsTarget();
        CustomUpdate();
    }
    protected virtual void CustomUpdate() { }
    public void AdjustSpeed(float newSpeed)
    {
        navMeshAgent.speed = newSpeed;
    }
    public void ApplySlowForSeconds(float slowPercent, float duration)
    {
        StartCoroutine(SlowForSeconds(slowPercent, duration));
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
    protected bool CheckDistanceToPlayer() //dusmanin playera uzakligini dondur
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

