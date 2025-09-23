using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    
    [Header("Options")]
    [SerializeField] private bool showFeatured = true;
    [SerializeField] private bool sortByPriceAsc = true;
    
    private readonly List<ShopItemView> _spawned = new();
    

    private void Awake()
    {
        if (closeButton)
            closeButton.onClick.AddListener(() => gameObject.SetActive(false));
        
        if ((catalog == null || catalog.Count == 0))
            catalog = Resources.LoadAll<PowerUpCardSO>("PowerUps").ToList();
    }

    private void OnEnable()
    {
        if(profile)
            profile.RequestShowGoldHud();
            
        RefreshGold();
        BuildCatalog();
        profile.OnGoldChanged += _ => RefreshGold();
    }

    private void OnDisable()
    {
        profile.OnGoldChanged -= _ => RefreshGold();

        if (profile)
            profile.ReleaseShowGoldHud();
        
        if (closeButton) closeButton.onClick.RemoveAllListeners();
        
        foreach (var v in _spawned) if (v) Destroy(v.gameObject);
        _spawned.Clear();
        if (gridParent) foreach (Transform t in gridParent) Destroy(t.gameObject);
        if (featuredParent) foreach (Transform t in featuredParent) Destroy(t.gameObject);
    }

    private void RefreshGold()
    {
        if (goldCounter)
            goldCounter.text = $"{profile.Data.Gold}";
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
            fv.Setup(featured.Id, featured.displayName, featured.icon, featured.priceGold, profile, isNew: false, rarity: featured.rarity);
            _spawned.Add(fv);
            list.RemoveAt(0);
        }
        
        foreach (var so in list)
        {
            var v = Instantiate(itemPrefab, gridParent);
            v.Setup(so.Id, so.displayName, so.icon, so.priceGold, profile, isNew: false, rarity: so.rarity);
            _spawned.Add(v);
        }
    }
}