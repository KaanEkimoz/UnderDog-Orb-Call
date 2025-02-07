using com.game.testing;
using UnityEngine;

namespace com.game.player
{
    public class PlayerLight : MonoBehaviour
    {
        [SerializeField] private Light m_light;
        [SerializeField] private float m_zeroIntensity;
        [SerializeField] private float m_stepIntensity;
        [SerializeField] private float m_zeroRange;
        [SerializeField] private float m_stepRange;
        PlayerOrbHandler_Test m_orbHandler;

        private void Start()
        {
            m_orbHandler = Player.Instance.Hub.OrbHandler;
        }

        private void Update()
        {
            int orbsInHand = m_orbHandler.OrbsInHand;

            m_light.range = m_zeroRange + (orbsInHand * m_stepRange);
            m_light.intensity = m_zeroIntensity + (orbsInHand * m_stepIntensity);
        }
    }
}
