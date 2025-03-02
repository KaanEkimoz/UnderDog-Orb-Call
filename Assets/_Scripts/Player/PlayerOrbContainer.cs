using com.game.itemsystem;
using com.game.orbsystem;
using com.game.orbsystem.itemsystemextensions;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.player
{
    public class PlayerOrbContainer : MonoBehaviour
    {
        [SerializeField] private OrbController m_targetController;

        Dictionary<SimpleOrb, OrbInventory> m_orbInventoryEntries = new();
        List<OrbItemProfile> m_upgradeCache;
        Stack<OrbInventoryChange> m_undoCache = new();

        public Dictionary<SimpleOrb, OrbInventory> OrbInventoryEntries => m_orbInventoryEntries;
        public List<OrbItemProfile> UpgradeCache { get { return m_upgradeCache; } set { m_upgradeCache = value; } }
        public Stack<OrbInventoryChange> UndoCache { get { return m_undoCache; } set { m_undoCache = value; } }

        public void SetUpgradeCache(IEnumerable<OrbItemProfile> enumerable)
        {
            if (enumerable == null)
            {
                m_upgradeCache = null;
                return;
            }

            m_upgradeCache = new List<OrbItemProfile>(enumerable);
        }

        public void Refresh()
        {
            Dictionary<SimpleOrb, OrbInventory> temp = new();
            foreach (SimpleOrb orb in m_targetController.OrbsOnEllipse)
            {
                if (m_orbInventoryEntries.TryGetValue(orb, out OrbInventory inventory))
                    temp.Add(orb, inventory);
                else
                    temp.Add(orb, new OrbInventory(orb.Stats));
            }

            m_orbInventoryEntries = temp;
        }

        public void UndoAll()
        {
            while(m_undoCache.TryPop(out OrbInventoryChange lastChange))
            {
                lastChange.Undo();
            }

            m_undoCache.Clear();
        }

        public bool ApplyUpgrade(OrbItemProfile upgrade, SimpleOrb target)
        {
            if (!m_orbInventoryEntries.TryGetValue(target, out OrbInventory inventory))
                return false;

            OrbInventoryChange undo = new OrbInventoryAddItemChange(target, inventory);
            bool success = inventory.Add(ItemObject.Create(upgrade));

            if (success)
            {
                m_undoCache.Push(undo);
            }

            return success;
        }
    }
}
