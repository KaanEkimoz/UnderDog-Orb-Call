using com.game.itemsystem;

namespace com.game.orbsystem
{
    public class OrbInventoryAddItemChange : OrbInventoryChange
    {
        ItemObject m_previousItem;

        public OrbInventoryAddItemChange(SimpleOrb orb, OrbInventory inventory) : base(orb, inventory)
        {
            m_previousItem = inventory.CurrentItem;
        }

        public override void Undo()
        {
            m_inventory.RemoveCurrentElement();

            if (m_previousItem == null)
                return;

            m_inventory.Add(m_previousItem);
        }
    }
}
