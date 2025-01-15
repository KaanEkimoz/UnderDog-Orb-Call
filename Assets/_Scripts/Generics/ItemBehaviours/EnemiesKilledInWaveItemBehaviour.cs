using com.game.itemsystem;
using com.game.player.statsystemextensions;
using UnityEngine;

namespace com.game.generics.itembehaviours
{
    public class EnemiesKilledInWaveItemBehaviour : ItemBehaviour
    {
        [SerializeField, Min(1)] private int m_amountOfEnemies = 1;
        [SerializeField] private PlayerStatModification m_modification = new();

        public override object[] GetDescriptionArguments()
        {
            string sign = m_modification.Value > 0 ? "+" : "";
            string mode = m_modification.ModificationType == 
                statsystem.presetobjects.StatModificationType.Percentage ? "%" : "";

            return new object[] 
            {
                sign,
                m_modification.Value,
                mode,
                m_modification.TargetStatType,
                m_amountOfEnemies,
            };
        }
    }
}
