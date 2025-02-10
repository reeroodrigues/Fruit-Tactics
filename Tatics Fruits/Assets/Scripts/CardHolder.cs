using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardHolder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CardManager _cardManager;
    public bool _available;
    public bool _completed;
    public bool _hasToHaveSameNumberOrColor;
    public int _maxAmount;
    public int _amountToComplete;

    public HolderType _holderType;
    
    public enum HolderType
    {
        Play,
        Discard,
        CardTrader,
        MainHolder
    }

    private void Update()
    {
        HandleCardHolderFunctinallity();

        foreach (Transform child in transform.GetComponentInChildren<Transform>())
        {
            if (_cardManager.Cards.Contains(child.gameObject))
            {
                _cardManager.Cards.Remove(child.gameObject);
            }

            if (child.GetComponent<Card>())
            {
                child.GetComponent<Card>()._canDrag = false;
                child.GetComponent<Card>()._cardState = Card.CardState.Played;
            }
        }
    }

    private void HandleCardHolderFunctinallity()
    {
        if (_holderType == HolderType.Play)
        {
            if (_hasToHaveSameNumberOrColor)
            {
                if (_cardManager._selectedCard != null && transform.childCount > 0) 
                {
                    if (_cardManager._selectedCard.GetComponent<Card>()._cardNumber == transform.GetChild(transform.childCount - 1).GetComponent<Card>()._cardNumber || 
                        _cardManager._selectedCard.GetComponent<Card>()._cardType._cardIcon == transform.GetChild(transform.childCount - 1).GetComponent<Card>()._cardType._cardIcon)
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
                _available = transform.childCount < _maxAmount;
            }

            _completed = transform.childCount == _amountToComplete;
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
