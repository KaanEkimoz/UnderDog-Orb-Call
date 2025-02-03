using System.Collections.Generic;

namespace com.game.utilities
{
    public interface IDatabaseInstance
    {
        int Size { get; }
        void Refresh();
    }

    public interface IDatabaseInstance<T1, T2> : IDatabaseInstance, IEnumerable<T2> where T2 : UnityEngine.Object
    {
        T2 Get(T1 key);
        bool TryGet(T1 key, out T2 output);
        bool Contains(T1 key);
    }

    public interface INativeDatabaseInstance<T1, T2> : IDatabaseInstance<T1, T2> where T2 : UnityEngine.Object
    {

    }

    public interface IMemberDatabaseInstance<T1, T2> : IDatabaseInstance<T1, T2> where T2 : UnityEngine.Object
    {

    }

    public interface IDynamicDatabaseInstance<T1, T2> : IDatabaseInstance<T1, T2> where T2 : UnityEngine.Object
    {

    }

    public interface INativeDatabaseInstance<T> : INativeDatabaseInstance<string, T> where T : UnityEngine.Object
    {

    }

    public interface IMemberDatabaseInstance<T> : IMemberDatabaseInstance<string, T> where T : UnityEngine.Object, IDatabaseMember
    {

    }

    public interface IDynamicDatabaseInstance<T> : IDynamicDatabaseInstance<string, T> where T : UnityEngine.Object
    {

    }
}
