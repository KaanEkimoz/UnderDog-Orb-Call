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

        public static event Action OnAltarCooldownEnded;
        public static void CommitAltarCooldownEnd() => OnAltarCooldownEnded?.Invoke();

        public static event Action<GameState> OnGameStateChanged;
        public static void CommitGameStateChange(GameState state) => OnGameStateChanged?.Invoke(state);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Reset()
        {
            OnWaveStarted = null;
            OnWaveEnded = null;
            OnGameStateChanged = null;
            OnAltarCooldownEnded = null;
        }
    }
}
