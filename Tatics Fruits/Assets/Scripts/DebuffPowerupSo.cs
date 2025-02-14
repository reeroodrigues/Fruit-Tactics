using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Debuff Powerup", menuName = "CardGame/Debuff")]
    public class DebuffPowerupSo : ScriptableObject
    {
        [Header("Info")] 
        public Sprite _cardIcon;
        public string _name;
        public string _description;
    }
}