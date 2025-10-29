using UnityEngine;

namespace New_GameplayCore
{
    [CreateAssetMenu(menuName = "Create CardTypeSOI", fileName = "CardTypeSOI", order = 0)]
    public class CardTypeSOI : ScriptableObject
    {
        public string id;
        public Sprite sprite;
        public int baseValue = 1;
        public Rarity rarity;
    }

    public enum Rarity
    {
        Common,
        Rare,
        Epic
    }

    [System.Serializable]
    public struct DeckEntrySummary
    {
        public CardTypeSOI type;
        public int quantity;
    }

    [CreateAssetMenu(menuName = "Create DeckConfigSO", fileName = "DeckConfigSO", order = 0)]
    public class DeckConfigSO : ScriptableObject
    {
        public DeckEntrySummary[] Entries;
    }

    [System.Serializable]
    public struct PreRoundModel
    {
        public string LevelId;
        public string DisplayName;
        public string Description;

        public int TargetScore;
        public int Star1Score;
        public int Star2Score;
        public int Star3Score;

        public int InitialTimeSec;
        public int HandSize;
        public int TimeBonusOnPair;
        public int SwapAllPenalty;
        public int SwapRandomPenalty;
        public bool AllowRefillFromDiscard;

        public int DeckTotalCount;
        public DeckEntrySummary[] Composition;

        public int BestScore;

        public bool UseFixedSeed;
        public int EffectiveSeed;
    }

}