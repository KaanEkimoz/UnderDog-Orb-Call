using com.game.itemsystem;
using com.game.itemsystem.scriptables;
using com.game.orbsystem.itemsystemextensions;
using com.game.orbsystem.statsystemextensions;
using com.game.statsystem;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace com.game.orbsystem
{
    [System.Serializable]
    public class OrbInventory : IInventory<ItemObject>
    {
        ItemObject m_currentItem;
        OrbStats m_stats;

        List<ModifierObject<OrbStatType>> m_modifiers;

        public ItemObject CurrentItem => m_currentItem;

        public OrbInventory(OrbStats stats)
        {
            m_stats = stats;
            m_modifiers = new();
            m_currentItem = null;
        }

        public bool Add(ItemObject target)
        {
            if (target.Profile is not OrbItemProfile profile) return false;

            if (m_currentItem == null)
            {
                m_currentItem = target;
                ApplyStatModifiers(target, profile);
                return true;
            }

            if (!ItemRecipeManager.Exists(m_currentItem.Profile, profile, out ItemRecipeProfile recipeProfile)) 
                return false;

            if (ItemManager.GetItem(recipeProfile.ResultGuid) is not OrbItemProfile resultProfile)
                return false;

            RemoveCurrentElement();

            m_currentItem = ItemObject.Create(resultProfile);
            ApplyStatModifiers(m_currentItem, resultProfile);
            return true;
        }

        private void ApplyStatModifiers(ItemObject targetItem, OrbItemProfile profile)
        {
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

            ItemActionDispatcher.DispatchAll(targetItem);
        }

        void RevertCurrentModifiers() 
        {
            m_modifiers.ForEach(mod =>
            {
                m_stats.Manipulator.Demodify(mod);
            });

            m_modifiers.Clear();

            // simply can not revert overrides, so...
        }

        [Obsolete]
        public void Remove(ItemObject target)
        {
            RemoveCurrentElement();
        }

        public void RemoveCurrentElement()
        {
            if (m_currentItem == null)
                return;

            RevertCurrentModifiers();
            m_currentItem = null;
        }

        public Sprite GetIcon()
        {
            if (m_currentItem == null) 
                return null;

            return m_currentItem.Profile.Icon;
        }

        public string GenerateDescription()
        {
            if (m_currentItem == null) 
                return "No elements.";

            StringBuilder sb = new();
            sb.Append(m_currentItem.Profile.DisplayName);
            sb.Append("\n\n");
            sb.Append(ItemSystemHelpers.Text.GenerateDescription(m_currentItem, false));
            return sb.ToString();
        }
    }
}
