using DefaultNamespace;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBuff", menuName = "CardGame/Buff")]
public class BuffPowerup : CardPowerup
{
    public override void ApplyEffect(Player player)
    {
        Debug.Log($"Aplicando Buff: {_powerupName}");
    }
}