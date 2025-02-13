using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Debuff Powerup", menuName = "CardGame/Debuff")]
    public class DebuffPowerup : CardPowerup
    {
        public override void ApplyEffect(Player player)
        {
            Debug.Log($"Aplicando Debuff: {_powerupName}");
        }
    }
}