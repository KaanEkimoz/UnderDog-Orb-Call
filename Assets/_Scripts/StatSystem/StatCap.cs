using System;

namespace com.game.statsystem
{
    [System.Serializable]
    public abstract class StatCap
    {
        public bool CapLow;
        public bool CapHigh;

        public float MinValue;
        public float MaxValue;

        public abstract Type GetEnumType();
    }

    [System.Serializable]
    public abstract class StatCap<T> : StatCap where T : Enum
    {
        public T TargetStatType;

        public override Type GetEnumType()
        {
            return typeof(T);
        }
    }
}
