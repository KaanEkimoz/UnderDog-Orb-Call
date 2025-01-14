using com.game.itemsystem.gamedependent;

namespace com.game.itemsystem
{
    public static class ItemSystemHelpers
    {
        public static class Text
        {
            public static string GenerateActionDescription(ItemCustomAction action, bool richText)
            {
                object[] args = ItemActionCorrelator.GetArgs(action);

                //string targetDesc = richText ? action.DescriptionForRichText : action.Description;
                string targetDesc = action.Description;

                if (args == null) return targetDesc + " (args null)";

                return string.Format(targetDesc, args);
            }
        }
    }
}
