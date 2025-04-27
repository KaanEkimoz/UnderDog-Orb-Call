using com.absence.consolesystem;
using com.absence.consolesystem.internals;
using com.game.player;
using com.game.player.statsystemextensions;

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
        public static void GiveItem()
        {

        }

        [Command]
        public static void GiveElement()
        {

        }
    }
}
