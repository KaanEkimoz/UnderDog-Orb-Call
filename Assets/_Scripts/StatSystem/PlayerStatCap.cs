namespace com.game.statsystem
{
    [System.Serializable]
    public class PlayerStatCap
    {
        public PlayerStatType TargetStatType;

        public bool CapLow;
        public bool CapHigh;

        public float MinValue;
        public float MaxValue;
    }
}
