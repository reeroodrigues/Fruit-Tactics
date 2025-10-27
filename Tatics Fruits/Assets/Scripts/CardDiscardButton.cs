using System;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class CardDiscardButton : MonoBehaviour
    {
        public Timer _timer;
        public CardManager _cardManager;
        public Button _discardButton;

        private void Start()
        {
            _discardButton.onClick.AddListener(OnDiscardCardsClicked);
        }

        public void OnDiscardCardsClicked()
        {
            DiscardAllCards();
            
            Debug.Log("Houve o click!");
            _cardManager.AddCard(_cardManager.startingAmount);
            ReduceTimer();
        }

        private void DiscardAllCards()
        {
            foreach (var card in _cardManager.cards)
            {
                if (card != null)
                {
                    Destroy(card);
                    Debug.Log("Carta destruída: " + card.name);
                }
            }
            _cardManager.cards.Clear();
            Debug.Log("Cartas descartadas!");
        }

        private void ReduceTimer()
        {
            _timer.remainingTime = Mathf.Max(0, _timer.remainingTime - 5);
            Debug.Log("Tempo restante: " + _timer.remainingTime);
        }
    }
}