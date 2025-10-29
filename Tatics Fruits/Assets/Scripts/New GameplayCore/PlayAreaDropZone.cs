using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using New_GameplayCore;
using New_GameplayCore.Views;

public class PlayAreaDropZone : MonoBehaviour, IDropHandler
{
    [Header("Slots de exibição (opcional)")]
    [SerializeField] private Transform dropContent;

    [Header("Referências de lógica")]
    [SerializeField] private GameControllerInitializer bootstrap;

    private IRuleEngine _rules;
    private IGameController _controller;
    private IHandService _hand;
    
    private readonly List<(CardView view, CardDragHandler drag, CardInstance data)> _staged = new(2);

    void Start()
    {
        _rules = bootstrap.RuleEngine;
        _controller = bootstrap.Controller;
        _hand = bootstrap.Hand;
        if (dropContent == null) dropContent = transform;
    }

    public void OnDrop(PointerEventData eventData)
    {
        var go = eventData.pointerDrag;
        if (go == null) return;

        var drag = go.GetComponent<CardDragHandler>();
        var view = go.GetComponent<CardView>();
        if (drag == null || view == null) return;

        var data = view.GetCardData();
        
        if (_staged.Count >= 2)
        {
            drag.ReturnToOrigin();
            return;
        }
        
        if (_staged.Count == 0)
        {
            _hand.TryRemove(data);
            
            drag.AcceptDrop(dropContent);
            _staged.Add((view, drag, data));
            return;
        }
        
        var first = _staged[0];
        bool isPair = _rules.IsValidPair(first.data, data);

        if (!isPair)
        {
            drag.ReturnToOrigin();
            return;
        }
        
        drag.AcceptDrop(dropContent);
        _staged.Add((view, drag, data));
        
        _controller.OnCardSelected(first.data);
        _controller.OnCardSelected(data);
        
        if (first.view) 
            Destroy(first.view.gameObject);
        
        if (view)
            Destroy(view.gameObject);
        
        _staged.Clear();
    }
}
