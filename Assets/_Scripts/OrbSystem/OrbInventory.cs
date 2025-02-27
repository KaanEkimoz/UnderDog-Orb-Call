using com.game.itemsystem;
using com.game.orbsystem.itemsystemextensions;
using com.game.orbsystem.statsystemextensions;
using com.game.statsystem;
using System;
using System.Collections.Generic;

namespace com.game.orbsystem
{
    [System.Serializable]
    public class OrbInventory : IInventory<ItemObject>
    {
        ItemObject m_currentItem;
        OrbItemProfile m_currentElement;
        OrbStats m_stats;

        List<ModifierObject<OrbStatType>> m_modifiers;

        public OrbInventory(OrbStats stats)
        {
            m_stats = stats;
            m_modifiers = new();
            m_currentItem = null;
            m_currentElement = null;
        }

        public bool Add(ItemObject target)
        {
            if (target.Profile is not OrbItemProfile profile) return false;

            if (m_currentElement == null)
            {
                m_currentItem = target;
                m_currentElement = profile;
                ApplyStatModifiers(target, profile);
                return true;
            }

            if (!ItemSystemHelpers.Recipes.TryCombine<OrbItemProfile>(m_currentElement, profile, out OrbItemProfile sumProfile)) 
                return false;

            RemoveCurrentElement();

            m_currentElement = sumProfile;
            m_currentItem = ItemObject.Create(sumProfile);
            ApplyStatModifiers(m_currentItem, m_currentElement);
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
            if (m_currentElement == null)
                return;

            RevertCurrentModifiers();
            m_currentElement = null;
        }
    }
}
