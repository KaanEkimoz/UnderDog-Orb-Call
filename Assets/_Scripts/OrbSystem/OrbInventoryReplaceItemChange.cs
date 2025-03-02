using com.game.itemsystem;

namespace com.game.orbsystem
{
    public class OrbInventoryReplaceItemChange : OrbInventoryChange
    {
        ItemObject m_itemRemoved;

        public OrbInventoryReplaceItemChange(SimpleOrb orb, OrbInventory inventory, ItemObject itemRemoved) : base(orb, inventory)
        {
            m_itemRemoved = itemRemoved;
        }

        public override void Undo()
        {
            m_inventory.RemoveCurrentElement();
            m_inventory.Add(m_itemRemoved);
        }
    }
}
