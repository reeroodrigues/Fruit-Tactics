using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class CardSwapper : MonoBehaviour
{
    public Button _swapAllButton;  
    public Transform _cardVisualsParent;  
    public List<CardType> _availableCardTypes;
    public Timer _timer; // Referência ao Timer

    private void Start()
    {
        if (_swapAllButton != null)
            _swapAllButton.onClick.AddListener(SwapAllCards);
    }

    private void SwapAllCards()
    {
        if (_cardVisualsParent == null || _availableCardTypes == null || _availableCardTypes.Count == 0)
        {
            Debug.LogWarning("CardVisuals ou CardTypes não configurados corretamente!");
            return;
        }

        foreach (Transform cardFaceTransform in _cardVisualsParent)
        {
            CardFace cardFace = cardFaceTransform.GetComponent<CardFace>();
            if (cardFace != null && cardFace._target != null)
            {
                Card card = cardFace._target.GetComponent<Card>();
                if (card != null)
                {
                    CardType newCardType = _availableCardTypes[Random.Range(0, _availableCardTypes.Count)];
                    card._cardType = newCardType;

                    card._cardNumber = newCardType._setAmount == 0 ? Random.Range(0, newCardType._maxCardNumber) : newCardType._setAmount;
                    cardFace._rightNumber.text = card._cardNumber.ToString();
                    cardFace._leftNumber.text = card._cardNumber.ToString();
                }
            }
        }

        // Reduzir o tempo em 10 segundos
        if (_timer != null)
        {
            _timer.AddTime(-10f);
        }

        Debug.Log("Todas as cartas foram trocadas! Tempo reduzido em 10 segundos.");
    }
}