using com.absence.utilities;
using com.game.orbsystem.itemsystemextensions;
using com.game.player;
using com.game.shopsystem;
using System;
using UnityEngine;

namespace com.game.ui
{
    public class OrbShopUI : Singleton<OrbShopUI>
    {
        [SerializeField] private ItemDisplay m_itemDisplayPrefab;
        [SerializeField] private GameObject m_panel;
        [SerializeField] private RectTransform m_stand;

        IShop<OrbItemProfile> m_shop;

        public event Action<OrbItemProfile> OnItemBoughtOneShot;

        protected override void Awake()
        {
            base.Awake();

            m_shop = Player.Instance.Hub.OrbShop;
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
            OnItemBoughtOneShot?.Invoke(itemInContext);
            OnItemBoughtOneShot = null;
        }

        public void Clear()
        {
            m_stand.DestroyChildren();
        }
    }
}
