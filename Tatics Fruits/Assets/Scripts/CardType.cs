using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "CardTypeScriptableObject", menuName = "CardType/CardTypeScriptableObject")]
    public class CardType : ScriptableObject
    {
        [Header("Info")] 
        public Sprite _cardIcon;
        public int _maxCardNumber;
        public int _setAmount;
    }
}