using UnityEngine;

[CreateAssetMenu(fileName = "newEnemyStat",menuName = "Enemy/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    public float speed;
    public float angularSpeed;
    public float acceleration;
    public float stoppingDistance;
    //public float attackRange;
    
    // Future Features: damage, attack delay, base health
}

