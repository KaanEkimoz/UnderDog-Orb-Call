using UnityEngine;

[CreateAssetMenu(fileName = "newEnemyMovementData",menuName = "Enemy/EnemyMovementData")]
public class EnemyMovementData : ScriptableObject
{
    public float speed;
    public float angularSpeed;
    public float acceleration;
    public float stoppingDistance;
    
}

