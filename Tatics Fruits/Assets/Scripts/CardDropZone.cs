using UnityEngine;
using UnityEngine.EventSystems;

namespace DefaultNamespace
{
    public class CardDropZone : MonoBehaviour, IDropHandler
    {
        public GameObject _discardArea;
        public void OnDrop(PointerEventData eventData)
        {
            Card card = eventData.pointerDrag.GetComponent<Card>();
            if (card != null)
            {
                // Verifica se é uma área válida para mover o card para o Discard
                if (card.transform.parent != transform)
                {
                    // Move o card para a área de descarte
                    card.transform.SetParent(transform);
                    card.transform.localPosition = Vector3.zero;  // Ajuste a posição conforme necessário
                }
            }
        }

        private void DiscardCard(Card card)
        {
            card.transform.SetParent(_discardArea.transform);
            Destroy(card.gameObject);
        }
    }
}