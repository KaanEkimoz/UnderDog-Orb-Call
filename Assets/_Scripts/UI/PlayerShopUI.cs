using com.absence.attributes.experimental;
using com.absence.utilities;
using com.game.itemsystem;
using com.game.itemsystem.scriptables;
using com.game.player;
using com.game.player.itemsystemextensions;
using com.game.shopsystem;
using com.game.statsystem;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
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
        [SerializeField] private TMP_Text m_statText;
        [SerializeField, InlineEditor] private ItemDisplay m_itemDisplayPrefab;

        public event Action<PlayerItemProfile> OnItemBoughtOneShot;

        IShop<PlayerItemProfile> m_shop;
        PlayerStats m_stats;
        PlayerMoneyLogic m_money;
        PlayerInventory m_inventory;

        List<ItemDisplay> m_currentDisplays = new();

        private void Start()
        {
            m_shop = Player.Instance.Hub.Shop;
            m_stats = Player.Instance.Hub.Stats;
            m_shop.OnReroll += OnShopReroll;
            m_inventory = Player.Instance.Hub.Inventory;
            m_money = Player.Instance.Hub.Money;

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
            }

            RefreshAll();
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

            m_money.Spend(playerItem.Price);
            m_inventory.Add(ItemObject.Create(item));
            display.buttonInteractabilityProvider = (_) => false;
            display.CanvasGroup.alpha = 0f;

            InvokeOnItemBought(playerItem);
            RefreshAll();
        }

        private void RefreshStatText()
        {
            StringBuilder sb = new();
            m_stats.Manipulator.ForAllStatEntries((key, value) =>
            {
                float diff = value;
                float refinedValue = m_stats.GetStat(key);

                float refinedDiff = refinedValue;
                string colorName;

                if (diff > 0f) colorName = "green";
                else if (diff == 0f) colorName = "white";
                else colorName = "red";

                string valueLabel = utilities.Helpers.Text.Colorize(value.ToString("0"), colorName);

                if (refinedDiff > 0f) colorName = "green";
                else if (refinedDiff == 0f) colorName = "white";
                else colorName = "red";

                string refinedValueLabel = utilities.Helpers.Text.Colorize($" ({refinedValue.ToString("0.00")})", colorName);

                sb.Append(utilities.Helpers.Text.Bold($"{StatSystemHelpers.Text.GetDisplayName(key, true)}: " +
                valueLabel + refinedValueLabel));
                sb.Append("\n");
            });

            m_statText.text = sb.ToString();
        }

        private string GetItemButtonText(PlayerItemProfile item)
        {
            StringBuilder sb = new("Buy ");

            int price = item.Price;
            bool canAfford = m_money.CanAfford(price);
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

            RefreshStatText();
        }

        public void Clear()
        {
            m_currentDisplays.Clear();
            m_stand.DestroyChildren();
        }
    }
}
