using System;
using System.Collections.Generic;
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

            foreach (var possibleTarget in FindObjectsOfType<Card>())
            {
                if (possibleTarget == this) continue;
                if (possibleTarget.cardTypeSo.isPowerCard) continue;

                var targetRectTransform = possibleTarget.GetComponent<RectTransform>();
                if (targetRectTransform == null) continue;

                var powerWorldRect = RectTransformToScreenRect(powerRect, rootCanvas);
                var targetWorldRect = RectTransformToScreenRect(targetRectTransform, rootCanvas);

                if (powerWorldRect.Overlaps(targetWorldRect))
                {
                    switch (cardTypeSo.powerEffect)
                    {
                        case PowerEffectType.DoublePoints:
                            possibleTarget.cardNumber *= 2;
    
                            var doubleFace = possibleTarget.GetComponent<CardFace>();
                            if (doubleFace != null)
                                doubleFace.UpdateCardInfo();
                            
                            foreach (var face in FindObjectsOfType<CardFace>())
                            {
                                if (face._target == gameObject)
                                {
                                    Destroy(face.gameObject);
                                    break;
                                }
                            }
                            
                            cardManager._cards.Remove(gameObject);
                            Destroy(transform.parent.gameObject);
                            break;

                        case PowerEffectType.ExplodeAdjacent:
                            foreach (var face in FindObjectsOfType<CardFace>())
                            {
                                if (face._target == possibleTarget.gameObject)
                                {
                                    Destroy(face.gameObject);
                                    break;
                                }
                            }
                            
                            cardManager._cards.Remove(possibleTarget.gameObject);
                            Destroy(possibleTarget.transform.parent.gameObject);
                            
                            foreach (var face in FindObjectsOfType<CardFace>())
                            {
                                if (face._target == gameObject)
                                {
                                    Destroy(face.gameObject);
                                    break;
                                }
                            }

                            cardManager._cards.Remove(gameObject);
                            Destroy(transform.parent.gameObject);
                            break;
                        
                            case PowerEffectType.Freeze:
                            possibleTarget.isFrozen = true;

                            var frozenFace = possibleTarget.GetComponent<CardFace>();
                            if (frozenFace != null)
                            {
                                frozenFace.UpdateCardInfo();
                            }
                            
                            foreach (var face in FindObjectsOfType<CardFace>())
                            {
                                if (face._target == gameObject)
                                {
                                    Destroy(face.gameObject);
                                    break;
                                }
                            }

                            cardManager._cards.Remove(gameObject);
                            Destroy(transform.parent.gameObject);
                            break;
                    }

                    var parentHolder = possibleTarget.transform.parent?.GetComponent<CardHolder>();
                    if (parentHolder != null)
                    {
                        parentHolder.CheckForMatchingCards();
                    }
                    
                    var layoutGroup = cardManager.GetComponentInChildren<HorizontalLayoutGroup>();
                    if (layoutGroup != null)
                    {
                        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
                    }

                    return;
                }
            }

            transform.localPosition = Vector2.zero;
            GetComponent<Image>().raycastTarget = true;
            transform.SetAsLastSibling();
            cardManager.GetComponent<AudioSource>().Play();
            return;
        }
        
        if (cardManager._hoveringMenu != null)
        {
            var cardHolder = cardManager._hoveringMenu.GetComponent<CardHolder>();

            if (cardHolder != null && cardHolder._holderType == CardHolder.HolderType.Discard)
            {
                var discardArea = cardHolder.transform;
                transform.SetParent(discardArea);
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

        transform.SetAsLastSibling();
        cardManager.GetComponent<AudioSource>().Play();
        GetComponent<Image>().raycastTarget = true;
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
