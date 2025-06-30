using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CardSwapper : MonoBehaviour
{
    public Button swapAllButton;
    public Button swapOneButton;
    public Transform cardVisualsParent;
    public List<CardTypeSo> availableCardTypes;
    public Timer timer;
    
    public CardManager cardManager;

    private void Start()
    {
        if (swapAllButton != null)
            swapAllButton.onClick.AddListener(SwapAllCards);

        if (swapOneButton != null)
            swapOneButton.onClick.AddListener(SwapOneCard);
    }

    private void SwapAllCards()
    {
        if (cardVisualsParent == null || availableCardTypes == null || availableCardTypes.Count == 0)
            return;
        
        if (cardManager != null && cardManager.swapAllFree)
        {
            cardManager.swapAllFree = false;
        }
        else
        {
            if (timer != null)
            {
                timer.AddTime(-10f);
            }
        }

        foreach (Transform cardFaceTransform in cardVisualsParent)
        {
            var cardFace = cardFaceTransform.GetComponent<CardFace>();
            if (cardFace != null && cardFace._target != null)
            {
                var card = cardFace._target.GetComponent<Card>();
                
                if (!card.transform.parent.CompareTag("PlayArea") && !card.isFrozen && !card.isProtected)
                {
                    SwapCard(cardFaceTransform);
                }
            }
        }
        
    }


    private void SwapOneCard()
    {
        if (cardVisualsParent == null || availableCardTypes == null || availableCardTypes.Count == 0)
            return;
        
        var swapableCards = new List<Transform>();

        foreach (Transform cardFaceTransform in cardVisualsParent)
        {
            var cardFace = cardFaceTransform.GetComponent<CardFace>();
            if (cardFace != null && cardFace._target != null)
            {
                var card = cardFace._target.GetComponent<Card>();
                
                if (!card.transform.parent.CompareTag("PlayArea") && !card.isFrozen && !card.isProtected)
                {
                    swapableCards.Add(cardFaceTransform);
                }
            }
        }
        
        if (swapableCards.Count > 0)
        {
            var randomCard = swapableCards[Random.Range(0, swapableCards.Count)];
            SwapCard(randomCard);
        }

        if (timer != null)
        {
            timer.AddTime(-2f);
        }
    }

    private void SwapCard(Transform cardFaceTransform)
    {
        var cardFace = cardFaceTransform.GetComponent<CardFace>();
        if (cardFace != null && cardFace._target != null)
        {
            var card = cardFace._target.GetComponent<Card>();
            if (card != null)
            {
                var newCardTypeSo = availableCardTypes[Random.Range(0, availableCardTypes.Count)];
                card.cardTypeSo = newCardTypeSo;

                card.cardNumber = newCardTypeSo.setAmount == 0
                    ? Random.Range(0, newCardTypeSo.maxCardNumber)
                    : newCardTypeSo.setAmount;
                cardFace._icon.sprite = newCardTypeSo.cardIcon;
                cardFace._rightNumber.text = card.cardNumber.ToString();
                cardFace._leftNumber.text = card.cardNumber.ToString();
            }
        }
    }
}
