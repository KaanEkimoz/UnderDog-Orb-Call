using com.game.player.statsystemextensions;
using UnityEngine;
using System.Collections.Generic;
using com.game.testing;

namespace com.game.player
{
    [DisallowMultipleComponent]
    public class PlayerStatPipelineOrbCountEffect : PlayerStatPipelineComponentBase
    {
        const float k_guiWidth = 540f;

        const float k_minAmplitude = 0f;
        const float k_maxAmplitude = 5f;
        const float k_minShift = -1f;
        const float k_maxShift = 0f;
        const float k_minCoefficient = -1f;
        const float k_maxCoefficient = 1f;

        [SerializeField] private AnimationCurve m_ascendingOrbCountCurve;
        [SerializeField] private bool m_inverse;
        [SerializeField, Range(k_minCoefficient, k_maxCoefficient)] private float m_generalCoefficient = 0f;
        [SerializeField, Range(k_minShift, k_maxShift)] private float m_shift = -0.5f;
        [SerializeField, Range(k_minAmplitude, k_maxAmplitude)] private float m_amplitude = 1f;

        PlayerOrbHandler_Test m_orbHandler;

        float m_countToGraphX;

        string m_curveType;

        Dictionary<int, float> m_evaluationPairs;

        public float GeneralCoefficient => m_generalCoefficient;
        public float Amplitude => m_amplitude;
        public float Shift => m_shift;
        public string CurveType => m_curveType;
        public bool Inverse => m_inverse;

        protected override void Initialize_Internal()
        {
            m_orbHandler = Player.Instance.Hub.OrbHandler;

            m_curveType = "Custom";
            m_countToGraphX = 1f / m_orbHandler.MaxOrbsCanBeHeld;

#if !UNITY_EDITOR
            m_evaluationPairs = new();
            for (int i = 0; i <= m_orbHandler.MaxOrbsCanBeHeld; i++)
            {
                Evaluate(i);
            }
#endif
            Evaluate(m_orbHandler.OrbsInHand);
        }

        protected override float Process_Internal(PlayerStatType statType, float statCoefficient, float rawValue)
        {
            int orbCount = statCoefficient < 0f ?
                m_orbHandler.MaxOrbsCanBeHeld - m_orbHandler.OrbsInHand : m_orbHandler.OrbsInHand;

            float realStatCoefficient = Mathf.Abs(statCoefficient);
            float precalculatedDiff = Evaluate(orbCount);

            float generalDiff = m_generalCoefficient * Mathf.Abs(rawValue) * precalculatedDiff;
            return rawValue + (precalculatedDiff * realStatCoefficient) + generalDiff;
        }

        private float Evaluate(int orbCount)
        {
            if (orbCount < 0 || orbCount > m_orbHandler.MaxOrbsCanBeHeld)
                return float.NaN;

            if (m_inverse) orbCount = m_orbHandler.MaxOrbsCanBeHeld - orbCount;

#if !UNITY_EDITOR
            if (m_evaluationPairs.ContainsKey(orbCount))
                return m_evaluationPairs[orbCount];
#endif

            float realXPosition = orbCount * m_countToGraphX;
            float rawGraphY = m_ascendingOrbCountCurve.Evaluate(realXPosition);
            float refinedGraphY = rawGraphY + m_shift;

            float precalculatedDiff = refinedGraphY * m_amplitude;

#if !UNITY_EDITOR
            m_evaluationPairs.Add(orbCount, precalculatedDiff);
#endif
            return precalculatedDiff;
        }

        public override void OnTestGUI()
        {
            float precalculatedDiff = Evaluate(m_orbHandler.OrbsInHand);

            GUILayout.BeginVertical("box", GUILayout.Width(k_guiWidth));

            GUILayout.Label("Orb Count Effect");

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();

            GUILayout.Label($"General Stat Value Coefficient: ({m_generalCoefficient.ToString("0.00")})");
            m_generalCoefficient = GUILayout.HorizontalSlider(m_generalCoefficient,
                k_minCoefficient, k_maxCoefficient);

            GUILayout.Label($"Orb Count Curve Amplitude: ({m_amplitude.ToString("0.00")})");
            m_amplitude = GUILayout.HorizontalSlider(m_amplitude,
                k_minAmplitude, k_maxAmplitude);

            GUILayout.Label($"Orb Count Curve Shift: ({m_shift.ToString("0.00")})");
            m_shift = GUILayout.HorizontalSlider(m_shift,
                k_minShift, k_maxShift);

            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            if (GUILayout.Button("Make Linear"))
            {
                m_ascendingOrbCountCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
                m_curveType = "Linear";
            }

            if (GUILayout.Button("Make EaseInOut"))
            {
                m_ascendingOrbCountCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
                m_curveType = "EaseInOut";
            }

            GUILayout.Label($"Curve: {m_curveType}");

            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            bool cannotAddOrb = m_orbHandler.OrbsAtMax;

            if (cannotAddOrb) GUI.enabled = false;
            if (GUILayout.Button("Add Orb"))
            {
                m_orbHandler.AddOrb();
            }
            if (cannotAddOrb) GUI.enabled = true;

            bool cannotRemoveOrb = m_orbHandler.OrbsAtMin;

            if (cannotRemoveOrb) GUI.enabled = false;
            if (GUILayout.Button("Remove Orb"))
            {
                m_orbHandler.RemoveOrb();
            }
            if (cannotRemoveOrb) GUI.enabled = true;

            GUILayout.Label("Orb Count -> Modifier:");
            GUILayout.Label($"{m_orbHandler.OrbsInHand} -> {precalculatedDiff}");

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }
    }
}
