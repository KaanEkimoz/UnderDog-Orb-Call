using System;

namespace com.game.utilities
{
    [System.Serializable]
    public abstract class SkinnedField
    {
        public virtual bool AllowSceneObjects => false;

        public abstract Type GetSkinnedType();
        public abstract Type GetRealType();
    }

    [System.Serializable]
    public abstract class SkinnedField<T1, T2> : SkinnedField where T1 : UnityEngine.Object
    {
        public T2 RealValue;

        public override Type GetSkinnedType() => typeof(T1); 
        public override Type GetRealType() => typeof(T2); 

        public abstract T2 FetchRealValue(T1 newSkinnedValue);
        public abstract T1 RefreshSkinnedValue(T2 realValue);
    }
}
