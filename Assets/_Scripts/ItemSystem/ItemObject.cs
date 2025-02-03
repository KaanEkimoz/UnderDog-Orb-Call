using com.game.itemsystem.scriptables;
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
        public ItemProfileBase Profile;
        public Dictionary<string, object> CustomData = new();

        public event Action OnDispose = null;

        public static ItemObject Create(ItemProfileBase profile)
        {
            return new ItemObject()
            {
                Profile = profile,
                CustomData = new(),
            };
        }

        public static ItemObject Create(string guid)
        {
            //ItemProfileBase profile = IteMMana.GetItem(guid);

            //return new ItemObject()
            //{
            //    Profile = profile,
            //    CustomData = new(),
            //};

            return null;
        }

        public void Dispose()
        {
            Profile = null;
            CustomData = null;
            OnDispose?.Invoke();
        }
    }
}
