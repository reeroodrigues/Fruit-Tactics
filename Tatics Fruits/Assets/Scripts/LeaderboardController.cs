using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardController : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] RectTransform panel;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Image overlay; // Image full-screen, raycastTarget=ON (bloqueia cliques por baixo)

    [Header("List")]
    [SerializeField] ScrollRect scroll;
    [SerializeField] Transform content;
    [SerializeField] LeaderboardEntryView rowPrefab;
    [SerializeField] GameObject emptyState;

    [Header("Misc")]
    [SerializeField] Button backButton;
    [SerializeField] Button refreshButton;
    [SerializeField] string currentPlayerId = "player_001";

    private LeaderboardPayLoad _data;
    private readonly List<LeaderboardEntryView> _rows = new();

    private void Awake()
    {
        if (backButton) backButton.onClick.AddListener(Close);
        if (refreshButton) refreshButton.onClick.AddListener(RefreshData);

        // Garante bloqueio de input: o Ranking deve capturar todos os cliques
        if (canvasGroup)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        if (overlay)
        {
            overlay.raycastTarget = true; // IMPORTANTÍSSIMO
            overlay.gameObject.SetActive(true);
        }
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
        DOTween.Sequence()
            .Append(panel.DOAnchorPosX(0f, 0.25f).SetEase(Ease.OutQuad))
            .Join(canvasGroup.DOFade(1f, 0.25f));
    }

    public void Close()
    {
        DOTween.Sequence()
            .Append(panel.DOAnchorPosX(60f, 0.18f).SetEase(Ease.InQuad))
            .Join(canvasGroup.DOFade(0f, 0.18f))
            .OnComplete(() => gameObject.SetActive(false));
    }

    private void RefreshData()
    {
        _data = LeaderboardService.LoadLocal();
        RenderList(_data?.allTime ?? _data?.daily); // mostra all-time; se não houver, cai pro daily
    }

    private void RenderList(LeaderboardEntry[] list)
    {
        // limpa
        foreach (var r in _rows) if (r) Destroy(r.gameObject);
        _rows.Clear();

        bool hasData = list != null && list.Length > 0;
        if (emptyState) emptyState.SetActive(!hasData);
        if (!hasData) return;

        float delay = 0f;
        foreach (var e in list)
        {
            var row = Instantiate(rowPrefab, content);
            row.transform.localScale = Vector3.one * 0.98f;
            var cg = row.GetComponent<CanvasGroup>();
            if (cg) cg.alpha = 0f;

            bool isCurrent = !string.IsNullOrEmpty(currentPlayerId) && e.playerId == currentPlayerId;
            row.Bind(e, isCurrent);

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
}
