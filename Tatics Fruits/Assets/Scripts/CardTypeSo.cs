using UnityEngine;

public enum PowerEffectType
{
    None,
    DoublePoints,
    ExplodeAdjacent,
    Freeze,
    Protection,
    Joker,
    ClearRow,
    IncreaseNumber,
    BonusPoints,
    Cleanse,
    FreezeTime,
    SwapFree
}

[CreateAssetMenu(menuName = "Card/CardType")]
public class CardTypeSo : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite cardIcon;
    public int maxCardNumber;
    public int setAmount;
    public int priceGold;
    public string rarity;

    public bool isPowerCard;
    public PowerEffectType powerEffect;

    [Header("Power Effect Settings")]
    public float protectionDuration = 5f;
    public float freezeTimeDuration = 5f; 
    public int increaseAmountMin = 1;
    public int increaseAmountMax = 5;
    
}