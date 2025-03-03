using com.absence.attributes.experimental;
using com.game.generics.interfaces;
using com.game.player.statsystemextensions;
using com.game.testing;
using System.Collections.Generic;
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
        [SerializeField] private LayerMask m_sparkMask;
        [SerializeField] private LayerMask m_groundMask;
        [SerializeField] private float m_generalLightStatCoefficient;
        [SerializeField] private float m_threshold = k_threshold;

        [SerializeField] private FieldSettings m_innerSpotAngleSettings;
        [SerializeField] private FieldSettings m_outerSpotAngleSettings;
        [SerializeField] private FieldSettings m_intensitySettings;
        [SerializeField] private FieldSettings m_rangeSettings;
        [SerializeField] private FieldSettings m_localHeightSettings;

        [SerializeField, BeginFoldoutGroup("Vision Settings")] private float m_fullVisionRadiusShift;
        [SerializeField] private float m_halfVisionRadiusShift;
        [SerializeField, Min(0)] private float m_fullVisionRadiusMultiplier;
        [SerializeField, Min(0), EndFoldoutGroup()] private float m_halfVisionRadiusMultiplier;

        PlayerStats m_playerStats;
        PlayerOrbHandler m_orbHandler;
        int m_orbsInHand;
        float m_rawLightStat;
        float m_generalCoefficient;
        float m_generalShift;
        float m_fullVisionRadius;
        float m_halfVisionRadius;
        RaycastHit m_groundData;
        bool m_hasGround;

#if UNITY_EDITOR
        int m_fullSeenCount;
        int m_halfSeenCount;
#endif

        List<IVisible> m_halfVisibles;
        List<IVisible> m_fullVisibles;

        public float FullVisionRadius => m_fullVisionRadius;
        public float HalfVisionRadius => m_halfVisionRadius;

        private void Awake()
        {
            m_halfVisibles = new();
            m_fullVisibles = new();
        }

        private void Start()
        {
            m_playerStats = Player.Instance.Hub.Stats;
            m_orbHandler = Player.Instance.Hub.OrbHandler;
        }

        private void Update()
        {
            CacheVariables();
            UpdateFields();
            CalculateView(out _);
            DetectAll();
            SparkAll();
        }

        void CacheVariables()
        {
            m_orbsInHand = m_orbHandler.OrbsInHand;
            m_rawLightStat = m_playerStats.GetStat(PlayerStatType.LightStrength);

            float refinedLightStat = m_rawLightStat * m_generalLightStatCoefficient;
            m_generalCoefficient = m_orbsInHand;
            m_generalShift = refinedLightStat;
        }

        void UpdateFields()
        {
            m_light.innerSpotAngle = UpdateViaFieldData(m_innerSpotAngleSettings, m_light.innerSpotAngle);
            m_light.spotAngle = UpdateViaFieldData(m_outerSpotAngleSettings, m_light.spotAngle);
            m_light.intensity = UpdateViaFieldData(m_intensitySettings, m_light.intensity);
            m_light.range = UpdateViaFieldData(m_rangeSettings, m_light.range);

            Vector3 lightLocalPosition = m_light.transform.localPosition;
            lightLocalPosition.y = UpdateViaFieldData(m_localHeightSettings, lightLocalPosition.y);
            m_light.transform.localPosition = lightLocalPosition;
        }

        public bool CalculateView(out RaycastHit groundData)
        {
            float outerAngleInRads = m_light.spotAngle * Mathf.Deg2Rad;
            float innerAngleInRads = m_light.innerSpotAngle * Mathf.Deg2Rad;
            float range = m_light.range;

            m_hasGround = Physics.Raycast(m_light.transform.position,
            Vector3.down,
            out groundData,
            range,
            m_groundMask,
            QueryTriggerInteraction.UseGlobal);

            m_fullVisionRadius = Mathf.Tan(innerAngleInRads / 2) * m_groundData.distance;
            m_halfVisionRadius = Mathf.Tan(outerAngleInRads / 2) * m_groundData.distance;

            m_fullVisionRadius *= m_fullVisionRadiusMultiplier;
            m_halfVisionRadius *= m_halfVisionRadiusMultiplier;

            m_fullVisionRadius += m_fullVisionRadiusShift;
            m_halfVisionRadius += m_halfVisionRadiusShift;

            m_groundData = groundData;
            return m_hasGround;
        }

        void DetectAll()
        {
            bool hit = m_hasGround;
            if (!hit) return;

            Vector3 position = transform.position;
            position.y = 0f;

            Collider[] rawFullSeens = Physics.OverlapSphere(position, m_fullVisionRadius, m_sparkMask);
            Collider[] rawHalfSeens = Physics.OverlapSphere(position, m_halfVisionRadius, m_sparkMask);
            
            m_fullVisibles.Clear();
            m_halfVisibles.Clear();

            foreach (Collider collider in rawFullSeens)
            {
                if (!collider.TryGetComponent(out IVisible visible)) continue;

                m_fullVisibles.Add(visible);
            }

            foreach (Collider collider in rawHalfSeens)
            {
                if (!collider.TryGetComponent(out IVisible visible)) continue;
                if (m_fullVisibles.Contains(visible)) continue;

                m_halfVisibles.Add(visible);
            }

            m_fullSeenCount = m_fullVisibles.Count;
            m_halfSeenCount = m_halfVisibles.Count;
        }

        void SparkAll()
        {
            m_halfVisibles.ForEach(full => full.Spark.Spark());
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
    }
}
