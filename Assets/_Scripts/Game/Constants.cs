namespace com.game
{
    public static class Constants
    {
        public static class Gameplay
        {
            /// <summary>
            /// This is the maxima count of orbs can be held by the player.
            /// </summary>
            // Set a value high enough to avoid exceeding of this count.
            // Set a value low enough to keep any computation related to this constant performent.
            public const int MAX_ORBS_CAN_BE_HELD = 12;
        }

        public static class AssetManagement
        {
            public const string RELEASE_BUILD_ASSET_LABEL = "release";
        }
    }
}
