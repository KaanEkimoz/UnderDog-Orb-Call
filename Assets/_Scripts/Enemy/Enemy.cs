using com.game.enemysystem;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemyMovementData enemyMovementData;
    protected GameObject target;
    [SerializeField] protected EnemyStats newEnemyStats;

    protected NavMeshAgent navMeshAgent;

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {  
        target = GameObject.FindWithTag("Player");

        if (newEnemyStats == null)
            GetComponent<EnemyStats>();

        navMeshAgent.updateRotation = false; //Donus yonetimini manuel yapmak icin
        navMeshAgent.speed = enemyMovementData.speed;
        navMeshAgent.angularSpeed = enemyMovementData.angularSpeed;
        navMeshAgent.acceleration = enemyMovementData.acceleration;
        navMeshAgent.stoppingDistance = enemyMovementData.stoppingDistance;

        navMeshAgent.SetDestination(target.transform.position);
    }

    protected virtual void Update()
    {
        if (target == null) return;

        navMeshAgent.SetDestination(target.transform.position);
        RotateTowardsTarget();
        CustomUpdate();
    }

    protected virtual void CustomUpdate() { }

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

