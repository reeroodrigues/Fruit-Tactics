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
}

[CreateAssetMenu(menuName = "Card/CardType")]
public class CardTypeSo : ScriptableObject
{
    public Sprite cardIcon;
    public int maxCardNumber;
    public int setAmount;

    public bool isPowerCard;
    public PowerEffectType powerEffect;

    [Header("Power Effect Settings")] 
    public float protectionDuration = 5f;
}