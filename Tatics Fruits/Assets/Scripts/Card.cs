using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Settings")] 
    public CardState _cardState;

    public CardManager _cardManager;
    public CardType _cardType;
    public int _cardNumber;

    public enum CardState
    {
        Idle,
        IsDragging,
        Played
    }

    [HideInInspector] public bool _canDrag;
    [HideInInspector] public bool _hovering;
    [HideInInspector] public Canvas _canvas;

    private void Start()
    {
        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        _cardManager = GameObject.Find("CardsManager").GetComponent<CardManager>();
        _cardManager.Cards.Add(gameObject);
        _canDrag = true;

        _cardNumber = _cardType._setAmount == 0 ? Random.Range(0, _cardType._maxCardNumber) : _cardType._setAmount;
    }
    

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_canDrag)
            return;

        _cardState = CardState.IsDragging;

        _cardManager._selectedCard = gameObject;
        _cardManager.GetComponent<AudioSource>().Play();
        GetComponent<Image>().raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_canDrag)
            return;

        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)_canvas.transform, Input.mousePosition, _canvas.worldCamera, out position);
        transform.position = _canvas.transform.TransformPoint(position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _cardState = CardState.Idle;

        _cardManager._selectedCard = null;
        if (_cardManager._hoveringMenu != null)
        {
            if (_cardManager._hoveringMenu.GetComponent<CardHolder>()._holderType == CardHolder.HolderType.CardTrader)
            {
                _cardManager.AddCard(_cardNumber);
            }

            Transform target = transform.parent;
            transform.position = _cardManager._hoveringMenu.transform.position;
            transform.SetParent(_cardManager._hoveringMenu.transform);
            Destroy(target.gameObject);
        }
        else
        {
            transform.transform.localPosition = Vector2.zero;
        }

        _cardManager.GetComponent<AudioSource>().Play();
        GetComponent<Image>().raycastTarget = true;
        GetComponent<Image>().raycastTarget = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hovering = true || _cardState == CardState.IsDragging;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hovering = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _cardManager._selectedCard = gameObject;
    }
}
