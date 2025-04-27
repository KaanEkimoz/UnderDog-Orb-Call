using com.absence.timersystem;
using UnityEngine;

namespace com.game.orbsystem.temporary
{
    public class PLACEHOLDERElectricOrb : SimpleOrb
    {
        [SerializeField] private int m_maxElectricBounceCount = 3;
        [SerializeField] private float m_electricBounceRadius = 15f;
        [SerializeField] private float m_electricEffectIntervalInSeconds = 1f;
        [SerializeField] private float m_electricDamageMultiplier = 1f;

        [SerializeField] private GameObject electricInstantEffectPrefab;
        [SerializeField] private GameObject electricChainEffectPrefab;

        int m_bounceCount;

        protected override void ApplyCombatEffects(IDamageable damageableObject, float damage)
        {
            base.ApplyCombatEffects(damageableObject, damage * m_electricDamageMultiplier);

            //Timer
        }
    }
}
