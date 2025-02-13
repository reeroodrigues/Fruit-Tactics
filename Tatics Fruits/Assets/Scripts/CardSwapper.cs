using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class CardSwapper : MonoBehaviour
{
    public Button _swapAllButton;  // Botão para trocar todas as cartas
    public Button _swapOneButton;  // Botão para trocar uma única carta
    public Transform _cardVisualsParent;  
    public List<CardTypeSo> _availableCardTypes;
    public Timer _timer; // Referência ao Timer

    private void Start()
    {
        if (_swapAllButton != null)
            _swapAllButton.onClick.AddListener(SwapAllCards);

        if (_swapOneButton != null)
            _swapOneButton.onClick.AddListener(SwapOneCard);
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
            SwapCard(cardFaceTransform);
        }

        // Reduz o tempo em 10 segundos
        if (_timer != null)
        {
            _timer.AddTime(-10f);
        }

        Debug.Log("Todas as cartas foram trocadas! Tempo reduzido em 10 segundos.");
    }

    private void SwapOneCard()
    {
        if (_cardVisualsParent == null || _availableCardTypes == null || _availableCardTypes.Count == 0)
        {
            Debug.LogWarning("CardVisuals ou CardTypes não configurados corretamente!");
            return;
        }

        int totalCards = _cardVisualsParent.childCount;
        if (totalCards == 0)
        {
            Debug.LogWarning("Nenhuma carta para trocar!");
            return;
        }

        // Escolhe uma carta aleatória e troca
        int randomIndex = Random.Range(0, totalCards);
        SwapCard(_cardVisualsParent.GetChild(randomIndex));

        // Reduz o tempo em 2 segundos apenas
        if (_timer != null)
        {
            _timer.AddTime(-2f);
        }

        Debug.Log("Uma carta foi trocada! Tempo reduzido em 2 segundos.");
    }

    private void SwapCard(Transform cardFaceTransform)
    {
        CardFace cardFace = cardFaceTransform.GetComponent<CardFace>();
        if (cardFace != null && cardFace._target != null)
        {
            Card card = cardFace._target.GetComponent<Card>();
            if (card != null)
            {
                CardTypeSo newCardTypeSo = _availableCardTypes[Random.Range(0, _availableCardTypes.Count)];
                card._cardTypeSo = newCardTypeSo;

                // Atualiza os valores da carta
                card._cardNumber = newCardTypeSo._setAmount == 0 ? Random.Range(0, newCardTypeSo._maxCardNumber) : newCardTypeSo._setAmount;
                cardFace._icon.sprite = newCardTypeSo._cardIcon;
                cardFace._rightNumber.text = card._cardNumber.ToString();
                cardFace._leftNumber.text = card._cardNumber.ToString();
            }
        }
    }
}
