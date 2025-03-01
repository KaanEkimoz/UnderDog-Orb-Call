using com.absence.attributes.experimental;
using com.absence.utilities;
using com.game.itemsystem;
using com.game.player;
using com.game.player.itemsystemextensions;
using com.game.shopsystem;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace com.game.ui
{
    public class PlayerShopUI : MonoBehaviour
    {
        [SerializeField] private GameObject m_panel;
        [SerializeField] private RectTransform m_stand;
        [SerializeField] private Button m_rerollButton;
        [SerializeField] private Button m_proceedButton;
        [SerializeField, InlineEditor] private ItemDisplay m_itemDisplayPrefab;

        public event Action<PlayerItemProfile> OnItemBoughtOneShot;

        IShop<PlayerItemProfile> m_shop;

        Dictionary<ItemDisplay, PlayerItemProfile> m_currentDisplays = new();

        private void Start()
        {
            m_shop = Player.Instance.Hub.Shop;
            m_shop.OnReroll += OnShopReroll;

            Hide(true);
            SetupButton(m_rerollButton, SoftReroll);
        }

        public void SetVisibility(bool visibility)
        {
            m_panel.SetActive(visibility);
        }

        public void Show(bool reroll = false)
        {
            if (reroll) SoftReroll();
            SetVisibility(true);
        }

        public void Hide(bool clear = false)
        {
            SetVisibility(false);
            if (clear) Clear();
        }

        public void SetupButtons(Action proceedButton)
        {
            SetupButton(m_proceedButton, proceedButton);
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

                display.SetButtonText(GetItemButtonText(item), true);
                display.SetupButton(() => OnDisplayBuyButtonClicked(display), () => CanDisplayBuyButtonBeClicked(display));
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
            display.SetupButton(delegate { }, () => false);
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

        private void SetupButton(Button target, Action action)
        {
            if (action == null)
            {
                target.onClick.RemoveAllListeners();
                target.gameObject.SetActive(false);
                return;
            }

            target.gameObject.SetActive(true);
            target.onClick.AddListener(() => action.Invoke());
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
                display.SetButtonText(GetItemButtonText(kvp.Value), true);
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
