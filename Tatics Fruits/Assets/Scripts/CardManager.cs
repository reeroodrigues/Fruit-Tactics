using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CardManager : MonoBehaviour
{
    [HideInInspector] public GameObject selectedCard;
    [HideInInspector] public GameObject hoveringMenu;
    [HideInInspector] public CardTypeSo cardType;
    [HideInInspector] public bool swapAllFree = false;

    [Header("Scripts/GameObjects")]
    public GameObject cardParent;
    public GridLayoutGroup defaultCardsLayoutGroup;
    public CardHolder defaultPlayArea;
    public GameObject singleCardsParent;
    public GameObject cardFaces;
    public Canvas canva;
    public Timer timer;
    public Button swapAllButton;
    
    private bool _isRoundOver = false;

    [Header("Settings")]
    [Range(0,12)] public int maxCards = 6;
    [Range(0,12)] public int startingAmount = 6;

    [Header("Lists")]
    public List<CardTypeSo> cardTypes = new List<CardTypeSo>();
    public List<GameObject> cards = new List<GameObject>();

    private void Start()
    {
        if (timer != null)
        {
            timer.OnRoundEnd += HandleRoundEnd;
        }

        if (startingAmount > 0)
            AddCard(startingAmount);
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
        if (selectedCard == null || selectedCard.Equals(null)) return;

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] == null || cards[i].Equals(null)) continue;

            if (selectedCard.transform.position.x > cards[i].transform.position.x)
            {
                if (selectedCard.transform.parent != null && selectedCard.transform.parent.GetSiblingIndex() < cards[i].transform.parent.GetSiblingIndex())
                {
                    SwapCards(selectedCard, cards[i]);
                    break;
                }
            }

            if (selectedCard.transform.position.x < cards[i].transform.position.x)
            {
                if (selectedCard.transform.parent != null && selectedCard.transform.parent.GetSiblingIndex() > cards[i].transform.parent.GetSiblingIndex())
                {
                    SwapCards(selectedCard, cards[i]);
                    break;
                }
            }
        }
    }

    public void PlayCard()
    {
        if (_isRoundOver) return;

        if (selectedCard == null)
            return;

        if (defaultPlayArea.available)
        {
            var target = selectedCard.transform.parent;
            selectedCard.transform.position = defaultPlayArea.transform.position;
            selectedCard.transform.SetParent(defaultPlayArea.transform);
            
            var canvas = selectedCard.GetComponent<Canvas>();
            if (canvas == null)
                canvas = selectedCard.AddComponent<Canvas>();
            
            canvas.overrideSorting = true;
            canvas.sortingOrder = GetNextSortingOrder();

            Destroy(target.gameObject);
            selectedCard = null;
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

        if (currentCard.transform.GetComponent<Card>().cardState != Card.CardState.IsDragging)
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
            if (defaultCardsLayoutGroup.transform.childCount < maxCards)
            {
                var card = Instantiate(cardParent, defaultCardsLayoutGroup.transform);
                var randomCard = Random.Range(0, cardTypes.Count);

                card.GetComponentInChildren<Card>().cardTypeSo = cardTypes[randomCard];
                var cardFace = Instantiate(cardFaces, GameObject.Find("CardVisuals").transform);

                cardFace.GetComponent<CardFace>()._target = card.GetComponentInChildren<Card>().gameObject;
            }
        }
    }
}
