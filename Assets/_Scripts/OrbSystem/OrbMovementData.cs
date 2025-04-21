using UnityEngine;

namespace com.game.orbsystem
{
    [CreateAssetMenu(menuName = "Game/Orb System/Orb Movement Data", fileName = "New Orb Movement Data")]
    public class OrbMovementData : ScriptableObject
    {
        public float movementSpeed = 5f;
        public float throwSpeedMultiplier = 1f;
        public float recallSpeedMultiplier = 1f;
        public float maxDistance = 20f;
    }
}
