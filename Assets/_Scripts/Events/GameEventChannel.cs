using System;
using UnityEngine;

namespace com.game.events
{
    public class GameEventChannel
    {
        public static event Action OnWaveStarted;
        public static void CommitWaveStart() => OnWaveStarted?.Invoke();

        public static event Action OnWaveEnded;
        public static void CommitWaveEnd() => OnWaveEnded?.Invoke();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset()
        {
            OnWaveStarted = null;
            OnWaveEnded = null;
        }
    }
}
