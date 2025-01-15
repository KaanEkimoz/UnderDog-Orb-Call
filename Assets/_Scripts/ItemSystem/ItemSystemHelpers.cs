using com.game.itemsystem.gamedependent;

namespace com.game.itemsystem
{
    public static class ItemSystemHelpers
    {
        public static class Text
        {
            public static string GenerateDescription(ItemCustomAction action, bool richText)
            {
                switch (action.ActionType)
                {
                    case ItemActionType.None:
                        return null;
                    case ItemActionType.SpawnItemBehaviour:
                        return action.ItemBehaviour.GenerateDescription(richText);
                    default:
                        return null;
                }
            }
        }
    }
}
