using com.game.itemsystem;
using com.game.player.itemsystemextensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.game.player
{
    public class PlayerShop : MonoBehaviour
    {
        public List<PlayerItemProfile> ItemsAvailable => m_itemsAvailable;
        public PlayerItemProfile[] ItemsOnStand => m_itemsOnStand;

        public event Action<PlayerShop> OnReroll = null;

        List<PlayerItemProfile> m_itemsAvailable;
        PlayerItemProfile[] m_itemsOnStand;

        private void Awake()
        {
            Reinitialize();
        }

        public void Reinitialize()
        {
            m_itemsAvailable = ItemManager.GetItemsOfType<PlayerItemProfile>();
        }

        public void Reroll()
        {
            Reroll(Constants.Shopping.PLAYER_SHOP_CAPACITY);
        }

        public void Reroll(int count)
        {
            // CHANGE LOGIC!!!
            m_itemsOnStand = new PlayerItemProfile[count];
            for (int i = 0; i < count; i++)
            {
                m_itemsOnStand[i] = m_itemsAvailable[UnityEngine.Random.Range(0, m_itemsAvailable.Count)];
            }
        }
    }
}
