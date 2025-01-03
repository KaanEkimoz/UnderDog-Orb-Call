using System;
using UnityEngine;

namespace com.game.utilities
{
    public class ObjectFieldAttribute : PropertyAttribute
    {
        public Type Type;

        public ObjectFieldAttribute(Type type)
        {
            Type = type;
        }
    }
}
