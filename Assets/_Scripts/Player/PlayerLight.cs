using com.absence.attributes.experimental;
using com.game.testing;
using UnityEngine;

namespace com.game.player
{
    public class PlayerLight : MonoBehaviour
    {
        static float s_threshold = 0.1f;

        [SerializeField] private Light m_light;

        [SerializeField, BeginFoldoutGroup("Intensity Settings")] private float m_zeroIntensity;
        [SerializeField] private float m_stepIntensity;
        [SerializeField, EndFoldoutGroup] private float m_intensitySpeed;

        [SerializeField, BeginFoldoutGroup("Range Settings")] private float m_zeroRange;
        [SerializeField] private float m_stepRange;
        [SerializeField, EndFoldoutGroup] private float m_rangeSpeed;

        PlayerOrbHandler_Test m_orbHandler;

        private void Start()
        {
            m_orbHandler = Player.Instance.Hub.OrbHandler;
        }

        private void Update()
        {
            int orbsInHand = m_orbHandler.OrbsInHand;

            float initialRange = m_light.range;
            float initialIntensity = m_light.intensity;

            float targetRange = m_zeroRange + (orbsInHand * m_stepRange);
            float targetIntensity = m_zeroIntensity + (orbsInHand * m_stepIntensity);

            if (Mathf.Abs(initialIntensity - targetIntensity) > s_threshold) m_light.range = Mathf.Lerp(initialRange, targetRange, m_rangeSpeed * Time.deltaTime);
            if (Mathf.Abs(initialRange - targetRange) > s_threshold) m_light.intensity = Mathf.Lerp(initialIntensity, targetIntensity, m_intensitySpeed * Time.deltaTime);
        }
    }
}
