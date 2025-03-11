using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CardManager : MonoBehaviour
{
    [HideInInspector] public GameObject _selectedCard;
    [HideInInspector] public GameObject _hoveringMenu;
    [HideInInspector] public CardTypeSo _cardType;

    [Header("Scripts/GameObjects")]
    public GameObject _cardParent;
    public HorizontalLayoutGroup _defaultCardsLayoutGroup;
    public CardHolder _defaultPlayArea;
    public GameObject _singleCardsParent;
    public GameObject _cardFace;
    public Canvas _canvas;
    public Timer _timer;
    
    private bool _isRoundOver = false;

    [Header("Settings")]
    [Range(0,12)] public int _maxCards = 6;
    [Range(0,12)] public int _startingAmount = 6;

    [Header("Lists")]
    public List<CardTypeSo> _cardTypes = new List<CardTypeSo>();
    public List<GameObject> _cards = new List<GameObject>();

    private void Start()
    {
        if (_timer != null)
        {
            _timer.OnRoundEnd += HandleRoundEnd;
        }

        if (_startingAmount > 0)
            AddCard(_startingAmount);
    }
    
    private void Update()
    {
        if (_isRoundOver) return;

        HandleCardMovements();
        
        if (Input.GetMouseButtonUp(0))
        {
            PlayCard();
        }
    }

    private void HandleRoundEnd()
    {
        _isRoundOver = true;
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
        if (_isRoundOver) return;

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

            Destroy(target.gameObject);
            _selectedCard = null;
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
        if (_isRoundOver) return;

        for (int i = 0; i < amount; i++)
        {
            if (_defaultCardsLayoutGroup.transform.childCount < _maxCards)
            {
                var card = Instantiate(_cardParent, _defaultCardsLayoutGroup.transform);
                var randomCard = Random.Range(0, _cardTypes.Count);

                card.GetComponentInChildren<Card>()._cardTypeSo = _cardTypes[randomCard];
                var cardFace = Instantiate(_cardFace, GameObject.Find("CardVisuals").transform);

                cardFace.GetComponent<CardFace>()._target = card.GetComponentInChildren<Card>().gameObject;
            }
        }
    }
}
