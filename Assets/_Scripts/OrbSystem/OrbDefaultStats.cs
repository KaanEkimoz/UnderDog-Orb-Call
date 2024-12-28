using com.absence.attributes;
using UnityEngine;

namespace com.game.orbsystem.scriptables
{
    [CreateAssetMenu(fileName = "new OrbDefaultStats", menuName = "Game/Orb Default Stats", order = int.MinValue)]
    public class OrbDefaultStats : ScriptableObject
    {
        [Header1("Default Stats for Orb")]

        public float ExampleDefaultValue;
    }
}
