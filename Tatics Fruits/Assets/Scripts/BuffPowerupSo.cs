using DefaultNamespace;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuff", menuName = "CardGame/Buff")]
public class BuffPowerupSo : ScriptableObject
{
    [Header("Info")] 
    public Sprite _cardIcon;
    public string _name;
    public string _description;
}