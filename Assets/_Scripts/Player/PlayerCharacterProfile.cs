using UnityEngine;

namespace com.game.player.scriptables
{
    [CreateAssetMenu(fileName = "New PlayerCharacterProfile", menuName = "Game/Player Character Profile", order = int.MinValue)]
    public class PlayerCharacterProfile : ScriptableObject
    {
        public float DefaultStat1;
        public float DefaultStat2;
    }
}