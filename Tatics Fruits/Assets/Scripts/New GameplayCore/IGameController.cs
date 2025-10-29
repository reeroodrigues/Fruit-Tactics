using System;

namespace New_GameplayCore
{
    public interface IGameController
    {
        bool IsPlaying { get; }
        event System.Action OnEnterPreRound;
        event System.Action OnExitPreRound;
        void StartLevel(LevelConfigSO cfg, DeckConfigSO deckCfg);
        void UpdateTick(float deltaTime);
        void OnCardSelected(CardInstance card);
        void BeginPlayFromPreRound(New_GameplayCore.PreRoundModel model);
        void BackToLevelSelect();
        void OnSwapAllRequested();
        void OnSwapRandomRequested();
        event Action OnLevelEnded;

    }
}