using DefaultNamespace;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Settings")] 
    public CardState _cardState;

    public CardManager _cardManager;
    public CardTypeSo _cardTypeSo;
    public int _cardNumber;

    private CardFace _cardFace;

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
        if (_canvas == null)
            _canvas = FindObjectOfType<Canvas>();

        if (_cardManager == null)
            _cardManager = FindObjectOfType<CardManager>();

        _cardManager._cards.Add(gameObject);
        _canDrag = true;

        _cardNumber = _cardTypeSo._setAmount == 0 
            ? Random.Range(0, _cardTypeSo._maxCardNumber) 
            : _cardTypeSo._setAmount;

        transform.SetAsLastSibling();
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

        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)_canvas.transform, Input.mousePosition, _canvas.worldCamera, out var position);
        transform.position = _canvas.transform.TransformPoint(position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _cardState = CardState.Idle;

        _cardManager._selectedCard = null;

        if (_cardManager._hoveringMenu != null)
        {
            var cardHolder = _cardManager._hoveringMenu.GetComponent<CardHolder>();
            
            if (cardHolder != null && cardHolder._holderType == CardHolder.HolderType.Discard)
            {
                var discardArea = cardHolder.transform;
                transform.SetParent(discardArea);
                transform.localPosition = Vector3.zero;
            }
            else
            {
                var target = transform.parent;
                transform.position = _cardManager._hoveringMenu.transform.position;
                transform.SetParent(_cardManager._hoveringMenu.transform);
                Destroy(target.gameObject);
            }
        }
        else
        {
            transform.transform.localPosition = Vector2.zero;
        }
        
        transform.SetAsLastSibling();

        _cardManager.GetComponent<AudioSource>().Play();
        GetComponent<Image>().raycastTarget = true;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        _hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hovering = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _cardManager._selectedCard = gameObject;

        if (_cardFace != null)
        {
            _cardFace.MoveToLastSibling();
        }
    }
}
