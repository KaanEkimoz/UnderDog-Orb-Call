using UnityEngine;

namespace com.game.generics.interfaces
{
    public interface IVisible
    {
        SparkLight Spark { get; }
        Transform transform { get; }
    }
}
