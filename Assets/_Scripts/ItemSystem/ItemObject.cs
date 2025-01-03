using com.game.itemsystem.scriptables;
using System.Collections.Generic;

namespace com.game.itemsystem
{
    /// <summary>
    /// The reference type used for holding runtime data of an item instance.
    /// </summary>
    [System.Serializable]
    public class ItemObject
    {
        public ItemProfile Profile;
        public Dictionary<string, object> CustomData = new();
    }
}
