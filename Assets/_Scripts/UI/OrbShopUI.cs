using com.absence.attributes.experimental;
using com.absence.utilities;
using com.game.orbsystem.itemsystemextensions;
using com.game.player;
using com.game.shopsystem;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.game.ui
{
    public class OrbShopUI : MonoBehaviour
    {
        [SerializeField] private GameObject m_panel;
        [SerializeField] private RectTransform m_stand;
        [SerializeField] private TMP_Text m_title;
        [SerializeField] private Button m_inventoryButton;
        [SerializeField] private Button m_rerollButton;
        [SerializeField] private Button m_passButton;
        [SerializeField, InlineEditor] private ItemDisplay m_itemDisplayPrefab;

        IShop<OrbItemProfile> m_shop;

        public event Action<OrbItemProfile> OnItemBought;
        public event Action<OrbItemProfile> OnItemBoughtOneShot;

        public TMP_Text Title => m_title;

        private void Start()
        {
            m_shop = Player.Instance.Hub.OrbShop;
            m_shop.OnReroll += OnShopReroll;

            Hide(true);
            SetupButton(m_rerollButton, SoftReroll);
        }

        public void SetVisibility(bool visibility)
        {
            m_panel.SetActive(visibility);
            SetupButtons(null, null);
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

        public void SetupButtons(Action inventoryButton, Action passButton)
        {
            SetupButton(m_inventoryButton, inventoryButton);
            SetupButton(m_passButton, passButton);
        }

        void OnShopReroll(IShop<OrbItemProfile> shop)
        {
            GenerateDisplay(shop);
        }

        public void SoftReroll()
        {
            m_shop.Reroll();
        }

        public virtual void GenerateDisplay(IShop<OrbItemProfile> shop)
        {
            Clear();
            foreach (OrbItemProfile item in shop.ItemsOnStand)
            {
                ItemDisplay display = Instantiate(m_itemDisplayPrefab, m_stand);
                display.Initialize(item);
                display.SetButtonText("Get");
                display.SetupButton(() =>
                {
                    InvokeOnGet(item);
                });
            }
        }

        private void InvokeOnGet(OrbItemProfile itemInContext)
        {
            OnItemBought?.Invoke(itemInContext);
            OnItemBoughtOneShot?.Invoke(itemInContext);
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

        public void Clear()
        {
            m_stand.DestroyChildren();
        }
    }
}
