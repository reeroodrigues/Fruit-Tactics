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
    }

    private void HandleCardHolderFunctinallity()
    {
        if (_holderType == HolderType.Play)
        {
            if (_hasToHaveSameNumberOrColor)
            {
                if (_cardManager._selectedCard != null && transform.childCount > 0)
                {
                    Transform lastChild = transform.GetChild(transform.childCount - 1); // Obtém o último filho

                    if (lastChild != null) // Verifica se o filho existe
                    {
                        Card lastCard = lastChild.GetComponent<Card>(); // Obtém o componente Card

                        if (lastCard != null && _cardManager._selectedCard.GetComponent<Card>() != null) //Verifica se ambos os componentes existem
                        {
                            if (_cardManager._selectedCard.GetComponent<Card>()._cardNumber == lastCard._cardNumber ||
                                _cardManager._selectedCard.GetComponent<Card>()._cardType._cardIcon == lastCard._cardType._cardIcon)
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
                            _available = true; // Ou false, dependendo da lógica desejada. É importante definir um valor aqui.
                            if(lastCard == null)
                            {
                                Debug.LogError("Filho não possui o componente Card!");
                            }
                            if(_cardManager._selectedCard.GetComponent<Card>() == null)
                            {
                                Debug.LogError("Carta selecionada não possui o componente Card!");
                            }
                        }
                    }
                    else
                    {
                        _available = true; // Ou false, dependendo da lógica desejada. É importante definir um valor aqui.
                        Debug.LogError("Não há filhos em transform!");
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
        if (_holderType == HolderType.Discard)
        {
            _available = false;
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
