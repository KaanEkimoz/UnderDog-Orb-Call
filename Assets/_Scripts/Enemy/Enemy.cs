using com.game.enemysystem;
using com.game.enemysystem.statsystemextensions;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    public EnemyMovementData enemyMovementData;
    public float slowPercentPerOrb = 25f;
    protected GameObject target;
    [SerializeField] protected EnemyStats enemyStats;
    protected NavMeshAgent navMeshAgent;
    float defaultSpeed;
    private float slowAmount = 0;

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {  
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

        AdjustSpeed((defaultSpeed + enemyStats.GetStat(EnemyStatType.WalkSpeed)) * (1 - (slowAmount/100)));
        navMeshAgent.SetDestination(target.transform.position);
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
    public void ApplySlowForOrbs(int orbCount)
    {
        slowAmount = slowPercentPerOrb * orbCount;
    }
    private IEnumerator SlowForSeconds(float slowPercent, float duration)
    {
        slowAmount = slowPercent;
        yield return new WaitForSeconds(duration);
        slowAmount = 0;
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

