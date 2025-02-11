using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // Importar o namespace do TextMeshPro

public class CardHolder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CardManager _cardManager;
    public bool _available;
    public bool _completed;
    public bool _hasToHaveSameNumberOrColor;
    public int _maxAmount;
    public int _amountToComplete;

    public HolderType _holderType;

    public ScoreManager _scoreManager; // Referência ao ScoreManager para atualizar o score
    public TextMeshProUGUI _scoreText; // Referência ao componente TextMeshProUGUI para exibir o score

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
        CheckForMatchingCards();

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
    
    private void CheckForMatchingCards()
    {
        if (_holderType != HolderType.Play) return;

        Dictionary<string, List<Card>> matchingCards = new Dictionary<string, List<Card>>();

        foreach (Transform child in transform)
        {
            Card card = child.GetComponent<Card>();
            if (card == null) continue;

            string key = $"{card._cardNumber}_{card._cardType._cardIcon}";

            if (!matchingCards.ContainsKey(key))
                matchingCards[key] = new List<Card>();

            matchingCards[key].Add(card);
        }

        List<GameObject> cardsToRemove = new List<GameObject>();

        foreach (var pair in matchingCards)
        {
            if (pair.Value.Count >= 2)
            {
                int sumValue = 0;

                foreach (Card card in pair.Value)
                {
                    sumValue += card._cardNumber;
                    cardsToRemove.Add(card.gameObject);
                }

                Debug.Log($"Cartas combinadas! Novo valor: {sumValue}");

                // Atualizando o score com o valor somado
                if (_scoreManager != null)
                {
                    _scoreManager.AddScore(sumValue);
                    Debug.Log($"Score Atualizado: {_scoreManager.GetScore()}");

                    // Atualizando o texto na tela com o novo score
                    UpdateScoreText();
                }
            }
        }

        foreach (GameObject card in cardsToRemove)
        {
            card.transform.SetParent(null);
            Destroy(card);
        }
    }

    private void UpdateScoreText()
    {
        if (_scoreText != null)
        {
            _scoreText.text = "Score: " + _scoreManager.GetScore().ToString();
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
                    Transform lastChild = transform.GetChild(transform.childCount - 1);
                    if (lastChild != null)
                    {
                        Card lastCard = lastChild.GetComponent<Card>();

                        if (lastCard != null && _cardManager._selectedCard.GetComponent<Card>() != null)
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
                            _available = true;
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
                        _available = true;
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
