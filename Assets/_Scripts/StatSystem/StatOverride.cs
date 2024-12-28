using System;

namespace com.game.statsystem
{
    [System.Serializable]
    public abstract class StatOverride
    {
        public float NewValue;

        public abstract Type GetEnumType();
    }

    [System.Serializable]
    public abstract class StatOverride<T> : StatOverride where T : System.Enum
    {
        public T TargetStatType;

        public override Type GetEnumType()
        {
            return typeof(T);
        }
    }
}
