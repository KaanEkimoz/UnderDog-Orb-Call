using com.absence.consolesystem;
using com.absence.consolesystem.internals;
using com.game.itemsystem;
using com.game.orbsystem;
using com.game.orbsystem.itemsystemextensions;
using com.game.player;
using com.game.player.itemsystemextensions;
using com.game.player.statsystemextensions;
using System.Collections;

namespace com.game
{
    public static class ConsoleGameCommands
    {
        [Command]
        public static void SetStat(string statName, float newValue)
        {
            if (!System.Enum.TryParse(typeof(PlayerStatType), statName, true, out object statTypeAsObject))
            {
                ConsoleWindow.Sender.LogError($"Player has no stat named: '{statName}'.");
                return;
            }

            PlayerStatOverride ovr = new()
            {
                TargetStatType = (PlayerStatType)(System.Enum)statTypeAsObject,
                NewValue = newValue,

            };

            Player.Instance.Hub.Stats.Manipulator.OverrideWith(ovr);
        }

        [Command]
        public static void BeDeprived()
        {
            foreach(PlayerStatType enumType in System.Enum.GetValues(typeof(PlayerStatType)))
            {
                PlayerStatOverride ovr = new()
                {
                    TargetStatType = enumType,
                    NewValue = 10f,
                };

                Player.Instance.Hub.Stats.Manipulator.OverrideWith(ovr);
            }
        }

        [Command]
        public static void GiveItem(string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
                return;

            PlayerItemProfile item = ItemManager.GetItemByCustomId<PlayerItemProfile>(itemId);

            if (item == null)
                return;

            Player.Instance.Hub.Inventory.Add(item);
        }

        [Command]
        public static void GiveElement(string elementId, int orbIndex)
        {
            if (string.IsNullOrWhiteSpace(elementId))
                return;

            OrbItemProfile item = ItemManager.GetItemByCustomId<OrbItemProfile>(elementId);

            if (item == null) 
                return;

            PlayerOrbContainer container = Player.Instance.Hub.OrbContainer;
            int totalOrbCount = container.Controller.orbsOnEllipse.Count;

            if (orbIndex < 0 || orbIndex >= totalOrbCount)
                return;

            SimpleOrb targetOrb = container.Controller.orbsOnEllipse[orbIndex];

            if (targetOrb == null)
                return;

            OrbInventory targetInventory = container.OrbInventoryEntries[targetOrb];

            if (targetInventory.CurrentItem != null)
                targetInventory.RemoveCurrentElement();

            targetInventory.Add(item);

            if (item.Prefab == null)
                return;

            bool swap = container.SwapOrb(targetOrb, item.Prefab);

            if (!swap)
                return;

            targetOrb.gameObject.SetActive(false);
        }

        [Command]
        public static void EarnMoney()
        {

        }
    }
}
