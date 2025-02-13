using UnityEngine;

namespace DefaultNamespace
{
    public abstract class CardPowerup : ScriptableObject
    {
        public string _powerupName;
        public string _description;

        public abstract void ApplyEffect(Player player);
    }
}