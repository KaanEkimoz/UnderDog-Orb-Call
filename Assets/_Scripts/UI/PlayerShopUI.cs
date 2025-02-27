using com.absence.utilities;
using com.game.itemsystem;
using com.game.player;
using com.game.player.itemsystemextensions;
using com.game.shopsystem;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace com.game.ui
{
    public class PlayerShopUI : Singleton<PlayerShopUI>
    {
        [SerializeField] private ItemDisplay m_itemDisplayPrefab;
        [SerializeField] private GameObject m_panel;
        [SerializeField] private RectTransform m_stand;

        public event Action<PlayerItemProfile> OnItemBoughtOneShot;

        IShop<PlayerItemProfile> m_shop;

        Dictionary<ItemDisplay, PlayerItemProfile> m_currentDisplays = new();

        protected override void Awake()
        {
            base.Awake();

            m_shop = Player.Instance.Hub.Shop;
            m_shop.OnReroll += OnShopReroll;
        }

        public void SetVisibility(bool visibility)
        {
            m_panel.SetActive(visibility);
        }

        public void Show(bool reroll = false)
        {
            if (reroll) SoftReroll();
            m_panel.SetActive(true);
        }

        public void Hide(bool clear = false)
        {
            m_panel.SetActive(true);
            if (clear) Clear();
        }

        void OnShopReroll(IShop<PlayerItemProfile> shop)
        {
            GenerateDisplay(shop);
        }

        public virtual void GenerateDisplay(IShop<PlayerItemProfile> shop)
        {
            Clear();
            foreach (PlayerItemProfile item in shop.ItemsOnStand)
            {
                ItemDisplay display = Instantiate(m_itemDisplayPrefab, m_stand);
                display.Initialize(item);

                m_currentDisplays.Add(display, item);

                display.SetBuyButtonText(GetItemButtonText(item), true);
                display.SetupBuyButton(() => OnDisplayBuyButtonClicked(display), () => CanDisplayBuyButtonBeClicked(display));
            }
        }

        private bool CanDisplayBuyButtonBeClicked(ItemDisplay display)
        {
            PlayerItemProfile item = m_currentDisplays[display];

            return Player.Instance.Hub.Money.CanAfford(item.Price);
        }

        private void OnDisplayBuyButtonClicked(ItemDisplay display)
        {
            PlayerItemProfile item = m_currentDisplays[display];

            Player.Instance.Hub.Money.Spend(item.Price);
            Player.Instance.Hub.Inventory.Add(ItemObject.Create(item));
            display.SetupBuyButton(delegate { }, () => false);
            display.CanvasGroup.alpha = 0f;

            RefreshAll();
            InvokeOnItemBought(item);
        }

        private static string GetItemButtonText(PlayerItemProfile item)
        {
            StringBuilder sb = new("Buy ");

            int price = item.Price;
            bool canAfford = Player.Instance.Hub.Money.CanAfford(price);
            string colorLabel = canAfford ? "white" : "red";

            sb.Append("<color=");
            sb.Append(colorLabel);
            sb.Append(">");
            sb.Append(price);
            sb.Append("$");
            sb.Append("</color>");

            return sb.ToString();
        }

        private void InvokeOnItemBought(PlayerItemProfile item)
        {
            OnItemBoughtOneShot?.Invoke(item);
            OnItemBoughtOneShot = null;
        }

        public void SoftReroll()
        {
            m_shop.Reroll();
        }

        public void RefreshAll()
        {
            foreach (KeyValuePair<ItemDisplay, PlayerItemProfile> kvp in m_currentDisplays)
            {
                ItemDisplay display = kvp.Key;
                display.SetBuyButtonText(GetItemButtonText(kvp.Value), true);
                display.Refresh();
            }
        }

        public void Clear()
        {
            m_currentDisplays.Clear();
            m_stand.DestroyChildren();
        }
    }
}
