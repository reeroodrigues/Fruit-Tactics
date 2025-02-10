using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CardManager : MonoBehaviour
{
    [HideInInspector] public GameObject _selectedCard;
    [HideInInspector] public GameObject _hoveringMenu;
    [HideInInspector] public CardType _cardType;

    [Header("Scripts/GameObjects")]
    public GameObject _cardParent;
    public HorizontalLayoutGroup _defaultCardsLayoutGroup;
    public CardHolder _defaultPlayArea;
    public GameObject _singleCardsParent;
    public GameObject _cardFace;
    public Canvas _canvas;
    
    [Header("Settings")]
    [Range(0,12)] public int _maxCards = 6;
    [Range(0,12)] public int _startingAmount = 6;

    [Header("Lists")]
    public List<CardType> CardTypes = new List<CardType>();
    public List<GameObject> Cards = new List<GameObject>();

    private void Start()
    {
        if (_startingAmount > 0)
            AddCard(_startingAmount);
    }

    

    private void Update()
    {
        HandleCardMovements();
    }

    private void HandleCardMovements()
    {
        if (_selectedCard == null)
            return;

        for (int i = 0; i < Cards.Count; i++)
        {
            if (_selectedCard.transform.position.x > Cards[i].transform.position.x)
            {
                if (_selectedCard.transform.parent.GetSiblingIndex() < Cards[i].transform.parent.GetSiblingIndex())
                {
                    SwapCards(_selectedCard, Cards[i]);
                    break;
                }
            }
            
            if (_selectedCard.transform.position.x < Cards[i].transform.position.x)
            {
                if (_selectedCard.transform.parent.GetSiblingIndex() > Cards[i].transform.parent.GetSiblingIndex())
                {
                    SwapCards(_selectedCard, Cards[i]);
                    break;
                }
            }
        }
    }

    public void PlayCard()
    {
        if (_selectedCard == null)
            return;

        if (_defaultPlayArea._available)
        {
            Transform target = _selectedCard.transform.parent;
            _selectedCard.transform.position = _defaultPlayArea.transform.position;
            _selectedCard.transform.SetParent(_defaultPlayArea.transform);
            Destroy(target.gameObject);

            _selectedCard = null;
        }
    }
    
    private void SwapCards(GameObject currentCard, GameObject targetCard)
    {
        Transform currentCardParent = currentCard.transform.parent;
        Transform targetedCardParent = targetCard.transform.parent;
        
        currentCard.transform.SetParent(targetedCardParent);
        targetCard.transform.SetParent(currentCardParent);

        if (currentCard.transform.GetComponent<Card>()._cardState != Card.CardState.IsDragging)
        {
            currentCard.transform.localPosition = Vector2.zero;
        }

        targetCard.transform.localPosition = Vector2.zero;

        GetComponent<AudioSource>().Play();
    }
    
    public void AddCard(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (_defaultCardsLayoutGroup.transform.childCount < _maxCards)
            {
                GameObject card = Instantiate(_cardParent, _defaultCardsLayoutGroup.transform);
                int randomCard = Random.Range(0, CardTypes.Count);

                card.GetComponentInChildren<Card>()._cardType = CardTypes[randomCard];
                GameObject cardFace = Instantiate(_cardFace, GameObject.Find("CardVisuals").transform);
                
                cardFace.GetComponent<CardFace>()._target = card.GetComponentInChildren<Card>().gameObject;
            }
        }
    }
    
}
