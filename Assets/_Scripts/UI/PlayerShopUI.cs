using com.absence.attributes.experimental;
using com.absence.utilities;
using com.game.itemsystem;
using com.game.itemsystem.scriptables;
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
        [SerializeField] private Button m_inventoryButton;
        [SerializeField] private Button m_rerollButton;
        [SerializeField] private Button m_proceedButton;
        [SerializeField, InlineEditor] private ItemDisplay m_itemDisplayPrefab;

        public event Action<PlayerItemProfile> OnItemBoughtOneShot;

        IShop<PlayerItemProfile> m_shop;

        List<ItemDisplay> m_currentDisplays = new();

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

        public void SetupButtons(Action inventoryButton, Action proceedButton)
        {
            SetupButton(m_inventoryButton, inventoryButton);
            SetupButton(m_proceedButton, proceedButton);
        }

        void OnShopReroll(IShop<PlayerItemProfile> shop)
        {
            GenerateDisplay(shop);
        }

        void GenerateDisplay(IShop<PlayerItemProfile> shop)
        {
            Clear();
            foreach (PlayerItemProfile item in shop.ItemsOnStand)
            {
                ItemDisplay display = Instantiate(m_itemDisplayPrefab, m_stand);
                display.Initialize(item);

                m_currentDisplays.Add(display);

                display.SetButtonText(GetItemButtonText(item), true);
                display.onButtonClick += OnDisplayBuyButtonClicked;
                display.buttonInteractabilityProvider = CanDisplayBuyButtonBeClicked;
                display.Refresh();
            }
        }

        private bool CanDisplayBuyButtonBeClicked(ItemDisplay display)
        {
            ItemProfileBase item = display.Profile;

            if (item is not PlayerItemProfile playerItem)
                return false;

            return Player.Instance.Hub.Money.CanAfford(playerItem.Price);
        }

        private void OnDisplayBuyButtonClicked(ItemDisplay display)
        {
            ItemProfileBase item = display.Profile;

            if (item is not PlayerItemProfile playerItem)
                return;

            Player.Instance.Hub.Money.Spend(playerItem.Price);
            Player.Instance.Hub.Inventory.Add(ItemObject.Create(item));
            display.buttonInteractabilityProvider = (_) => false;
            display.CanvasGroup.alpha = 0f;

            RefreshAll();
            InvokeOnItemBought(playerItem);
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
            foreach (ItemDisplay display in m_currentDisplays)
            {
                display.SetButtonText(GetItemButtonText(display.Profile as PlayerItemProfile), true);
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
