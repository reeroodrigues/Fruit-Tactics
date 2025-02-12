using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CardManager : MonoBehaviour
{
    [HideInInspector] public CardType _cardType;
    [HideInInspector] public GameObject _selectedCard;
    [HideInInspector] public GameObject _hoveringMenu;

    [Header("Scripts/GameObjects")]
    public Canvas _canvas;
    public CardHolder _defaultPlayArea;
    public GameObject _cardParent;
    public GameObject _cardFacePowerup;
    public GameObject _cardFace;
    public GameObject _singleCardsParent;
    public HorizontalLayoutGroup _defaultCardsLayoutGroup;
    
    [Header("Settings")]
    [UnityEngine.Range(0,12)] public int _maxCards = 6;
    [UnityEngine.Range(0,12)] public int _startingAmount = 6;

    [Header("Lists")]
    public List<CardType> _cardTypes = new List<CardType>();
    public List<CardPowerupType> _powerupType = new List<CardPowerupType>();
    public List<GameObject> _cards = new List<GameObject>();
    
    private Dictionary<Card, CardPowerupType> _cardsWithPowerup = new Dictionary<Card, CardPowerupType>();

    private void Start()
    {
        if (_startingAmount > 0)
            AddCard(_startingAmount);
        
        AssignPowerupsToCards();
    }

    private void Update()
    {
        HandleCardMovements();
    }

    private void HandleCardMovements()
    {
        if (_selectedCard == null || _selectedCard.Equals(null)) return;

        for (int i = 0; i < _cards.Count; i++)
        {
            if (_cards[i] == null || _cards[i].Equals(null)) continue;

            if (_selectedCard.transform.position.x > _cards[i].transform.position.x)
            {
                if (_selectedCard.transform.parent != null && _selectedCard.transform.parent.GetSiblingIndex() < _cards[i].transform.parent.GetSiblingIndex())
                {
                    SwapCards(_selectedCard, _cards[i]);
                    break;
                }
            }
        
            if (_selectedCard.transform.position.x < _cards[i].transform.position.x)
            {
                if (_selectedCard.transform.parent != null && _selectedCard.transform.parent.GetSiblingIndex() > _cards[i].transform.parent.GetSiblingIndex())
                {
                    SwapCards(_selectedCard, _cards[i]);
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
            var target = _selectedCard.transform.parent;
            _selectedCard.transform.position = _defaultPlayArea.transform.position;
            _selectedCard.transform.SetParent(_defaultPlayArea.transform);
        
            var canvas = _selectedCard.GetComponent<Canvas>();
            if (canvas == null)
                canvas = _selectedCard.AddComponent<Canvas>();

            canvas.overrideSorting = true;
            canvas.sortingOrder = GetNextSortingOrder();

            // Remover a carta do CardVisuals
            RemoveCardFace(_selectedCard.GetComponent<Card>());

            Destroy(target.gameObject);
            _selectedCard = null;
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
    
    private int _sortingOrder = 1;
    private int GetNextSortingOrder()
    {
        return _sortingOrder++;
    }
    
    private void SwapCards(GameObject currentCard, GameObject targetCard)
    {
        var currentCardParent = currentCard.transform.parent;
        var targetedCardParent = targetCard.transform.parent;
        
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
        Debug.Log("Tentativa de adicionar nova carta.");
        for (int i = 0; i < amount; i++)
        {
            if (_defaultCardsLayoutGroup.transform.childCount < _maxCards)
            {
                var card = Instantiate(_cardParent, _defaultCardsLayoutGroup.transform);
                var randomCard = Random.Range(0, _cardTypes.Count);

                var cardComponent = card.GetComponentInChildren<Card>();
                cardComponent._cardType = _cardTypes[randomCard];
                
                bool isPowerup = Random.value > 0.5f;
                GameObject cardFace;
            
                if (isPowerup)
                {
                    cardFace = Instantiate(_cardFacePowerup, GameObject.Find("CardVisuals").transform);
                    var powerupManager = FindObjectOfType<CardManager>();
                    if (powerupManager != null)
                    {
                        powerupManager.AssignPowerupToCard(cardComponent);
                    }
                }
                else
                {
                    cardFace = Instantiate(_cardFace, GameObject.Find("CardVisuals").transform);
                }

                cardFace.GetComponent<CardFace>()._target = card.GetComponentInChildren<Card>().gameObject;
            }
        }
    }

    
    public void AssignPowerupsToCards()
    {
        foreach (var cardObject in FindObjectsOfType<Card>())
        {
            if (Random.value > 0.5)
            {
                AssignPowerupToCard(cardObject);
            }
        }
    }

    public void AssignPowerupToCard(Card card)
    {
        if (card == null || _powerupType.Count == 0)
            return;

        var randomPowerup = _powerupType[Random.Range(0, _powerupType.Count)];
        _cardsWithPowerup[card] = randomPowerup;
    }

    public void UsePowerupOnCard(Card card)
    {
        if (card == null || !_cardsWithPowerup.ContainsKey(card))
            return;

        CardPowerupType powerup = _cardsWithPowerup[card];
        powerup.ApplyEffect(card);

        _cardsWithPowerup.Remove(card);
    }

    public void UseRandomPowerup()
    {
        if (_cardsWithPowerup.Count == 0)
            return;

        var randomCard = new List<Card>(_cardsWithPowerup.Keys)[Random.Range(0, _cardsWithPowerup.Count)];
        UsePowerupOnCard(randomCard);
    }
    
}
