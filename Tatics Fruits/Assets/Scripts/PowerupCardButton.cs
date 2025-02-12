using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class PowerupCardButton : MonoBehaviour
    {
        private CardManager _cardManager;

        private void Start()
        {
            _cardManager = FindObjectOfType<CardManager>();
        }

        public void UseSelectedCardPowerup()
        {
            if (_cardManager == null || _cardManager._selectedCard == null)
                return;

            Card selectedCard = _cardManager._selectedCard.GetComponent<Card>();
            if (selectedCard != null)
            {
                _cardManager.UsePowerupOnCard(selectedCard);
            }
        }
    }
}