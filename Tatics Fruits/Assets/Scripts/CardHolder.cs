using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CardHolder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CardManager _cardManager;
    public bool _available;
    public bool _completed;
    public bool _hasToHaveSameNumberOrColor;
    public int _maxAmount;
    public int _amountToComplete;

    public HolderType _holderType;
    public ScoreManager _scoreManager;
    public TextMeshProUGUI _scoreText;
    public Timer _timer;

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
            if (_cardManager._cards.Contains(child.gameObject))
            {
                _cardManager._cards.Remove(child.gameObject);
            }

            if (child.GetComponent<Card>())
            {
                child.GetComponent<Card>().canDrag = false;
                child.GetComponent<Card>().cardState = Card.CardState.Played;
            }
        }
        if (_timer != null && _timer.GetTimeRemaining() <= 0)
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
        if (_holderType != HolderType.Play) return;

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

                foreach (Card card in pair.Value)
                {
                    sumValue += card.cardNumber;
                    cardsToRemove.Add(card.gameObject);
                }

                if (_scoreManager != null)
                {
                    _scoreManager.AddScore(sumValue);
                    UpdateScoreText();
                }

                if (_timer != null)
                {
                    _timer.AddTime(5f);
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
        if (_scoreText != null)
        {
            _scoreText.text = _scoreManager.GetScore().ToString();
        }
    }

    public void HandleCardHolderFunctionality()
    {
        if (_holderType == HolderType.Play)
        {
            if (_timer != null && _timer.GetTimeRemaining() <= 0)
            {
                _available = false;
            }
            else
            {
                if (_hasToHaveSameNumberOrColor)
                {
                    if (_cardManager._selectedCard != null && transform.childCount > 0)
                    {
                        var lastChild = transform.GetChild(transform.childCount - 1);
                        if (lastChild != null)
                        {
                            var lastCard = lastChild.GetComponent<Card>();

                            if (lastCard != null && _cardManager._selectedCard.GetComponent<Card>() != null)
                            {
                                if (_cardManager._selectedCard.GetComponent<Card>().cardNumber == lastCard.cardNumber ||
                                    _cardManager._selectedCard.GetComponent<Card>().cardTypeSo.cardIcon == lastCard.cardTypeSo.cardIcon)
                                {
                                    _available = transform.childCount < _maxAmount;
                                }
                                else
                                {
                                    _available = false;
                                }
                            }
                            else
                            {
                                _available = true;
                            }
                        }
                        else
                        {
                            _available = true;
                        }
                    }
                    else
                    {
                        _available = true;
                    }
                }
                else
                {
                    _available = transform.childCount < _maxAmount;
                }
            }

            _completed = transform.childCount == _amountToComplete;
        }

        if (_holderType == HolderType.Discard)
        {
            _available = true;
        }

        if (_holderType == HolderType.CardTrader)
        {
            _available = true;
        }

        if (_holderType == HolderType.MainHolder)
        {
            _available = true;
            _completed = transform.childCount == _amountToComplete;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_available)
            _cardManager._hoveringMenu = gameObject;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_available)
            _cardManager._hoveringMenu = null;
    }
}
