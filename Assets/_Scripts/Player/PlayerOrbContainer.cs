using com.game.orbsystem;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.player
{
    public class PlayerOrbContainer : MonoBehaviour
    {
        [SerializeField] private OrbController m_targetController;

        Dictionary<SimpleOrb, OrbInventory> m_orbInventoryEntries = new();

        public Dictionary<SimpleOrb, OrbInventory> OrbInventoryEntries => m_orbInventoryEntries;

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
    }
}
