using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class CardSwapper : MonoBehaviour
{
    public Button _swapAllButton;
    public Button _swapOneButton;
    public Transform _cardVisualsParent;
    public List<CardTypeSo> _availableCardTypes;
    public Timer _timer;

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
            return;

        foreach (Transform cardFaceTransform in _cardVisualsParent)
        {
            CardFace cardFace = cardFaceTransform.GetComponent<CardFace>();
            if (cardFace != null && cardFace._target != null)
            {
                Card card = cardFace._target.GetComponent<Card>();
                
                if (card != null && card.transform.parent != null && 
                    !card.transform.parent.CompareTag("PlayArea")) 
                {
                    SwapCard(cardFaceTransform);
                }
            }
        }

        if (_timer != null)
        {
            _timer.AddTime(-10f);
        }
    }


    private void SwapOneCard()
    {
        if (_cardVisualsParent == null || _availableCardTypes == null || _availableCardTypes.Count == 0)
            return;

        foreach (Transform cardFaceTransform in _cardVisualsParent)
        {
            CardFace cardFace = cardFaceTransform.GetComponent<CardFace>();
            if (cardFace != null && cardFace._target != null)
            {
                Card card = cardFace._target.GetComponent<Card>();
                
                if (card != null && card.transform.parent != null && 
                    !card.transform.parent.CompareTag("PlayArea")) 
                {
                    SwapCard(cardFaceTransform);
                }
            }
        }

        if (_timer != null)
        {
            _timer.AddTime(-2f);
        }
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

                card._cardNumber = newCardTypeSo._setAmount == 0
                    ? Random.Range(0, newCardTypeSo._maxCardNumber)
                    : newCardTypeSo._setAmount;
                cardFace._icon.sprite = newCardTypeSo._cardIcon;
                cardFace._rightNumber.text = card._cardNumber.ToString();
                cardFace._leftNumber.text = card._cardNumber.ToString();
            }
        }
    }

    public void DisableSwapButtons()
    {
        if (_swapAllButton != null)
            _swapAllButton.interactable = false;
        
        if (_swapOneButton != null)
            _swapOneButton.interactable = false;
    }
}
