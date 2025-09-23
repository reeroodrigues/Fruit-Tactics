using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Serialization;

public class CardHolder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CardManager cardManager;
    public bool available;
    public bool completed;
    public bool hasToHaveSameNumberOrColor;
    public int maxAmount;
    public int amountToComplete;

    public HolderType holderType;
    public ScoreManager scoreManager;
    public TextMeshProUGUI scoreText;
    public Timer timer;

    public enum HolderType
    {
        Play,
        Discard,
        CardTrader,
        MainHolder
    }

    private void Update()
    {
        HandleCardHolderFunctionality();
        CheckForMatchingCards();

        foreach (Transform child in transform.GetComponentInChildren<Transform>())
        {
            if (cardManager._cards.Contains(child.gameObject))
            {
                cardManager._cards.Remove(child.gameObject);
            }

            if (child.GetComponent<Card>())
            {
                child.GetComponent<Card>().canDrag = false;
                child.GetComponent<Card>().cardState = Card.CardState.Played;
            }
        }
        if (timer != null && timer.GetTimeRemaining() <= 0)
        {
            foreach (Transform child in transform)
            {
                var card = child.GetComponent<Card>();
                if (card != null)
                    card.canDrag = false;
            }
        }
    }

    public void CheckForMatchingCards()
    {
        if (holderType != HolderType.Play) return;

        var matchingCards = new Dictionary<string, List<Card>>();

        foreach (Transform child in transform)
        {
            var card = child.GetComponent<Card>();
            if (card == null) continue;

            var key = $"{card.cardTypeSo.cardIcon}";

            if (!matchingCards.ContainsKey(key))
                matchingCards[key] = new List<Card>();

            matchingCards[key].Add(card);
        }

        var cardsToRemove = new List<GameObject>();

        foreach (var pair in matchingCards)
        {
            if (pair.Value.Count >= 2)
            {
                var sumValue = 0;
                foreach (var card in pair.Value)
                {
                    var cardScore = card.cardNumber;
                    if (card.hasBonusPoints)
                        cardScore += 10;

                    sumValue += cardScore;
                    cardsToRemove.Add(card.gameObject);
                }

                if (scoreManager != null)
                {
                    scoreManager.AddScore(sumValue);
                    UpdateScoreText();
                }

                if (timer != null)
                {
                    timer.AddTime(5f);
                }
            }
        }

        foreach (var card in cardsToRemove)
        {
            card.transform.SetParent(null);
            Destroy(card);
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = scoreManager.GetScore().ToString();
        }
    }

    public void HandleCardHolderFunctionality()
    {
        if (holderType == HolderType.Play)
        {
            if (timer != null && timer.GetTimeRemaining() <= 0)
            {
                available = false;
            }
            else
            {
                if (hasToHaveSameNumberOrColor)
                {
                    if (cardManager._selectedCard != null && transform.childCount > 0)
                    {
                        var lastChild = transform.GetChild(transform.childCount - 1);
                        if (lastChild != null)
                        {
                            var lastCard = lastChild.GetComponent<Card>();

                            if (lastCard != null && cardManager._selectedCard.GetComponent<Card>() != null)
                            {
                                if (cardManager._selectedCard.GetComponent<Card>().cardNumber == lastCard.cardNumber ||
                                    cardManager._selectedCard.GetComponent<Card>().cardTypeSo.cardIcon == lastCard.cardTypeSo.cardIcon)
                                {
                                    available = transform.childCount < maxAmount;
                                }
                                else
                                {
                                    available = false;
                                }
                            }
                            else
                            {
                                available = true;
                            }
                        }
                        else
                        {
                            available = true;
                        }
                    }
                    else
                    {
                        available = true;
                    }
                }
                else
                {
                    available = transform.childCount < maxAmount;
                }
            }

            completed = transform.childCount == amountToComplete;
        }

        if (holderType == HolderType.Discard)
        {
            available = true;
        }

        if (holderType == HolderType.CardTrader)
        {
            available = true;
        }

        if (holderType == HolderType.MainHolder)
        {
            available = true;
            completed = transform.childCount == amountToComplete;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (available)
            cardManager._hoveringMenu = gameObject;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (available)
            cardManager._hoveringMenu = null;
    }
}
