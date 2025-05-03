using UnityEngine;

namespace com.game.utilities
{
    public interface IStunnable : IObject
    {
        void Stun(float duration, CCBreakFlags flags);
    }
}
