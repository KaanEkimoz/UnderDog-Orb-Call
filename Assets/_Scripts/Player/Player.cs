using com.absence.attributes;
using com.absence.utilities;
using com.game.player.scriptables;
using UnityEngine;

namespace com.game.player
{
    public class Player : Singleton<Player>
    {
        [SerializeField, Readonly] private PlayerCharacterProfile m_characterProfile;
        [SerializeField] private PlayerComponentHub m_componentHub;

        public PlayerComponentHub Hub => m_componentHub;
        public PlayerCharacterProfile CharacterProfile 
        { 
            get
            {
                return m_characterProfile;
            }

            set
            {
                m_characterProfile = value;
            }
        }
    }
}
