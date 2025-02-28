using com.absence.utilities;
using com.game.orbsystem;
using com.game.player;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.ui
{
    public class OrbContainerUI : Singleton<OrbContainerUI>
    {
        [SerializeField] private GameObject m_panel;
        [SerializeField] private RectTransform m_pivot;
        [SerializeField] private OrbDisplay m_displayPrefab;
        [SerializeField] private float m_diameter;
        [SerializeField] private float m_descriptionPanelOffset;

        PlayerOrbContainer m_container;
        Dictionary<SimpleOrb, OrbDisplay> m_displays = new();

        private void Start()
        {
            m_container = Player.Instance.Hub.OrbContainer;
        }

        public void SetVisibility(bool visibility)
        {
            m_panel.SetActive(visibility);
        }

        public void Show(bool refresh = false)
        {
            if (refresh) SoftRefresh();
            SetVisibility(true);
        }

        public void Hide(bool clear = false)
        {
            SetVisibility(false);
            if (clear) Clear();
        }

        public void SoftRedraw()
        {
            foreach (KeyValuePair<SimpleOrb, OrbInventory> kvp in m_container.OrbInventoryEntries)
            {
                SimpleOrb orb = kvp.Key;
                OrbInventory inventory = kvp.Value;

                m_displays[orb].Initialize(orb, inventory);
            }
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
                Vector2 descriptionPanelPosition = direction * (m_diameter + m_descriptionPanelOffset);

                OrbDisplay display = GameObject.Instantiate(m_displayPrefab);
                display.transform.SetParent(m_pivot, false);
                display.transform.localPosition = position;

                display.Initialize(orb, inventory);
                if (display.DescriptionPanel != null)
                {
                    display.DescriptionPanel.SetParent(m_pivot, false);
                    //display.DescriptionPanel.anchorMin = -direction;
                    //display.DescriptionPanel.anchorMax = -direction;
                    display.DescriptionPanel.anchoredPosition = descriptionPanelPosition;
                }

                m_displays.Add(orb, display);

                index++;
            }
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
