using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemView : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button buyButton;
    [SerializeField] private GameObject ownedOverlay;
    [SerializeField] private GameObject ribbonNew;

    // dados do item
    private string _cardId;
    private int _price;
    private PlayerProfileController _profile;

    public void Setup(string cardId, string displayName, Sprite sprite, int priceGold, 
                      PlayerProfileController profile, bool isNew = false)
    {
        _cardId = cardId;
        _price = priceGold;
        _profile = profile;

        if (icon) icon.sprite = sprite;
        if (title) title.text = displayName;
        if (priceText) priceText.text = $"{priceGold} Golds";
        if (ribbonNew) ribbonNew.SetActive(isNew);

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnBuyClick);

        // reage a mudanças de gold
        _profile.OnGoldChanged += _ => RefreshState();

        RefreshState();
    }

    private void OnDestroy()
    {
        if (_profile != null)
            _profile.OnGoldChanged -= _ => RefreshState(); // segurança: remover assinatura
    }

    private void OnEnable() => RefreshState();

    private void RefreshState()
    {
        if (_profile == null) return;

        bool owned = _profile.HasCard(_cardId);
        if (ownedOverlay) ownedOverlay.SetActive(owned);

        bool canBuy = !owned && _profile.CanAfford(_price);
        if (buyButton) buyButton.interactable = canBuy;

        // preço meio apagado quando já possui
        if (priceText) priceText.alpha = owned ? 0.5f : 1f;
        if (title) title.alpha = owned ? 0.6f : 1f;
    }

    private void OnBuyClick()
    {
        if (_profile.TryPurchaseCard(_cardId, _price))
        {
            RefreshState();
            // TODO: feedback (sfx, partícula, leve escala com DOTween, etc.)
        }
        else
        {
            // TODO: feedback sem gold
        }
    }
}
