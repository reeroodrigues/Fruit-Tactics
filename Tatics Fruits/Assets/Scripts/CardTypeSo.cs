using UnityEngine;

public enum PowerEffectType
{
    None,
    DoublePoints,
    ExplodeAdjacent
}

[CreateAssetMenu(menuName = "Card/CardType")]
public class CardTypeSo : ScriptableObject
{
    public Sprite cardIcon;
    public int maxCardNumber;
    public int setAmount;

    public bool isPowerCard;
    public PowerEffectType powerEffect;
}