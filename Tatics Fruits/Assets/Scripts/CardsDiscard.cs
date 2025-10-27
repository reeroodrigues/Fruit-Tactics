using UnityEngine;
using UnityEngine.EventSystems;

public class CardsDiscard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CardManager _cardsManager;
    public void OnPointerEnter(PointerEventData eventData)
    {
        _cardsManager.hoveringMenu = gameObject;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _cardsManager.hoveringMenu = null;
    }
}
