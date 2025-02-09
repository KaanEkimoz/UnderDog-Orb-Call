using UnityEngine;

namespace com.game.abilitysystem
{
    public enum AbilityUseType
    {
        [InspectorName("Press (One Shot)")] Press,
        Hold,
        Toggle,
        Custom,
    }
}
