using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class PowerupCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [Header("Settings")]
        public CardState _cardState;
        public CardManager _cardManager;
        public CardPowerupTypeSo _cardPowerupTypeSo;
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
            _cardManager._cards.Add(gameObject);
            _canDrag = true;
            
            _cardNumber = _cardPowerupTypeSo._effectValue == 0 ? Random.Range(1, 10) : _cardPowerupTypeSo._effectValue;
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
                    
                    Transform discardArea = cardHolder.transform;
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
        }
        
        public void ApplyPowerupEffect(Card targetCard)
        {
            if (_cardPowerupTypeSo != null)
            {
                _cardPowerupTypeSo.ApplyEffect(targetCard);
            }
        }
    }
}