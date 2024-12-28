using com.absence.attributes;
using UnityEngine;

namespace com.game.statsystem
{
    [CreateAssetMenu(fileName = "New PlayerDefaultStats", menuName = "Game/Player Default Stats", order = int.MinValue)]
    public class PlayerDefaultStats : ScriptableObject
    {
        [Header1("Default Stats for Player")]

        public float Health;
        public float Armor;
        public float WalkSpeed;
        public float LifeSteal;
        public float Luck;
        public float Gathering;
        public float Damage;
        public float AttackSpeed;
        public float CriticalHits;
        public float Range;
        public float Knockback;
        public float Penetration;
        public float CrowdControl;
        public float LightStrength;
    }
}
