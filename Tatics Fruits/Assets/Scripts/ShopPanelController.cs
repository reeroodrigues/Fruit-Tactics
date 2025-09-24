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

    [Header("Anim")]
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private RectTransform window;
    [SerializeField] private float fadeIn  = 0.18f;
    [SerializeField] private float fadeOut = 0.15f;
    [SerializeField] private float scaleIn = 0.22f;

    [Header("Options")]
    [SerializeField] private bool showFeatured = true;
    [SerializeField] private bool sortByPriceAsc = true;

    private readonly List<ShopItemView> _spawned = new();
    private bool _animating;
    private Action<int> _goldChangedHandler;

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
    }

    private void OnDisable()
    {
        if (profile != null && _goldChangedHandler != null)
            profile.OnGoldChanged -= _goldChangedHandler;
        _goldChangedHandler = null;

        if (profile) profile.ReleaseShowGoldHud();

        if (closeButton) closeButton.onClick.RemoveListener(Hide);

        foreach (var v in _spawned) if (v) Destroy(v.gameObject);
        _spawned.Clear();

        if (gridParent) foreach (Transform t in gridParent) Destroy(t.gameObject);
        if (featuredParent) foreach (Transform t in featuredParent) Destroy(t.gameObject);

        DOTween.Kill(panelCanvasGroup);
        DOTween.Kill(window);

        if (panelCanvasGroup)
        {
            panelCanvasGroup.alpha = 0f;
            panelCanvasGroup.interactable = false;
            panelCanvasGroup.blocksRaycasts = false;
        }
        _animating = false;
    }

    private void OnGoldChanged(int _) => RefreshGold();

    private void RefreshGold()
    {
        if (goldCounter) goldCounter.text = $"{profile.Data.Gold}";
    }

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
}
