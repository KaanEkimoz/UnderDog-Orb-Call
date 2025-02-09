using com.absence.attributes.experimental;
using com.game.player.statsystemextensions;
using com.game.testing;
using UnityEngine;

namespace com.game.player
{
    public class PlayerLight : MonoBehaviour
    {
        static float s_threshold = 0.1f;

        [SerializeField] private Light m_light;
        [SerializeField] private float m_generalLightStatCoefficient;

        [SerializeField, BeginFoldoutGroup("Inner Spot Angle Settings"), Range(0f, 180f)] private float m_zeroInnerSpot;
        [SerializeField] private float m_innerSpotLightStatCoefficient;
        [SerializeField] private float m_stepInnerSpot;
        [SerializeField, EndFoldoutGroup] private float m_innerSpotSpeed;

        [SerializeField, BeginFoldoutGroup("Intensity Settings")] private float m_zeroIntensity;
        [SerializeField] private float m_intensityLigthStatCoefficient;
        [SerializeField] private float m_stepIntensity;
        [SerializeField, EndFoldoutGroup] private float m_intensitySpeed;

        [SerializeField, BeginFoldoutGroup("Range Settings")] private float m_zeroRange;
        [SerializeField] private float m_rangeLightStatCoefficient;
        [SerializeField] private float m_stepRange;
        [SerializeField, EndFoldoutGroup] private float m_rangeSpeed;

        PlayerStats m_playerStats;
        PlayerOrbHandler_Test m_orbHandler;

        private void Start()
        {
            m_playerStats = Player.Instance.Hub.Stats;
            m_orbHandler = Player.Instance.Hub.OrbHandler;
        }

        private void Update()
        {
            int orbsInHand = m_orbHandler.OrbsInHand;

            float refinedLightStat = m_playerStats.GetStat(PlayerStatType.LightStrength) * m_generalLightStatCoefficient;
            float generalCoefficient = orbsInHand;
            float generalShift = refinedLightStat;

            float innerSpotShift = m_innerSpotLightStatCoefficient * generalShift;
            float rangeShift = m_rangeLightStatCoefficient * generalShift;
            float intensityShift = m_intensityLigthStatCoefficient * generalShift;

            float initialInnerSpot = m_light.innerSpotAngle;
            float targetInnerSpot = (generalCoefficient * m_stepInnerSpot) + innerSpotShift;
            targetInnerSpot += m_zeroInnerSpot;

            targetInnerSpot = Mathf.Clamp(targetInnerSpot, 0f, 180f);

            //m_light.colorTemperature = 

            float initialRange = m_light.range;
            float initialIntensity = m_light.intensity;

            float targetRange = m_zeroRange + (generalCoefficient * m_stepRange) + rangeShift;
            float targetIntensity = m_zeroIntensity + (generalCoefficient * m_stepIntensity) + intensityShift;

            if (Mathf.Abs(initialIntensity - targetIntensity) > s_threshold) m_light.range = Mathf.Lerp(initialRange, targetRange, m_rangeSpeed * Time.deltaTime);
            if (Mathf.Abs(initialRange - targetRange) > s_threshold) m_light.intensity = Mathf.Lerp(initialIntensity, targetIntensity, m_intensitySpeed * Time.deltaTime);
            if (Mathf.Abs(initialInnerSpot - targetInnerSpot) > s_threshold) m_light.innerSpotAngle = Mathf.Lerp(initialInnerSpot, targetInnerSpot, m_innerSpotSpeed * Time.deltaTime);
        }
    }
}
