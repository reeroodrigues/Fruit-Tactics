using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShopPanelController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerProfileController profile;
    [SerializeField] private List<PowerUpCardSO> catalog;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI goldCounter;
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform gridParent;
    [SerializeField] private ShopItemView itemPrefab;
    [SerializeField] private Transform featuredParent;
    [SerializeField] private ShopItemView featuredPrefab;

    [Header("Scroll")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Button downArrowButton;
    [SerializeField] private Button upArrowButton;

    [Header("Anim")]
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private RectTransform window;
    [SerializeField] private float fadeIn  = 0.18f;
    [SerializeField] private float fadeOut = 0.15f;
    [SerializeField] private float scaleIn = 0.22f;

    [Header("Options")]
    [SerializeField] private bool showFeatured = true;
    [SerializeField] private bool sortByPriceAsc = true;

    [Header("Arrow Options")]
    [SerializeField] private float arrowAppearDelay = 3f;
    [SerializeField] private float arrowScrollStep = 0.35f;
    [SerializeField] private float arrowScrollDuration = 0.35f;
    [SerializeField] private float arrowNudgeDistance = 12f;
    [SerializeField] private float arrowNudgeDuration = 0.6f;
    [SerializeField] private float bottomThreshold = 0.01f;

    private readonly List<ShopItemView> _spawned = new();
    private bool _animating;
    private Action<int> _goldChangedHandler;

    private RectTransform _downRt, _upRt;
    private Vector2 _downStartPos, _upStartPos;

    private void Awake()
    {
        if ((catalog == null || catalog.Count == 0))
            catalog = Resources.LoadAll<PowerUpCardSO>("PowerUps").ToList();

        if (panelCanvasGroup)
        {
            panelCanvasGroup.alpha = 0f;
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = false;
        }
        if (window) window.localScale = Vector3.one * 0.96f;

        if (downArrowButton) downArrowButton.gameObject.SetActive(false);
        if (upArrowButton)   upArrowButton.gameObject.SetActive(false);

        _downRt = downArrowButton ? downArrowButton.transform as RectTransform : null;
        _upRt   = upArrowButton   ? upArrowButton.transform as RectTransform   : null;
        if (_downRt) _downStartPos = _downRt.anchoredPosition;
        if (_upRt)   _upStartPos   = _upRt.anchoredPosition;

        gameObject.SetActive(false);
        _animating = false;
    }

    private void OnEnable()
    {
        if (closeButton)
        {
            closeButton.onClick.RemoveListener(Hide);
            closeButton.onClick.AddListener(Hide);
        }

        if (profile) profile.RequestShowGoldHud();

        RefreshGold();
        BuildCatalog();

        _goldChangedHandler = OnGoldChanged;
        if (profile != null) profile.OnGoldChanged += _goldChangedHandler;
        
        if (downArrowButton)
        {
            downArrowButton.onClick.RemoveListener(OnDownArrowClick);
            downArrowButton.onClick.AddListener(OnDownArrowClick);
        }
        if (upArrowButton)
        {
            upArrowButton.onClick.RemoveListener(OnUpArrowClick);
            upArrowButton.onClick.AddListener(OnUpArrowClick);
        }
        if (scrollRect)
        {
            scrollRect.onValueChanged.RemoveListener(OnScrollChanged);
            scrollRect.onValueChanged.AddListener(OnScrollChanged);
        }
        
        DOTween.Kill(this, complete:false);
        DOVirtual.DelayedCall(arrowAppearDelay, TryShowDownArrow, ignoreTimeScale:true).SetTarget(this);
        
        HideUpArrow();
    }

    private void OnDisable()
    {
        if (profile != null && _goldChangedHandler != null)
            profile.OnGoldChanged -= _goldChangedHandler;
        _goldChangedHandler = null;

        if (profile) profile.ReleaseShowGoldHud();

        if (closeButton) closeButton.onClick.RemoveListener(Hide);

        if (downArrowButton) downArrowButton.onClick.RemoveListener(OnDownArrowClick);
        if (upArrowButton)   upArrowButton.onClick.RemoveListener(OnUpArrowClick);
        if (scrollRect)      scrollRect.onValueChanged.RemoveListener(OnScrollChanged);

        foreach (var v in _spawned) if (v) Destroy(v.gameObject);
        _spawned.Clear();

        if (gridParent) foreach (Transform t in gridParent) Destroy(t.gameObject);
        if (featuredParent) foreach (Transform t in featuredParent) Destroy(t.gameObject);

        DOTween.Kill(panelCanvasGroup);
        DOTween.Kill(window);
        DOTween.Kill(this);

        if (panelCanvasGroup)
        {
            panelCanvasGroup.alpha = 0f;
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = false;
        }

        ResetArrowsVisual();
        _animating = false;
    }

    private void ResetArrowsVisual()
    {
        if (_downRt) _downRt.anchoredPosition = _downStartPos;
        if (_upRt)   _upRt.anchoredPosition   = _upStartPos;
        if (downArrowButton) downArrowButton.gameObject.SetActive(false);
        if (upArrowButton)   upArrowButton.gameObject.SetActive(false);
    }

    private void OnGoldChanged(int _) => RefreshGold();
    private void RefreshGold() { if (goldCounter) goldCounter.text = $"{profile.Data.Gold}"; }

    private IEnumerable<PowerUpCardSO> Enumerate()
    {
        var list = (catalog ?? new List<PowerUpCardSO>()).Where(p => p != null);
        list = sortByPriceAsc ? list.OrderBy(p => p.priceGold).ThenBy(p => p.displayName)
                              : list.OrderBy(p => p.displayName);
        return list;
    }

    public void BuildCatalog()
    {
        if (gridParent) foreach (Transform t in gridParent) Destroy(t.gameObject);
        if (featuredParent) foreach (Transform t in featuredParent) Destroy(t.gameObject);
        _spawned.Clear();

        var list = Enumerate().ToList();
        PowerUpCardSO featured = (showFeatured && featuredPrefab && featuredParent && list.Count > 0) ? list[0] : null;

        if (featured != null)
        {
            var fv = Instantiate(featuredPrefab, featuredParent);
            fv.SetupFromSO(featured, profile, isNew: false);
            _spawned.Add(fv);
            list.RemoveAt(0);
        }

        foreach (var so in list)
        {
            var v = Instantiate(itemPrefab, gridParent);
            v.SetupFromSO(so, profile, isNew: false);
            _spawned.Add(v);
        }

        if (scrollRect) scrollRect.verticalNormalizedPosition = 1f;
    }

    public void Show()
    {
        if (_animating) return;
        _animating = true;

        gameObject.SetActive(true);

        if (panelCanvasGroup)
        {
            panelCanvasGroup.DOKill();
            panelCanvasGroup.alpha = 0f;
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = false;
        }
        if (window)
        {
            window.DOKill();
            window.localScale = Vector3.one * 0.96f;
        }

        DOTween.Sequence()
            .Append(panelCanvasGroup ? panelCanvasGroup.DOFade(1f, fadeIn) : null)
            .Join(window ? window.DOScale(1f, scaleIn).SetEase(Ease.OutBack) : null)
            .OnComplete(() =>
            {
                if (panelCanvasGroup)
                {
                    panelCanvasGroup.interactable = true;
                    panelCanvasGroup.blocksRaycasts = true;
                }
                _animating = false;
            });
    }

    public void Hide()
    {
        if (_animating) return;
        _animating = true;

        if (panelCanvasGroup)
        {
            panelCanvasGroup.DOKill();
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = false;
        }
        if (window) window.DOKill();

        DOTween.Sequence()
            .Append(panelCanvasGroup ? panelCanvasGroup.DOFade(0f, fadeOut) : null)
            .Join(window ? window.DOScale(0.94f, fadeOut).SetEase(Ease.InSine) : null)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                _animating = false;
            });
    }

    private void TryShowDownArrow()
    {
        if (!downArrowButton || !scrollRect) return;
        
        bool atBottom = scrollRect.verticalNormalizedPosition <= bottomThreshold;
        if (!atBottom) ShowDownArrow();
    }

    private void ShowDownArrow()
    {
        if (!downArrowButton) return;

        if (!downArrowButton.gameObject.activeSelf)
            downArrowButton.gameObject.SetActive(true);
        
        if (_downRt)
        {
            _downRt.DOKill();
            _downRt.anchoredPosition = _downStartPos;
            _downRt.DOAnchorPos(_downStartPos - new Vector2(0f, arrowNudgeDistance), arrowNudgeDuration)
                   .SetLoops(-1, LoopType.Yoyo)
                   .SetEase(Ease.InOutSine)
                   .SetTarget(this);
        }
    }

    private void HideDownArrow()
    {
        if (!downArrowButton) return;
        if (_downRt) _downRt.DOKill();
        if (_downRt) _downRt.anchoredPosition = _downStartPos;
        downArrowButton.gameObject.SetActive(false);
    }

    private void ShowUpArrow()
    {
        if (!upArrowButton) return;

        if (!upArrowButton.gameObject.activeSelf)
            upArrowButton.gameObject.SetActive(true);
        
        if (_upRt)
        {
            _upRt.DOKill();
            _upRt.anchoredPosition = _upStartPos;
            _upRt.DOAnchorPos(_upStartPos + new Vector2(0f, arrowNudgeDistance), arrowNudgeDuration)
                 .SetLoops(-1, LoopType.Yoyo)
                 .SetEase(Ease.InOutSine)
                 .SetTarget(this);
        }
    }

    private void HideUpArrow()
    {
        if (!upArrowButton) return;
        if (_upRt) _upRt.DOKill();
        if (_upRt) _upRt.anchoredPosition = _upStartPos;
        upArrowButton.gameObject.SetActive(false);
    }

    private void OnDownArrowClick()
    {
        if (!scrollRect) return;

        float current = scrollRect.verticalNormalizedPosition;
        float target  = Mathf.Max(0f, current - arrowScrollStep);

        DOTween.Kill(scrollRect);
        scrollRect.DOVerticalNormalizedPos(target, arrowScrollDuration)
                  .SetEase(Ease.OutCubic);
        
        DOVirtual.DelayedCall(arrowScrollDuration * 0.95f, () =>
        {
            bool atBottom = scrollRect.verticalNormalizedPosition <= bottomThreshold;
            if (atBottom)
            {
                HideDownArrow();
                ShowUpArrow();
            }
        }).SetTarget(this);
    }

    private void OnUpArrowClick()
    {
        if (!scrollRect) return;

        DOTween.Kill(scrollRect);
        scrollRect.DOVerticalNormalizedPos(1f, arrowScrollDuration)
                  .SetEase(Ease.OutCubic);
        
        DOVirtual.DelayedCall(arrowScrollDuration * 0.95f, () =>
        {
            HideUpArrow();
            ShowDownArrow();
        }).SetTarget(this);
    }

    private void OnScrollChanged(Vector2 _)
    {
        if (!scrollRect) return;

        bool atBottom = scrollRect.verticalNormalizedPosition <= bottomThreshold;

        if (atBottom)
        {
            HideDownArrow();
            ShowUpArrow();
        }
        else
        {
            HideUpArrow();
            if (downArrowButton && !downArrowButton.gameObject.activeSelf)
                ShowDownArrow();
        }
    }
}
