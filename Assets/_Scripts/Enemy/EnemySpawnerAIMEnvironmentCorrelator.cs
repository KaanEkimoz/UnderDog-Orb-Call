using com.absence.attributes;
using Polarith.AI.Move;
using System;
using UnityEngine;

namespace com.game.enemysystem
{
    public class EnemySpawnerAIMEnvironmentCorrelator : MonoBehaviour
    {
        [SerializeField, Required] private EnemySpawner m_enemySpawner;
        [SerializeField, Required] private AIMEnvironment m_enemyEnvironment;

        private void Start()
        {
            m_enemyEnvironment.GameObjects = new(64);
            m_enemySpawner.OnEnemySpawned += OnEnemySpawned;
        }

        private void FixedUpdate()
        {
            //m_enemyEnvironment.UpdateLayerGameObjects();
        }

        private void OnEnemySpawned(GameObject @object, EnemyCombatant combatant)
        {
            m_enemyEnvironment.GameObjects.Add(@object);

            combatant.OnDie += (_) => m_enemyEnvironment.GameObjects.Remove(@object);
        }
    }
}
