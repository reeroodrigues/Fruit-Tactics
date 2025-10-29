using System;

namespace DefaultNamespace.New_GameplayCore
{
    public enum GameState {Boot, Loading, Countdown, Playing, Pause, Results}
    public interface IGameStateMachine
    {
        GameState Current { get; }
        void SetState(GameState next);
        event Action<GameState> OnStateChange;
    }
}