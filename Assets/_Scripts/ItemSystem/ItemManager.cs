using com.absence.utilities.experimental.databases;
using com.game.itemsystem.scriptables;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.game.itemsystem
{
    public static class ItemManager
    {
        public const bool ENABLED = true;
        public const string ITEM_PROFILE_TAG = "itemprofile";

        static IDatabaseInstance<string, ItemProfileBase> s_instance;
        public static IDatabaseInstance<string, ItemProfileBase> Instance => s_instance;

#pragma warning disable CS0162 // Unreachable code detected
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Initalize()
        {
            if (!ENABLED)
                return;

            if (s_instance != null)
                s_instance.Dispose();

            s_instance = new AddressablesDatabaseInstance<string, ItemProfileBase>(ITEM_PROFILE_TAG, false);
            s_instance.Refresh();
        }
#pragma warning restore CS0162 // Unreachable code detected

        public static IEnumerable<T> GetItemsOfType<T>() where T : ItemProfileBase
        {
            return s_instance.Values.Where(value => value.GetType().Equals(typeof(T)))
                .ToList().ConvertAll(value => value as T);
        }
    }
}
