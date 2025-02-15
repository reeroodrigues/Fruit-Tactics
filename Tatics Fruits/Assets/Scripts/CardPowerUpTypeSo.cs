using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "CardPowerUpTypeScriptableObject", menuName = "CardType/CardPowerUpTypeScriptableObject")]
    public class CardPowerUpTypeSo : ScriptableObject
    {
        [Header("Info")] 
        public Sprite _cardIcon;
        public int _maxCardNumber;
        public int _setAmount;
    }
}