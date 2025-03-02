using com.absence.attributes.experimental;
using com.absence.utilities;
using com.game.itemsystem;
using com.game.itemsystem.scriptables;
using com.game.orbsystem;
using com.game.orbsystem.itemsystemextensions;
using com.game.player;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.game.ui
{
    public class OrbContainerUI : MonoBehaviour
    {
        [Header("Utilities")]
        [SerializeField] private GameObject m_panel;
        [SerializeField] private GameObject m_cachePanel;
        [SerializeField] private RectTransform m_pivot;
        [SerializeField] private RectTransform m_cacheStand;
        [SerializeField] private GameObject m_descriptionPanel;
        [SerializeField] private TMP_Text m_cacheDescriptionText;
        [SerializeField] private TMP_Text m_descriptionText;
        [SerializeField] private Button m_backButton;
        [SerializeField] private Button m_resetButton;
        [SerializeField] private Button m_confirmButton;
        [SerializeField, InlineEditor] private OrbDisplay m_displayPrefab;
        [SerializeField, InlineEditor] private ItemDisplay m_compactDisplayPrefab;

        [Space, Header("Settings")]

        [SerializeField] private float m_diameter;
        [SerializeField] private bool m_constantDescription;

        PlayerOrbContainer m_container;
        Dictionary<SimpleOrb, OrbDisplay> m_displays = new();
        List<ItemDisplay> m_upgradeDisplays = new();

        ItemDisplay m_hoveredUpgradeDisplay;
        ItemDisplay m_selectedUpgradeDisplay;

        SimpleOrb m_hoveredOrb;

        bool m_undoable;

        private void Start()
        {
            m_container = Player.Instance.Hub.OrbContainer;
            if (!m_constantDescription) InitializeDescriptionPanel(null);

            Hide(true);
            SetupButton(m_confirmButton, ConfirmChanges);
            SetupButton(m_resetButton, ResetChanges);
        }

        public void SetUpgradeCache(IEnumerable<OrbItemProfile> enumerable)
        {
            m_container.SetUpgradeCache(enumerable);
        }

        public void SetVisibility(bool visibility)
        {
            m_panel.SetActive(visibility);
            SetupButtons(null, null);
            SetupButton(m_confirmButton, ConfirmChanges);
            RefreshButtonStates();
        }

        public void Show(bool refresh = false)
        {
            if (refresh) SoftRefresh();
            SetVisibility(true);
        }

        void RefreshButtonStates()
        {
            m_resetButton.gameObject.SetActive(m_container.UpgradeCache != null);
            m_confirmButton.gameObject.SetActive(m_container.UpgradeCache != null);
            m_resetButton.interactable = m_undoable;
        }

        public void Hide(bool clear = false)
        {
            SetVisibility(false);
            if (clear) Clear();
        }

        public void SetupButtons(Action backButton, Action confirmButton)
        {
            SetupButton(m_backButton, backButton);
            SetupButton(m_confirmButton, confirmButton);
        }

        public void SetupButtons(Action backButton)
        {
            SetupButton(m_backButton, backButton);
        }

        public void SoftRedraw()
        {
            foreach (KeyValuePair<SimpleOrb, OrbInventory> kvp in m_container.OrbInventoryEntries)
            {
                SimpleOrb orb = kvp.Key;
                OrbInventory inventory = kvp.Value;

                m_displays[orb].Initialize(orb, inventory);
            }

            InitializeDescriptionPanel(m_hoveredOrb);
            RefreshButtonStates();
        }

        public void HardRedraw()
        {
            Clear();
            DrawOrbs();
            DrawUpgradeCache();
            RefreshButtonStates();
        }

        void DrawOrbs()
        {
            int count = m_container.OrbInventoryEntries.Count;
            float stepAngle = 360f / count;
            int index = 0;
            foreach (KeyValuePair<SimpleOrb, OrbInventory> kvp in m_container.OrbInventoryEntries)
            {
                SimpleOrb orb = kvp.Key;
                OrbInventory inventory = kvp.Value;

                float totalAngle = index * stepAngle;
                float cos = Mathf.Cos(totalAngle * Mathf.Deg2Rad);
                float sin = Mathf.Sin(totalAngle * Mathf.Deg2Rad);
                Vector2 direction = new Vector2(sin, cos);
                Vector2 position = direction * m_diameter;

                OrbDisplay display = GameObject.Instantiate(m_displayPrefab);
                display.transform.SetParent(m_pivot, false);
                display.transform.localPosition = position;

                display.Initialize(orb, inventory);

                display.onPointerEnter += InitializeDescriptionPanel;
                display.onPointerClick += OnOrbDisplayClick;

                if (!m_constantDescription)
                    display.onPointerExit += (_) => InitializeDescriptionPanel(null);

                m_displays.Add(orb, display);

                index++;
            }
        }

        private void OnOrbDisplayClick(SimpleOrb orb)
        {
            if (m_selectedUpgradeDisplay == null)
                return;

            bool success = m_container.ApplyUpgrade(m_selectedUpgradeDisplay.Profile as OrbItemProfile, orb);

            if (!success)
                return;

            m_displays[orb].Refresh();

            m_undoable = true;

            UnselectCurrentUpgrade(true);

            RefreshButtonStates();
            InitializeDescriptionPanel(orb);
        }

        void UnselectCurrentUpgrade(bool discard)
        {
            if (m_selectedUpgradeDisplay == null)
                return;

            m_selectedUpgradeDisplay.UnlockOutline();
            if (discard) m_selectedUpgradeDisplay.Interactable = false;

            m_selectedUpgradeDisplay = null;
        }

        void ResetChanges()
        {
            m_undoable = false;
            m_container.UndoAll();

            RefreshButtonStates();
            if (m_hoveredOrb != null) InitializeDescriptionPanel(m_hoveredOrb);

            UnselectCurrentUpgrade(false);

            FetchOrbIcons();
            RefreshUpgradeDescription();

            foreach (ItemDisplay display in m_upgradeDisplays)
            {
                display.Interactable = true;
            }
        }

        void DrawUpgradeCache()
        {
            if (m_container.UpgradeCache == null || m_container.UpgradeCache.Count == 0)
            {
                m_cachePanel.SetActive(false);
                return;
            }

            m_cachePanel.SetActive(true);

            foreach (OrbItemProfile upgrade in m_container.UpgradeCache) 
            {
                ItemDisplay display = Instantiate(m_compactDisplayPrefab, m_cacheStand);
                display.Initialize(upgrade);
                display.onPointerEnter += OnHoverUpgrade;
                display.onPointerExit += (_) => OnHoverUpgrade(null);
                display.onPointerClick += OnSelectUpgrade;

                m_upgradeDisplays.Add(display);
            }
        }

        private void FetchOrbIcons()
        {
            foreach (OrbDisplay display in m_displays.Values)
            {
                display.Refresh();
            }
        }

        private void OnSelectUpgrade(ItemDisplay display)
        {
            UnselectCurrentUpgrade(false);

            if (m_selectedUpgradeDisplay != null && m_selectedUpgradeDisplay.Equals(display))
                m_selectedUpgradeDisplay = null;
            else
                m_selectedUpgradeDisplay = display;

            if (m_selectedUpgradeDisplay != null)
            {
                m_selectedUpgradeDisplay.LockOutline();
                m_selectedUpgradeDisplay.CanvasGroup.alpha = 0.7f;
            }

            RefreshUpgradeDescription();
        }

        private void OnHoverUpgrade(ItemDisplay display)
        {
            m_hoveredUpgradeDisplay = display;

            RefreshUpgradeDescription();
        }

        private void RefreshUpgradeDescription()
        {
            if (m_hoveredUpgradeDisplay == null)
                m_cacheDescriptionText.text = 
                    m_selectedUpgradeDisplay != null ?
                    GenerateFullDescription(m_selectedUpgradeDisplay.Profile) :
                    string.Empty;
            else
                m_cacheDescriptionText.text = GenerateFullDescription(m_hoveredUpgradeDisplay.Profile);
        }

        private string GenerateFullDescription(ItemProfileBase profile)
        {
            StringBuilder sb = new();
            sb.Append(profile.DisplayName);
            sb.Append("\n\n");
            sb.Append(ItemSystemHelpers.Text.GenerateDescription(profile, false));
            return sb.ToString();
        }

        void ConfirmChanges()
        {
            m_container.ClearUndoHistory();
        }

        void InitializeDescriptionPanel(SimpleOrb orb)
        {
            if (m_hoveredOrb != null)
                m_displays[m_hoveredOrb].SetOutlineVisibility(false);

            if (orb == null)
            {
                m_hoveredOrb = null;
                m_descriptionPanel.SetActive(false);
                return;
            }

            m_hoveredOrb = orb;
            m_displays[orb].SetOutlineVisibility(true);
            m_descriptionPanel.SetActive(true);
            m_descriptionText.text = m_container.OrbInventoryEntries[orb].GenerateDescription();
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

        public void SoftRefresh()
        {
            m_container.Refresh();
            HardRedraw();
        }

        public void Clear()
        {
            m_selectedUpgradeDisplay = null;
            m_hoveredUpgradeDisplay = null;

            m_cacheStand.DestroyChildren();
            m_pivot.DestroyChildren();
            m_displays.Clear();
            m_upgradeDisplays.Clear();
        }
    }
}
