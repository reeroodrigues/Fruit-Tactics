using System;

namespace New_GameplayCore
{
    public interface IGameController
    {
        void StartLevel(LevelConfigSO cfg, DeckConfigSO deckCfg);
        void UpdateTick(float deltaTime);
        void OnCardSelected(CardInstance card);
        void OnSwapAllRequested();
        void OnSwapRandomRequested();
        event Action OnLevelEnded;

    }
}