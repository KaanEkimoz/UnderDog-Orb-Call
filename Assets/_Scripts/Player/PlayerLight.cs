using com.game.player.statsystemextensions;
using com.game.testing;
using UnityEngine;

namespace com.game.player
{
    public class PlayerLight : MonoBehaviour
    {
        [System.Serializable]
        public class FieldSettings
        {
            [Min(0)] public float InitialValue;
            [Min(0)] public float LightStatCoefficient;
            [Min(0)] public float StepAmount;
            [Min(0)] public float AnimationSpeed;
        }

        const float k_threshold = 0.1f;

        [SerializeField] private Light m_light;
        [SerializeField] private LayerMask m_groundMask;
        [SerializeField] private float m_generalLightStatCoefficient;
        [SerializeField] private float m_threshold = k_threshold;

        [SerializeField] private FieldSettings m_innerSpotAngleSettings;
        [SerializeField] private FieldSettings m_outerSpotAngleSettings;
        [SerializeField] private FieldSettings m_intensitySettings;
        [SerializeField] private FieldSettings m_rangeSettings;
        [SerializeField] private FieldSettings m_localHeightSettings;

        PlayerStats m_playerStats;
        PlayerOrbHandler_Test m_orbHandler;
        int m_orbsInHand;
        float m_rawLightStat;
        float m_generalCoefficient;
        float m_generalShift;

        private void Start()
        {
            m_playerStats = Player.Instance.Hub.Stats;
            m_orbHandler = Player.Instance.Hub.OrbHandler;
        }

        private void Update()
        {
            CacheVariables();

            m_light.innerSpotAngle = UpdateViaFieldData(m_innerSpotAngleSettings, m_light.innerSpotAngle);
            m_light.spotAngle = UpdateViaFieldData(m_outerSpotAngleSettings, m_light.spotAngle);
            m_light.intensity = UpdateViaFieldData(m_intensitySettings, m_light.intensity);
            m_light.range = UpdateViaFieldData(m_rangeSettings, m_light.range);

            Vector3 lightLocalPosition = m_light.transform.localPosition;
            lightLocalPosition.y = UpdateViaFieldData(m_localHeightSettings, lightLocalPosition.y);
            m_light.transform.localPosition = lightLocalPosition;
        }

        void CacheVariables()
        {
            m_orbsInHand = m_orbHandler.OrbsInHand;
            m_rawLightStat = m_playerStats.GetStat(PlayerStatType.LightStrength);

            float refinedLightStat = m_rawLightStat * m_generalLightStatCoefficient;
            m_generalCoefficient = m_orbsInHand;
            m_generalShift = refinedLightStat;
        }

        float UpdateViaFieldData(FieldSettings target, float initialValue)
        {
            float result = initialValue;

            float localZero = target.InitialValue;
            float localCoefficient = target.LightStatCoefficient;
            float localSpeed = target.AnimationSpeed;
            float localStep = target.StepAmount;

            float shift = localCoefficient * m_generalShift;
            float targetValue = (m_generalCoefficient * localStep) + shift + localZero;

            if (Mathf.Abs(initialValue - targetValue) > m_threshold) 
                result = Mathf.Lerp(initialValue, targetValue, localSpeed * Time.deltaTime);

            return result;
        }

        private void OnDrawGizmos()
        {
            float outerAngleInRads = m_light.spotAngle * Mathf.Deg2Rad;
            float innerAngleInRads = m_light.innerSpotAngle * Mathf.Deg2Rad;
            float range = m_light.range;

            bool hit = Physics.Raycast(m_light.transform.position,
            Vector3.down,
            out RaycastHit groundData,
            range,
            m_groundMask,
            QueryTriggerInteraction.UseGlobal);

            float innerRadius = Mathf.Tan(innerAngleInRads / 2) * groundData.distance;
            float outerRadius = Mathf.Tan(outerAngleInRads / 2) * groundData.distance;

            const float kAlpha = 1f;

            Color fullSeenColor = Color.red;
            fullSeenColor.a = kAlpha;

            Color halfSeenColor = Color.yellow;
            halfSeenColor.a = kAlpha;

#if UNITY_EDITOR
            UnityEditor.Handles.color = fullSeenColor;
            if (hit) UnityEditor.Handles.DrawWireDisc(groundData.point, Vector2.up, innerRadius);
            UnityEditor.Handles.color = halfSeenColor;
            if (hit) UnityEditor.Handles.DrawWireDisc(groundData.point, Vector2.up, outerRadius);
#endif

            //Gizmos.color = fullSeenColor;
            //if (hit) Gizmos.DrawSphere(groundData.point, innerRadius);
            //Gizmos.color = halfSeenColor;
            //if (hit) Gizmos.DrawSphere(groundData.point, outerRadius);
        }
    }
}
