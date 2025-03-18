using com.absence.attributes;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.enemysystem
{
    [RequireComponent(typeof(EnemySpawner))]
    public class EnemySpawnerFakeEnemyExtension : MonoBehaviour
    {
        public const bool BRUTE_FORCE = false;

        [SerializeField, Readonly] private EnemySpawner m_owner;
        [SerializeField] private EnemyMovementData m_fakeMovementData;
        [SerializeField] private float m_spawnDelay;
        [SerializeField] private int m_maxFakeEnemyCount;

        List<EnemyCombatant> m_instances = new();

        float m_timer;
        bool m_enabled = true;

        public bool Enabled
        {
            get
            {
                return m_enabled;
            }

            set
            {
                if (m_enabled != value)
                    ResetTimer();

                m_enabled = value;
            }
        }

        public int MaxInstances
        {
            get
            {
                return m_maxFakeEnemyCount;
            }

            set
            {
                m_maxFakeEnemyCount = value;
                TrimExcessEnemies();
            }
        }

        public float SpawnDelay
        {
            get
            {
                return m_spawnDelay;
            }

            set
            {
                m_spawnDelay = value;
                ResetTimer();
            }
        }

        private void Update()
        {
            if (!m_enabled) 
                return;

            if ((!BRUTE_FORCE) && m_spawnDelay <= 0f)
            {
                Debug.LogWarning("Spawn delay can not be 0 or lower!");
                return;
            }

            if (m_timer > 0f) m_timer -= Time.deltaTime;
            else SpawnRandom();
        }

        void SpawnRandom()
        {
            ResetTimer();

            if (m_instances.Count >= m_maxFakeEnemyCount)
                return;

            Transform spawnPoint = m_owner.GetRandomSpawnPoint();
            GameObject enemyPrefab = m_owner.GetRandomEnemyPrefab();

            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            EnemyCombatant combatant = enemy.GetComponentInChildren<EnemyCombatant>();

            if (m_fakeMovementData != null) combatant.Owner.enemyMovementData = m_fakeMovementData;
            combatant.ReinitializeAsFake();

            combatant.OnDie += () => m_instances.Remove(combatant);

            m_instances.Add(combatant);
        }

        public void ResetTimer()
        {
            m_timer = m_spawnDelay;
        }

        public void TrimExcessEnemies()
        {
            for (int i = 0; i < m_instances.Count; i++)
            {
                if (i < m_maxFakeEnemyCount) 
                    continue;

                EnemyCombatant combatant = m_instances[i];
                combatant.Die();
            }
        }

        private void Reset()
        {
            m_owner = GetComponent<EnemySpawner>();
        }
    }
}
