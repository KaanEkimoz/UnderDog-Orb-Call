using com.game.itemsystem.scriptables;
using com.game.ui.gamedependent.datatypes;

namespace com.game.ui.gamedependent.internals
{
    public static class ExtensionMethods
    {
        public static ItemUIData GenerateUIData(this ItemProfileBase itemSO)
        {
            return new ItemUIData()
            {
                Icon = itemSO.Icon,
                DisplayName = itemSO.DisplayName,
                Description = itemSO.Description,
            };
        }
    }
}
