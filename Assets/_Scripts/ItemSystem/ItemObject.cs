using com.game.itemsystem.scriptables;
using com.game.statsystem.presetobjects;
using System;
using System.Collections.Generic;

namespace com.game.itemsystem
{
    /// <summary>
    /// The reference type used for holding runtime data of an item instance.
    /// </summary>
    [System.Serializable]
    public class ItemObject : IDisposable
    {
        public ItemProfile Profile;
        public Dictionary<string, object> CustomData = new();

        public event Action OnDispose = null;

        public void Dispose()
        {
            Profile = null;
            CustomData = null;
            OnDispose?.Invoke();
        }

        public static ItemObject Create(ItemProfile profile)
        {
            return new ItemObject()
            {
                Profile = profile,
                CustomData = new(),
            };
        }

        public static ItemObject Create(string guid)
        {
            //ItemProfile profile = ItemDatabase.GetItem(guid);

            //return new ItemObject()
            //{
            //    Profile = profile,
            //    CustomData = new(),
            //};

            return null;
        }
    }
}
