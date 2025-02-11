using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;

public class CardSwapper : MonoBehaviour
{
    public Button swapAllButton;  // O botão que dispara a troca de cartas
    public Transform cardVisualsParent;  // O GameObject que contém os prefabs `CardFace`
    public List<CardType> availableCardTypes; // Lista de tipos de cartas disponíveis

    private void Start()
    {
        if (swapAllButton != null)
            swapAllButton.onClick.AddListener(SwapAllCards);
    }

    private void SwapAllCards()
    {
        if (cardVisualsParent == null || availableCardTypes == null || availableCardTypes.Count == 0)
        {
            Debug.LogWarning("CardVisuals ou CardTypes não configurados corretamente!");
            return;
        }

        foreach (Transform cardFaceTransform in cardVisualsParent)
        {
            CardFace cardFace = cardFaceTransform.GetComponent<CardFace>();
            if (cardFace != null && cardFace._target != null)
            {
                Card card = cardFace._target.GetComponent<Card>();
                if (card != null)
                {
                    // Escolhe um novo tipo de carta aleatoriamente
                    CardType newCardType = availableCardTypes[Random.Range(0, availableCardTypes.Count)];
                    card._cardType = newCardType;

                    // Atualiza a aparência da carta no CardFace
                    cardFace._icon.sprite = newCardType._cardIcon;
                    cardFace._rightNumber.text = newCardType._maxCardNumber.ToString();
                    cardFace._leftNumber.text = newCardType._maxCardNumber.ToString();
                }
            }
        }

        Debug.Log("Todas as cartas foram trocadas!");
    }
}