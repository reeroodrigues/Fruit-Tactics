using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanelController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerProfileController profile;
    [SerializeField] private List<CardTypeSo> catalog;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI goldCounter;
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform gridParent;
    [SerializeField] private ShopItemView itemPrefab;
    [SerializeField] private Transform featuredParent;
    [SerializeField] private ShopItemView featuredPrefab;
    

    private void Awake()
    {
        if (closeButton)
            closeButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private void OnEnable()
    {
        RefreshGold();
        BuildCatalog();
        profile.OnGoldChanged += _ => RefreshGold();
    }

    private void OnDisable()
    {
        profile.OnGoldChanged -= _ => RefreshGold();
    }

    private void RefreshGold()
    {
        if (goldCounter)
            goldCounter.text = $"💰{profile.Data.Gold}";
    }

    private void BuildCatalog()
    {
        foreach (Transform t in gridParent)
            Destroy(t.gameObject);

        if (featuredParent != null)
            foreach (Transform t in featuredParent)
                Destroy(t.gameObject);

        CardTypeSo featured = null;
        if (catalog != null && catalog.Count > 0)
            featured = catalog[0];
        
        if (featuredParent != null && featuredPrefab != null && featured != null)
        {
            var go = Instantiate(featuredPrefab, featuredParent);
            go.Setup(featured.id, featured.displayName, featured.cardIcon, featured.priceGold, profile, isNew:true);
        }
        
        foreach (var so in catalog)
        {
            if (featured != null && so == featured)
                continue;

            var item = Instantiate(itemPrefab, gridParent);
            item.Setup(so.id, so.displayName, so.cardIcon, so.priceGold, profile);
        }
    }
}