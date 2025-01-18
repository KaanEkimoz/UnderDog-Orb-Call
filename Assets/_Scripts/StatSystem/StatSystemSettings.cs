namespace com.game.statsystem
{
    public static class StatSystemSettings
    {
        /// <summary>
        /// If true, percentage-based modifications always affect their target stat 
        /// based on the overall modified value of that stat.
        /// </summary>
        public const bool PERCENTAGE_MODS_ON_TOP = false;    

        /// <summary>
        /// If true, <see cref="PlayerStatOverride"/>s clear mutations when applied.
        /// </summary>
        public const bool OVERRIDES_CLEAR_MUTATIONS = true;
    }
}
