using com.game.abilitysystem;
using System;
using UnityEngine;

namespace com.game.player
{
    public class PlayerAbilitySet : MonoBehaviour
    {
        public event Action<PlayerAbilitySet> OnSetChanged;

        public IRuntimeAbility Ability1
        {
            get
            {
                return m_ability1;
            }

            set
            {
                m_ability1 = value;
                OnSetChanged?.Invoke(this);
            }
        }

        public IRuntimeAbility Ability2
        {
            get
            {
                return m_ability2;
            }

            set
            {
                m_ability2 = value;
                OnSetChanged?.Invoke(this);
            }
        }

        public IRuntimeAbility Ability3
        {
            get
            {
                return m_ability3;
            }

            set
            {
                m_ability3 = value;
                OnSetChanged?.Invoke(this);
            }
        }

        public IRuntimeAbility Ability4
        {
            get
            {
                return m_ability4;
            }

            set
            {
                m_ability4 = value;
                OnSetChanged?.Invoke(this);
            }
        }

        IRuntimeAbility m_ability1;
        IRuntimeAbility m_ability2;
        IRuntimeAbility m_ability3;
        IRuntimeAbility m_ability4;
    }
}
