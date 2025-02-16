using com.absence.utilities;
using com.game.player;
using com.game.player.itemsystemextensions;
using System;
using UnityEngine;

namespace com.game.ui
{
    public class PlayerShopUI : MonoBehaviour
    {
        [SerializeField] private GameObject m_itemDisplayPrefab;
        [SerializeField] private Transform m_root;

        PlayerShop m_shop;

        private void Awake()
        {
            m_shop = Player.Instance.Hub.Shop;
            //m_shop.OnReroll += OnShopReroll;
        }

        //private void OnShopReroll(PlayerShop shop)
        //{
        //    Clear();
        //    foreach (PlayerItemProfile item in shop.ItemsOnStand)
        //    {
        //        Instantiate(m_itemDisplayPrefab, m_root);
        //    }
        //}

        //public void Clear()
        //{
        //    m_root.DestroyChildren();
        //}

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();

            DrawItems();

            if (GUILayout.Button("Reroll"))
            {
                m_shop.Reroll();
            }

            GUILayout.EndHorizontal();

            return;

            void DrawItems()
            {
                if (m_shop.ItemsOnStand == null)
                    return;

                GUILayout.BeginVertical();

                foreach (PlayerItemProfile item in m_shop.ItemsOnStand)
                {
                    GUILayout.Button(item.DisplayName);
                }

                GUILayout.EndVertical();
            }
        }
    }
}
