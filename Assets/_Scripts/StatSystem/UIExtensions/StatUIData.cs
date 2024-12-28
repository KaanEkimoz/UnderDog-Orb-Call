using UnityEngine;

namespace com.game.statsystem.ui
{
    [System.Serializable]
    public abstract class StatUIData
    {
        public Sprite Icon;
        public string DisplayName;
        [Multiline] public string Description;
    }

    [System.Serializable]
    public abstract class StatUIData<T> : StatUIData where T : System.Enum
    {
        public T TargetStat;
    }
}
