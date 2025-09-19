using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum RankScope { Daily, Weekly, AllTime }

public class LeaderboardController : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private CanvasGroup canvasGroup;
    
    [Header("Tabs")]
    [SerializeField] private Button chipDaily;
    [SerializeField] private Button chipWeekly;
    [SerializeField] private Button chipAllTime;
    [SerializeField] private Image chipDailyBg;
    [SerializeField] private Image chipWeeklyBg;
    [SerializeField] private Image chipAllTimeBg;
    [SerializeField, Range(0, 1)] private float unselectedAlpha = 0.5f;
    
    [Header("List")]
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private Transform content;
    [SerializeField] private LeaderboardEntryView rowPrefab;
    [SerializeField] private GameObject emptyState;

    [Header("Misc")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button refreshButton;
    [SerializeField] private string currentPlayer = "player_001"; //substituir quando tiver login

    private LeaderboardPayLoad _data;
    private RankScope _scope = RankScope.Daily;
    private readonly List<LeaderboardEntryView> _rows = new();

    private void Awake()
    {
        chipDaily.onClick.AddListener(() => SetScope(RankScope.Daily));
        chipWeekly.onClick.AddListener(() => SetScope(RankScope.Weekly));
        chipAllTime.onClick.AddListener(() => SetScope(RankScope.AllTime));
        backButton.onClick.AddListener(Close);
        refreshButton.onClick.AddListener(RefreshData);
    }

    private void OnEnable()
    {
        Open();
        RefreshData();
    }

    private void Open()
    {
        panel.anchoredPosition = new Vector2(60f, panel.anchoredPosition.y);
        canvasGroup.alpha = 0f;
        DOTween.Sequence().Append(panel.DOAnchorPosX(0f, 0.25f).SetEase(Ease.OutQuad)).Join(canvasGroup.DOFade(1f, 0.25f));
    }

    private void Close()
    {
        DOTween.Sequence().Append(panel.DOAnchorPosX(60f, 0.18f).SetEase(Ease.InQuad)).Join(canvasGroup.DOFade(0f, 0.18f)).OnComplete(() => gameObject.SetActive(false));
    }

    private void RefreshData()
    {
        _data = LeaderboardService.LoadLocal();
        RenderScope();
    }

    private void SetScope(RankScope scope)
    {
        _scope = scope;
        RenderScope();
        AnimateChip(chipDailyBg, scope == RankScope.Daily);
        AnimateChip(chipWeeklyBg, scope == RankScope.Weekly);
        AnimateChip(chipAllTimeBg, scope == RankScope.AllTime);
    }

    private void AnimateChip(Image img, bool selected)
    {
        if(!img)
            return;
        var c = img.color;
        c.a = selected ? 1f : unselectedAlpha;
        img.color = c;
        img.transform.DOKill();
        img.transform.DOScale(selected ? 1.0f : 0.96f, 0.12f).SetEase(Ease.OutQuad);
    }

    private void RenderScope()
    {
        foreach (var r in _rows)
        {
            if (r)
            {
                Destroy(r.gameObject);
            }
            _rows.Clear();
        }

        var list = GetCurrentList();
        emptyState?.SetActive(list == null || list.Length == 0);

        if (list == null)
            return;

        float delay = 0f;
        foreach (var e in list)
        {
            var row = Instantiate(rowPrefab, content);
            row.transform.localScale = Vector3.one * 0.98f;
            row.GetComponent<CanvasGroup>()?.SetAlpha(0f);

            bool isCurrent = !string.IsNullOrEmpty(currentPlayer) && e.playerId == currentPlayer;
            row.Bind(e, isCurrent);

            var cg = row.GetComponent<CanvasGroup>();
            DOTween.Sequence()
                .SetDelay(delay)
                .Append(row.transform.DOScale(1f, 0.15f).SetEase(Ease.OutQuad))
                .Join(cg ? cg.DOFade(1f, 0.15f) : null)
                .Join(row.transform.DOLocalMoveY(row.transform.localPosition.y + 12f, 0.15f).From().SetEase(Ease.OutQuad));

            _rows.Add(row);
            delay += 0.03f;
        }
        
        scroll.verticalNormalizedPosition = 1f;
    }
    
    private LeaderboardEntry[] GetCurrentList()
    {
        return _scope switch
        {
            RankScope.Daily   => _data?.daily,
            RankScope.Weekly  => _data?.weekly,
            RankScope.AllTime => _data.allTime,
            _ => null
        };
    }
}

// Extensão utilitária opcional:
static class CanvasGroupExt {
    public static void SetAlpha(this CanvasGroup cg, float a) { if (cg) cg.alpha = a; }
}