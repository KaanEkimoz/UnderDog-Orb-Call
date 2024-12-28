namespace com.game.statsystem
{
    public static class StatSystemSettings
    {
        /// <summary>
        /// If true, percentage-based modifications always affect their target stat 
        /// based on the overall modified value of that stat.
        /// </summary>
        public const bool PERCENTAGE_MODS_ON_TOP = true;    

        /// <summary>
        /// If true, <see cref="PlayerStatOverride"/>s clear mutations when applied.
        /// </summary>
        public const bool OVERRIDES_CLEAR_MUTATIONS = true;

        /// <summary>
        /// If true (and if <see cref="OVERRIDES_CLEAR_MUTATIONS"/> is false),
        /// <see cref="PlayerStatOverride"/>s overrides the raw form 
        /// (which is not affected from the mutations) of the value of a stat variable 
        /// instead of the refined form.
        /// </summary>
        public const bool OVERRIDES_AFFECT_FROM_ROOT = false && 
            !OVERRIDES_CLEAR_MUTATIONS;
    }
}
