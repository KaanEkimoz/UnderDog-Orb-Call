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
                    string dataDesc = (itemObject.CustomData[ItemBehaviour.CustomDataKey] as ItemBehaviour).GenerateDataDescription(richText);
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
        public static class Recipes
        {
            public static bool TryCombine<T>(T profile1, T profile2, out T result) where T : ItemProfileBase
            {
                result = null;
                bool success = CanCombine(profile1, profile2);
                if (!success) return success;

                result = Combine<T>(profile1, profile2);
                return result;
            }

            public static bool CanCombine<T>(T profile1, T profile2) where T : ItemProfileBase 
            {
                return false;
            }

            public static T Combine<T>(T profile1, T profile2) where T : ItemProfileBase
            {
                return null;
            }
        }
    }
}
