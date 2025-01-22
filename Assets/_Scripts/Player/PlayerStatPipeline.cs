using com.absence.attributes;
using com.game.player.statsystemextensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.game.player
{
    public class PlayerStatPipeline : MonoBehaviour
    {
        [SerializeField, Readonly] private List<PlayerStatPipelineComponentBase> m_pipelineComponentList;

        public List<PlayerStatPipelineComponentBase> Query => m_pipelineComponentList;

        private void Start()
        {
            Refresh();
            m_pipelineComponentList.ForEach(comp => comp.Initialize());
        }

        public float Process(PlayerStatType statType, float rawValue)
        {
            float value = rawValue;
            for (int i = 0; i < m_pipelineComponentList.Count; i++) 
            {
                value = m_pipelineComponentList[i].Process(statType, value);
            }

            return value;
        }

#if UNITY_EDITOR
        public void OnTestGUI()
        {
            m_pipelineComponentList.ForEach(comp => comp.OnTestGUI());
        }
#endif

        [Button("Refresh Pipeline Component List")]
        void Refresh()
        {
            m_pipelineComponentList = GetComponents<PlayerStatPipelineComponentBase>().ToList().
                OrderBy(comp => comp.Order).ToList();
        }
    }
}