using com.absence.attributes;
using com.game.player;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.enemysystem
{
    [RequireComponent(typeof(EnemySpawner))]
    public class EnemySpawnerVirtualEnemyExtension : MonoBehaviour
    {
        public const bool BRUTE_FORCE = false;

        [SerializeField, Readonly] private EnemySpawner m_owner;
        [SerializeField] private float m_spawnDelay;

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

            GameObject enemyPrefab = m_owner.GetRandomEnemyPrefab();
            GameObject enemy = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
            EnemyInstance instance = enemy.GetComponent<EnemyInstance>();
            instance.Combatant.ProvidePlayerCombatant(Player.Instance.Hub.Combatant);
            VirtualEnemy.CreateNew(instance, Player.Instance.Hub.Light);
        }

        public void ResetTimer()
        {
            m_timer = m_spawnDelay;
        }

        public void KillAll()
        {
            VirtualEnemy.KillAll();
        }

        private void Reset()
        {
            m_owner = GetComponent<EnemySpawner>();
        }
    }
}
