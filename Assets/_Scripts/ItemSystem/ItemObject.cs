using com.game.itemsystem.scriptables;
using System;
using System.Collections.Generic;

namespace com.game.itemsystem
{
    [System.Serializable]
    public abstract class ItemObject : IDisposable
    {
        public static ItemObject<T> Create<T>(T profile) where T : ItemProfileBase
        {
            return new ItemObject<T>(profile);
        }

        public static ItemObject<T> Create<T>(string guid) where T : ItemProfileBase
        {
            return new ItemObject<T>(guid);
        }

        public ItemProfileBase Profile { get; }
        public Dictionary<string, object> CustomData = new();

        public event Action OnDispose = null;

        public ItemObject(ItemProfileBase profile)
        {
            Profile = profile;
        }

        public ItemObject(string guid)
        {
            Profile = ItemManager.GetItem(guid);
        }

        protected void InvokeOnDispose()
        {
            OnDispose?.Invoke();
        }
        public abstract void Dispose();
    }

    /// <summary>
    /// The reference type used for holding runtime data of an item instance.
    /// </summary>
    [System.Serializable]
    public class ItemObject<T> : ItemObject where T : ItemProfileBase
    {
        public new T Profile { get; private set; }

        public ItemObject(T profile) : base(profile)
        {
            if (profile == null)
                throw new Exception("Provided item profile for ItemObject is null.");

            Profile = profile;
            CustomData = new();
        }

        public ItemObject(string guid) : base(guid)
        {
            T profile = ItemManager.GetItem<T>(guid);

            if (profile == null)
                throw new Exception("Provided item profile for ItemObject is null.");

            Profile = profile;
            CustomData = new();
        }

        public override void Dispose()
        {
            Profile = null;
            CustomData = null;
            InvokeOnDispose();
        }
    }
}
