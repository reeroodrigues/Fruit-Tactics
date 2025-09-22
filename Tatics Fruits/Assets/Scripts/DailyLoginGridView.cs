using System.Collections.Generic;
using UnityEngine;

public class DailyLoginGridView : MonoBehaviour
{
    [Header("Logic")]
    [SerializeField] private DailyMissionsController controller;

    [Header("UI")]
    [SerializeField] private Transform gridParent;                 // onde instanciar
    [SerializeField] private DailyLoginDayItemView dayPrefab;      // dias 1..6
    [SerializeField] private DailyLoginDayItemView specialPrefab;  // dia 7

    private readonly List<DailyLoginDayItemView> _items = new();

    private void OnEnable()
    {
        Build();
        if (controller != null)
            controller.OnDailyLoginChanged += Refresh;
    }

    private void OnDisable()
    {
        if (controller != null)
            controller.OnDailyLoginChanged -= Refresh;
    }

    public void Build()
    {
        foreach (Transform t in gridParent) Destroy(t.gameObject);
        _items.Clear();

        var days = controller.GetLoginDays(); // sempre 7

        for (int i = 0; i < days.Count; i++)
        {
            var info = days[i];
            var prefab = (i == 6 && specialPrefab != null) ? specialPrefab : dayPrefab;

            var item = Instantiate(prefab, gridParent);
            item.Setup(controller, info);
            _items.Add(item);
        }
    }

    public void Refresh()
    {
        var days = controller.GetLoginDays();
        for (int i = 0; i < _items.Count && i < days.Count; i++)
        {
            _items[i].Refresh(days[i]);
        }
    }
}