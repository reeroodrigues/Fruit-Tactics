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
                child.GetComponent<Card>()._canDrag = false;
                child.GetComponent<Card>()._cardState = Card.CardState.Played;
            }
        }

        // Impede que as cartas sejam arrastadas após o tempo acabar
        if (_timer != null && _timer.GetTimeRemaining() <= 0)
        {
            foreach (Transform child in transform)
            {
                Card card = child.GetComponent<Card>();
                if (card != null)
                    card._canDrag = false;
            }
        }
    }

    private void CheckForMatchingCards()
    {
        if (_holderType != HolderType.Play) return;

        Dictionary<string, List<Card>> matchingCards = new Dictionary<string, List<Card>>();

        foreach (Transform child in transform)
        {
            Card card = child.GetComponent<Card>();
            if (card == null) continue;

            string key = $"{card._cardNumber}_{card._cardTypeSo._cardIcon}";

            if (!matchingCards.ContainsKey(key))
                matchingCards[key] = new List<Card>();

            matchingCards[key].Add(card);
        }

        List<GameObject> cardsToRemove = new List<GameObject>();

        foreach (var pair in matchingCards)
        {
            if (pair.Value.Count >= 2)
            {
                int sumValue = 0;

                foreach (Card card in pair.Value)
                {
                    sumValue += card._cardNumber;
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

        foreach (GameObject card in cardsToRemove)
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
                _available = false; // Impede que joguem cartas na PlayArea
            }
            else
            {
                if (_hasToHaveSameNumberOrColor)
                {
                    if (_cardManager._selectedCard != null && transform.childCount > 0)
                    {
                        Transform lastChild = transform.GetChild(transform.childCount - 1);
                        if (lastChild != null)
                        {
                            Card lastCard = lastChild.GetComponent<Card>();

                            if (lastCard != null && _cardManager._selectedCard.GetComponent<Card>() != null)
                            {
                                if (_cardManager._selectedCard.GetComponent<Card>()._cardNumber == lastCard._cardNumber ||
                                    _cardManager._selectedCard.GetComponent<Card>()._cardTypeSo._cardIcon == lastCard._cardTypeSo._cardIcon)
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
