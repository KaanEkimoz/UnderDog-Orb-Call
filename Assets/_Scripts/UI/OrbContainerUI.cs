using com.absence.attributes.experimental;
using com.absence.utilities;
using com.game.orbsystem;
using com.game.orbsystem.itemsystemextensions;
using com.game.player;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.game.ui
{
    public class OrbContainerUI : MonoBehaviour
    {
        [Header("Utilities")]
        [SerializeField] private GameObject m_panel;
        [SerializeField] private RectTransform m_pivot;
        [SerializeField] private GameObject m_descriptionPanel;
        [SerializeField] private TMP_Text m_descriptionText;
        [SerializeField] private Button m_backButton;
        [SerializeField] private Button m_resetButton;
        [SerializeField] private Button m_confirmButton;
        [SerializeField, InlineEditor] private OrbDisplay m_displayPrefab;

        [Space, Header("Settings")]

        [SerializeField] private float m_diameter;
        [SerializeField] private bool m_constantDescription;

        PlayerOrbContainer m_container;
        Dictionary<SimpleOrb, OrbDisplay> m_displays = new();

        SimpleOrb m_hoveredOrb;
        List<OrbItemProfile> m_upgradeCache;

        bool m_undoable;

        private void Start()
        {
            m_container = Player.Instance.Hub.OrbContainer;
            if (!m_constantDescription) InitializeDescriptionPanel(null);

            Hide(true);
            SetupButton(m_confirmButton, ConfirmChanges);
        }

        public void SetUpgradeCache(IEnumerable<OrbItemProfile> enumerable)
        {
            if (enumerable == null)
            {
                m_upgradeCache = null;
                return;
            }

            m_upgradeCache = new List<OrbItemProfile>(enumerable);
        }

        public void SetVisibility(bool visibility)
        {
            m_panel.SetActive(visibility);
            SetupButtons(null, null);
            SetupButton(m_confirmButton, ConfirmChanges);
            m_undoable = false;
            RefreshButtonStates();
        }

        public void Show(bool refresh = false)
        {
            if (refresh) SoftRefresh();
            SetVisibility(true);
        }

        void RefreshButtonStates()
        {
            m_resetButton.gameObject.SetActive(m_upgradeCache != null);
            m_confirmButton.gameObject.SetActive(m_upgradeCache != null);
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

                if (!m_constantDescription) 
                    display.onPointerExit += (_) => InitializeDescriptionPanel(null);

                m_displays.Add(orb, display);

                index++;
            }

            RefreshButtonStates();
        }

        void ConfirmChanges()
        {
            Debug.Log("yipee!!!");
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
            m_pivot.DestroyChildren();
            m_displays.Clear();
        }
    }
}
