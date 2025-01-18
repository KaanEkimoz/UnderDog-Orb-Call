using com.game.itemsystem.scriptables;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace com.game.itemsystem
{
    public static class ItemDatabase
    {
        private static Dictionary<string, ItemProfileBase> s_entries;
        public static Dictionary<string, ItemProfileBase> Entries => s_entries;

        public static async void InitializeAsync()
        {
            await Task.Delay(1000);

            /*
                NO LOGIC SET.
            */
        }

        public static ItemProfileBase GetItem(string guid)
        {
            return null;

            /*
                NO LOGIC SET.
            */
        }

        public static bool TryGetItem(string guid, out ItemProfileBase result)
        {
            result = null;
            return false;

            /*
                NO LOGIC SET.
            */
        }
    }
}
