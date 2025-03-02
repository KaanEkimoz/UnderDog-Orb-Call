using com.game.itemsystem.scriptables;
using System.Text;

namespace com.game.itemsystem
{
    public static class ItemSystemHelpers
    {
        public static class Text
        {
            public static string GenerateDescription(ItemObject itemObject, bool richText)
            {
                return GenerateFullDescription_Internal(itemObject, itemObject.Profile, richText);
            }

            public static string GenerateDescription(ItemProfileBase itemProfile, bool richText)
            {
                return GenerateFullDescription_Internal(null, itemProfile, richText);
            }

            private static string GenerateFullDescription_Internal(ItemObject itemObject, ItemProfileBase itemProfile, bool richText)
            {
                string trimmedRawDesc = itemProfile.Description.Trim();

                StringBuilder sb = new(trimmedRawDesc);
                if (!string.IsNullOrWhiteSpace(trimmedRawDesc)) sb.Append("\n\n");

                string customActionDesc = GenerateCustomActionDescription_Internal(itemObject, itemProfile, richText);
                string furtherDesc = itemProfile.GenerateFurtherDescription(itemObject, richText);

                if (furtherDesc != null) sb.Append(furtherDesc);
                if (customActionDesc != null) sb.Append(customActionDesc);

                if (itemObject != null)
                {
                    string dataDesc = null;
                    if (itemObject.CustomData.TryGetValue(ItemBehaviour.CustomDataKey, out object value))
                    {
                        dataDesc =  (value as ItemBehaviour).GenerateDataDescription(richText);
                    }

                    if (dataDesc != null) sb.Append(dataDesc);
                }

                return sb.ToString();
            }

            private static string GenerateDescription_Internal(ItemObject context, ItemCustomAction action, bool richText)
            {
                return action.ItemBehaviour.GenerateActionDescription(richText);
            }

            private static string GenerateCustomActionDescription_Internal(ItemObject context, ItemProfileBase profile, bool richText)
            {
                StringBuilder sb = new();

                profile.CustomActions.ForEach(act =>
                {
                    sb.Append(ItemSystemHelpers.Text.GenerateDescription_Internal(context, act, richText));
                    sb.Append("\n");
                });

                return sb.ToString();
            }
        }
    }
}
