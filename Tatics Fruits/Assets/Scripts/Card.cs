using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Settings")]
    public CardState cardState;

    public CardManager cardManager;
    public CardTypeSo cardTypeSo;
    public int cardNumber;

    private CardFace _cardFace;

    public enum CardState
    {
        Idle,
        IsDragging,
        Played
    }

    [HideInInspector] public bool canDrag;
    [HideInInspector] public bool hovering;
    [HideInInspector] public Canvas canvas;
    [HideInInspector] public bool isFrozen = false;
    [HideInInspector] public bool isProtected = false;
    [HideInInspector] public bool hasBonusPoints = false;

    [Obsolete("Obsolete")]
    private void Start()
    {
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();

        if (cardManager == null)
            cardManager = FindObjectOfType<CardManager>();

        cardManager._cards.Add(gameObject);
        canDrag = true;

        cardNumber = cardTypeSo.setAmount == 0
            ? Random.Range(0, cardTypeSo.maxCardNumber)
            : cardTypeSo.setAmount;

        transform.SetAsLastSibling();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!canDrag)
            return;

        cardState = CardState.IsDragging;
        cardManager._selectedCard = gameObject;

        cardManager.GetComponent<AudioSource>().Play();
        GetComponent<Image>().raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, Input.mousePosition, canvas.worldCamera, out var position);
        transform.position = canvas.transform.TransformPoint(position);
    }

    [Obsolete("Obsolete")]
    public void OnEndDrag(PointerEventData eventData)
    {
        cardState = CardState.Idle;
        cardManager._selectedCard = null;

        if (cardTypeSo.isPowerCard)
        {
            var powerRect = GetComponent<RectTransform>();
            var rootCanvas = canvas.rootCanvas;
            
            var allTargets = FindObjectsOfType<MonoBehaviour>();
            foreach (var target in allTargets)
            {
                if (target == this) continue;

                var targetRectTransform = target.GetComponent<RectTransform>();
                if (targetRectTransform == null) continue;

                var powerWorldRect = RectTransformToScreenRect(powerRect, rootCanvas);
                var targetWorldRect = RectTransformToScreenRect(targetRectTransform, rootCanvas);

                if (powerWorldRect.Overlaps(targetWorldRect))
                {
                    if (target is IPowerupTarget powerupTarget)
                    {
                        powerupTarget.ReceivePowerup(this);
                        return;
                    }

                    if (target is Card targetCard)
                    {
                        return;
                    }
                }
            }

            ResetCardPosition();
            return;
        }

        HandleDropArea();

        transform.SetAsLastSibling();
        cardManager.GetComponent<AudioSource>().Play();
        GetComponent<Image>().raycastTarget = true;
    }

    public void ResetCardPosition()
    {
        transform.localPosition = Vector2.zero;
        GetComponent<Image>().raycastTarget = true;
        transform.SetAsLastSibling();
        cardManager.GetComponent<AudioSource>().Play();
    }

    private void HandleDropArea()
    {
        if (cardManager._hoveringMenu != null)
        {
            var cardHolder = cardManager._hoveringMenu.GetComponent<CardHolder>();

            if (cardHolder != null && cardHolder.holderType == CardHolder.HolderType.Discard)
            {
                transform.SetParent(cardHolder.transform);
                transform.localPosition = Vector3.zero;
            }
            else
            {
                var target = transform.parent;
                transform.position = cardManager._hoveringMenu.transform.position;
                transform.SetParent(cardManager._hoveringMenu.transform);
                Destroy(target.gameObject);
            }
        }
        else
        {
            transform.localPosition = Vector2.zero;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        cardManager._selectedCard = gameObject;

        if (_cardFace != null)
        {
            _cardFace.MoveToLastSibling();
        }
    }

    private Rect RectTransformToScreenRect(RectTransform rectTransform, Canvas canvas)
    {
        var worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);

        var bottomLeft = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, worldCorners[0]);
        var topRight = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, worldCorners[2]);

        return new Rect(bottomLeft, topRight - bottomLeft);
    }
}
