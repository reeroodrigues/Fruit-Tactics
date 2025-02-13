using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
    public class CardDropZone : MonoBehaviour, IDropHandler
    {
        public GameObject _discardArea;
        private CardManager _cardManager;

        [Obsolete("Obsolete")]
        private void Start()
        {
            _cardManager = FindObjectOfType<CardManager>();
        }

        public void OnDrop(PointerEventData eventData)
        {
            Card card = eventData.pointerDrag.GetComponent<Card>();
            if (card != null)
            {
                if (card.transform.parent != transform)
                {
                    RemoveCardFace(card);
            
                    card.transform.SetParent(transform);
                    card.transform.localPosition = Vector3.zero;
            
                    _cardManager?.AddCard(1);
                }
            }
        }
        
        private void RemoveCardFace(Card card)
        {
            GameObject cardVisualsParent = GameObject.Find("CardVisuals");
            if (cardVisualsParent == null) return;

            foreach (Transform child in cardVisualsParent.transform)
            {
                CardFace cardFace = child.GetComponent<CardFace>();
                if (cardFace != null && cardFace._target == card.gameObject)
                {
                    Destroy(child.gameObject);
                    break;
                }
            }
        }
    }
}