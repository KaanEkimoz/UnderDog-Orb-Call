using com.absence.utilities;
using UnityEngine;

namespace com.game.player
{
    public class Player : Singleton<Player>
    {
        [SerializeField] private PlayerComponentHub m_componentHub;

        public PlayerComponentHub Hub => m_componentHub;
    }
}
