namespace com.game.orbsystem
{
    public abstract class OrbInventoryChange
    {
        protected SimpleOrb m_target;
        protected OrbInventory m_inventory;

        public OrbInventoryChange(SimpleOrb orb, OrbInventory inventory)
        {
            m_target = orb;
            m_inventory = inventory;
        }

        public abstract void Undo();
    }
}
