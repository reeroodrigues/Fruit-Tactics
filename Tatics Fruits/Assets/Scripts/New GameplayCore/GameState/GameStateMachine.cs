using System;
using DefaultNamespace.New_GameplayCore;

namespace New_GameplayCore.GameState
{
    public class GameStateMachine : IGameStateMachine
    {
        public DefaultNamespace.New_GameplayCore.GameState Current { get; private set; } = DefaultNamespace.New_GameplayCore.GameState.Boot;
        public event Action<DefaultNamespace.New_GameplayCore.GameState> OnStateChanged;

        public void SetState(DefaultNamespace.New_GameplayCore.GameState next)
        {
            if (Current == next) return;
            Current = next;
            OnStateChanged?.Invoke(next);
        }

        public event Action<DefaultNamespace.New_GameplayCore.GameState> OnStateChange;
    }
}