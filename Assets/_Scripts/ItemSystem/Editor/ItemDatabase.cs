using com.game.itemsystem.scriptables;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace com.game.itemsystem
{
    public static class ItemDatabase
    {
        private static Dictionary<string, ItemProfile> s_entries;
        public static Dictionary<string, ItemProfile> Entries => s_entries;

        public static async void InitializeAsync()
        {
            await Task.Delay(1000);

            /*
                NO LOGIC SET.
            */
        }

        public static ItemProfile GetItem(string guid)
        {
            return null;

            /*
                NO LOGIC SET.
            */
        }

        public static bool TryGetItem(string guid, out ItemProfile result)
        {
            result = null;
            return false;

            /*
                NO LOGIC SET.
            */
        }
    }
}
