using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "CardPowerupTypeScriptableObject", menuName = "CardPowerupType/CardPowerupTypeScriptableObject")]
    public class CardPowerupTypeSo : ScriptableObject
    {
        [Header("Info")] 
        public string _powerupName;
        public string _descriptionName;
        public Sprite _icon;
        public PowerupType _powerupType;
        public int _effectValue;

        public enum PowerupType
        {
            IncreaseValue,
            DoubleValue,
            StealCard,
            Shield,
            ExtraTurn,
            Randomize,
            Destroy,
        }

        public void ApplyEffect(Card targetcard)
        {
            switch (_powerupType)
            {
                case PowerupType.IncreaseValue:
                    targetcard._cardNumber += _effectValue;
                    break;
                case PowerupType.DoubleValue:
                    targetcard._cardNumber *= 2;
                    break;
                case PowerupType.StealCard:
                    Debug.Log("Escolha uma carta para ficar no topo da mesa!");
                    break;
                case PowerupType.Shield:
                    Debug.Log("Selecione a carta que será protegida e não poderá ser descartada");
                    break;
                case PowerupType.ExtraTurn:
                    Debug.Log("Jogador ganhou mais tempo para jogar!");
                    break;
                case PowerupType.Randomize:
                    targetcard._cardNumber = Random.Range(1, 10);
                    break;
                case PowerupType.Destroy:
                    Debug.Log("Destrua uma carta da sua mão!");
                    break;
            }
        }
    }
}