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
    public struct DeckEntry
    {
        public CardTypeSOI type;
        public int quantity;
    }

    [CreateAssetMenu(menuName = "Create DeckConfigSO", fileName = "DeckConfigSO", order = 0)]
    public class DeckConfigSO : ScriptableObject
    {
        public DeckEntry[] entries;
    }

}