using UnityEngine;

namespace com.game.statsystem
{
    public enum StatModificationType
    {
        [InspectorName("±")] Incremental = 0,
        [InspectorName("%")] Percentage = 1,
    }
}
