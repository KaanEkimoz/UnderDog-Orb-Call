using com.game.itemsystem;
using com.game.player.itemsystemextensions;
using com.game.player.statsystemextensions;
using com.game.statsystem;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.player
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private List<PlayerItemProfile> m_itemBank = new();

        [SerializeField] private List<ItemObject> m_items = new();

        [SerializeField]
        private Dictionary<ItemObject, List<ModifierObject<PlayerStatType>>>
            m_itemModifierEntries = new();

        string m_labelText = "No items selected.\n\nSelect an item to display its description here.";
        ItemObject m_selectedItem;

        PlayerStats m_stats;

        private void Start()
        {
            m_stats = Player.Instance.Hub.Stats;
        }

        public bool AddItem(ItemObject itemToAdd)
        {
            if (itemToAdd.Profile is not PlayerItemProfile profile) return false;
            if (m_items.Contains(itemToAdd)) return false;

            ApplyItemModifiers(itemToAdd, profile);
            m_items.Add(itemToAdd);

            return true;
        }

        public void RemoveItem(ItemObject itemToRemove)
        {
            if (!m_items.Contains(itemToRemove)) return;

            m_items.Remove(itemToRemove);
            RevertItemModifiers(itemToRemove);
        }

        void ApplyItemModifiers(ItemObject targetItem, PlayerItemProfile profile)
        {
            List<ModifierObject<PlayerStatType>> m_modifiers = new();

            profile.StatModifications.ForEach(mod =>
            {
                m_modifiers.Add(m_stats.Manipulator.ModifyWith(mod));
            });

            profile.StatCaps.ForEach(cap =>
            {
                m_modifiers.Add(m_stats.Manipulator.CapWith(cap));
            });

            profile.StatOverrides.ForEach(ovr =>
            {
                m_stats.Manipulator.OverrideWith(ovr);
            });

            m_itemModifierEntries.Add(targetItem, m_modifiers);
            ItemActionDispatcher.DispatchAll(targetItem);
        }

        void RevertItemModifiers(ItemObject targetItem)
        {
            List<ModifierObject<PlayerStatType>> modifiers = m_itemModifierEntries[targetItem];

            modifiers.ForEach(mod =>
            {
                m_stats.Manipulator.Demodify(mod);
            });

            // simply can not revert overrides, so...

            targetItem.Dispose();
        }

#if UNITY_EDITOR
        const float k_totalGUIWidth = 340f;

        public void OnTestGUI()
        {
            GUILayout.BeginHorizontal(GUILayout.Width(k_totalGUIWidth));

            GUILayout.BeginVertical("box");

            GUILayout.Label("Shop");

            m_itemBank.ForEach(itemProfile =>
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button($"Buy {itemProfile.DisplayName}"))
                    AddItem(ItemObject.Create(itemProfile));

                if (GUILayout.Button("i"))
                    m_labelText = ItemSystemHelpers.Text.GenerateDescription(itemProfile, true);

                GUILayout.EndHorizontal();
            });

            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");

            GUILayout.Label("Inventory");

            List<ItemObject> itemsMarkedForRemoval = new();

            m_items.ForEach(itemObject =>
            {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button($"Sell {itemObject.Profile.DisplayName}"))
                    itemsMarkedForRemoval.Add(itemObject);

                if (GUILayout.Button("i"))
                    m_labelText = ItemSystemHelpers.Text.GenerateDescription(itemObject, true);

                GUILayout.EndHorizontal();
            });

            itemsMarkedForRemoval.ForEach(item => RemoveItem(item));

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.Label(m_labelText);
        }
#endif
    }
}
