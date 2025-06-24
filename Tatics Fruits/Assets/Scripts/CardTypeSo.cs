using UnityEngine;

public enum PowerEffectType
{
    None,
    DoublePoints,
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