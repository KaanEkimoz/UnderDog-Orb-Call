using com.absence.attributes;
using com.game.ai;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.enemysystem.ai
{
    [DefaultExecutionOrder(-1)]
    public class EnemyAIBuitlInScriptSelector : MonoBehaviour
    {
        [SerializeField, DisableIf(nameof(m_fetchFromSceneManager))] AISelection m_AISelection;
        [SerializeField] private bool m_fetchFromSceneManager = false;

        [Space]

        [SerializeField] private EnemyAINavMeshAgent m_navMeshAgentScript;
        [SerializeField] private EnemyAIPolarith m_polarithAIScript;

        public IEnemyAI Current => m_currentAI;

        Dictionary<AISelection, IEnemyAI> m_entries;
        IEnemyAI m_currentAI;

        private void Update()
        {
            if (m_currentAI == null)
                return;

            transform.position = m_currentAI.transform.position;
            transform.rotation = m_currentAI.transform.rotation;
        }

        private void Awake()
        {
            if (m_fetchFromSceneManager)
                m_AISelection = SceneManager.Instance.DefaultAISelection;

            m_entries = new()
            {
                { AISelection.NavMeshAgent, m_navMeshAgentScript },
                { AISelection.PolarithAI, m_polarithAIScript },
            };

            foreach (var kvp in m_entries)
            {
                kvp.Value.Enabled = false;
                kvp.Value.gameObject.SetActive(false);
            }

            m_currentAI = m_entries[m_AISelection];
            m_entries[m_AISelection].Enabled = true;
            m_entries[m_AISelection].gameObject.SetActive(true);
        }
    }
}


