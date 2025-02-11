using com.absence.utilities.experimental.databases;

namespace com.game.utilities
{
    [System.Serializable]
    public abstract class SkinnedField
    {
        public virtual bool AllowSceneObjects => false;

#if UNITY_EDITOR
        public abstract object Fetch();
#endif
    }

    [System.Serializable]
    public class SkinnedField<T1, T2> : SkinnedField where T2 : UnityEngine.Object, IDatabaseMember<T1>
    {
#if UNITY_EDITOR
        public T2 SkinValue;
#endif
        public T1 RealValue;

#if UNITY_EDITOR
        public override object Fetch()
        {
            if (SkinValue == null) return default(T1);
            return SkinValue.GetDatabaseKey();
        }
#endif
    }
}
