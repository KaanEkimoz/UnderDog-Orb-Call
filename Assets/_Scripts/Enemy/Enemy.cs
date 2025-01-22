using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemyStats enemyStats;
    protected GameObject target;

    protected NavMeshAgent navMeshAgent;

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {  
        target = GameObject.FindWithTag("Player");

        navMeshAgent.speed = enemyStats.speed;
        navMeshAgent.angularSpeed = enemyStats.angularSpeed;
        navMeshAgent.acceleration = enemyStats.acceleration;
        navMeshAgent.stoppingDistance = enemyStats.stoppingDistance;

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
        return Vector3.Distance(transform.position, target.transform.position) <= enemyStats.stoppingDistance;
    }


}

