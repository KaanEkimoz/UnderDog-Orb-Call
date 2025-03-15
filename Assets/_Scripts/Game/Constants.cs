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

        public static class Paranoia
        {
            public const int PARANOIA_SEGMENT_COUNT = 5;
        }

        public static class Shopping
        {
            public const int PLAYER_SHOP_CAPACITY = 4;
            public const int ORB_SHOP_CAPACITY = 3;
        }

        public static class AssetManagement
        {
            public const string RELEASE_BUILD_ASSET_LABEL = "release";
        }

        public static class Placeholder
        {
            public static readonly UnityEngine.Vector3 AltarPosition = UnityEngine.Vector3.zero;
        }
    }
}
