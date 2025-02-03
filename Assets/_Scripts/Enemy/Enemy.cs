using UnityEngine;
using UnityEngine.AI;
using EnemyNewStats = com.game.enemysystem.EnemyStats;

public class Enemy : MonoBehaviour
{
    public EnemyMovementData enemyMovementData;
    protected GameObject target;
    [SerializeField] protected EnemyNewStats newEnemyStats;

    protected NavMeshAgent navMeshAgent;

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {  
        target = GameObject.FindWithTag("Player");

        if (newEnemyStats == null)
            GetComponent<EnemyNewStats>();

        navMeshAgent.speed = enemyMovementData.speed;
        navMeshAgent.angularSpeed = enemyMovementData.angularSpeed;
        navMeshAgent.acceleration = enemyMovementData.acceleration;
        navMeshAgent.stoppingDistance = enemyMovementData.stoppingDistance;

        navMeshAgent.SetDestination(target.transform.position);
    }

    protected virtual void Update()
    {
        navMeshAgent.SetDestination(target.transform.position);

        CustomUpdate();
    }

    protected virtual void CustomUpdate() { }

    protected bool CheckDistanceToPlayer() //dusmanin playera uzakligini dondur
    {
        return Vector3.Distance(transform.position, target.transform.position) <= enemyMovementData.stoppingDistance;
    }


}

