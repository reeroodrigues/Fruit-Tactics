using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ShopItemView : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;

    // Texto de preço FORA do botão (opcional). Se você mostra o preço DENTRO do botão,
    // deixe nulo ou aponte para um TMP diferente do label do botão.
    [SerializeField] private TextMeshProUGUI priceText;

    [SerializeField] private TextMeshProUGUI rarityText;

    [SerializeField] private Button buyButton;
    [SerializeField] private Image  buyButtonGraphic;

    // TMP que fica DENTRO do botão "Comprar"
    [SerializeField] private TextMeshProUGUI buyLabel;

    [SerializeField] private GameObject ownedOverlay;
    [SerializeField] private GameObject ribbonNew;

    [Header("Behavior")]
    [SerializeField] private bool disableButtonWhenCantAfford = false;

    // Mostra o preço dentro do botão
    [SerializeField] private bool showPriceInsideButton = true;

    // NOVO: quando verdadeiro, o botão mostra SOMENTE o valor (ex.: "100")
    [SerializeField] private bool showOnlyPriceInButton = true;

    // Se quiser complementar com a palavra "Gold"
    [SerializeField] private bool showGoldWordInButton = false;

    [Header("Localization Keys")]
    [SerializeField] private string buyTextKey   = "shop.buy";
    [SerializeField] private string ownedTextKey = "shop.owned";
    [SerializeField] private string goldTextKey  = "currency.gold";

    // Injete sua função de tradução em algum bootstrap: ShopItemView.Translate = Localizer.Instance.Tr;
    public static Func<string, string> Translate = key => key;

    [Header("FX")]
    [SerializeField] private float cantAffordShakeDuration = 0.35f;
    [SerializeField] private float cantAffordShakeStrength = 0.25f;
    [SerializeField] private int   cantAffordShakeVibrato  = 18;
    [SerializeField] private Color cantAffordFlashColor    = new Color(1f, 0.25f, 0.25f, 1f);
    [SerializeField] private float cantAffordFlashDuration = 0.20f;

    [SerializeField] private float purchasePunchDuration   = 0.15f;
    [SerializeField] private float purchasePunchScale      = 0.12f;

    private string _cardId;
    private int _price;
    private PlayerProfileController _profile;

    private Color _btnOriginalColor = Color.white;
    private Tween _activeTween;

    private string T(string key) => (Translate != null ? Translate(key) : key) ?? key;

    private void Awake()
    {
        // Se não foi atribuído no Inspector, tenta achar no botão
        if (!buyLabel && buyButton)
            buyLabel = buyButton.GetComponentInChildren<TextMeshProUGUI>(true);

        if (!buyButtonGraphic && buyButton)
            buyButtonGraphic = buyButton.targetGraphic as Image;

        if (buyButtonGraphic)
            _btnOriginalColor = buyButtonGraphic.color;

        // Garante que o targetGraphic do Button é a imagem de fundo (não o TMP)
        if (buyButton && buyButtonGraphic && buyButton.targetGraphic != buyButtonGraphic)
            buyButton.targetGraphic = buyButtonGraphic;
    }

    public void Setup(string cardId, string displayName, Sprite sprite, int priceGold,
                      PlayerProfileController profile, bool isNew = false, string rarity = null)
    {
        _cardId  = cardId;
        _price   = priceGold;
        _profile = profile;

        if (icon)  icon.sprite = sprite;
        if (title) title.text  = displayName;

        if (priceText) priceText.text = $"{priceGold} {T(goldTextKey)}";
        if (ribbonNew) ribbonNew.SetActive(isNew);
        if (rarityText) rarityText.text = string.IsNullOrEmpty(rarity) ? "" : rarity;

        if (buyButton)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuyClick);
        }

        _profile.OnGoldChanged -= HandleGoldChanged;
        _profile.OnGoldChanged += HandleGoldChanged;

        RefreshState();
    }

    private void OnEnable() => RefreshState();

    private void OnDisable()
    {
        if (buyButton)        buyButton.transform.DOKill();
        if (buyButtonGraphic) buyButtonGraphic.DOKill();
        _activeTween?.Kill();
        _activeTween = null;
    }

    private void OnDestroy()
    {
        if (_profile != null)
            _profile.OnGoldChanged -= HandleGoldChanged;
    }

    private void HandleGoldChanged(int _) => RefreshState();

    private void RefreshState()
    {
        if (_profile == null) return;

        bool owned = _profile.HasCard(_cardId);
        if (ownedOverlay) ownedOverlay.SetActive(owned);

        bool hasMoney = _profile.CanAfford(_price);

        bool interactable = !owned && (!disableButtonWhenCantAfford || hasMoney);
        if (buyButton) buyButton.interactable = interactable;

        // Label dentro do botão
        if (buyLabel)
        {
            buyLabel.gameObject.SetActive(true);
            buyLabel.enabled = true;
            buyLabel.alpha   = 1f;

            string buyWord   = T(buyTextKey);
            string ownedWord = T(ownedTextKey);
            string goldWord  = T(goldTextKey);

            if (owned)
            {
                buyLabel.text = ownedWord;
            }
            else if (showPriceInsideButton)
            {
                if (showOnlyPriceInButton)
                {
                    // SOMENTE o valor (com ou sem "Gold")
                    buyLabel.text = showGoldWordInButton ? $"{_price} {goldWord}" : $"{_price}";
                }
                else
                {
                    // Texto + preço (modo antigo)
                    buyLabel.text = $"{buyWord}\n{_price} {goldWord}";
                }
            }
            else
            {
                // Sem preço no botão, apenas o texto "Comprar"
                buyLabel.text = buyWord;
            }
        }

        // Se houver um priceText separado, só esconda se NÃO for o mesmo label do botão
        if (priceText && priceText != buyLabel)
            priceText.gameObject.SetActive(!showPriceInsideButton);

        if (title) title.alpha = owned ? 0.6f : 1f;

        if (buyButtonGraphic) buyButtonGraphic.color = _btnOriginalColor;
        if (buyButton)        buyButton.transform.localScale = Vector3.one;
    }

    private void OnBuyClick()
    {
        if (_profile.HasCard(_cardId)) return;

        if (_profile.TryPurchaseCard(_cardId, _price))
        {
            if (buyButton)
            {
                buyButton.transform.DOKill();
                buyButton.transform.DOPunchScale(Vector3.one * purchasePunchScale, purchasePunchDuration, 8, 0.9f);
            }
            RefreshState();
        }
        else
        {
            FeedbackCantAfford();
        }
    }

    private void FeedbackCantAfford()
    {
        if (buyButton)
        {
            buyButton.transform.DOKill();
            buyButton.transform.localScale = Vector3.one;
            buyButton.transform.DOShakeScale(
                duration:  cantAffordShakeDuration,
                strength:  cantAffordShakeStrength,
                vibrato:   cantAffordShakeVibrato,
                randomness: 90f
            );
        }

        if (buyButtonGraphic)
        {
            buyButtonGraphic.DOKill();
            buyButtonGraphic.color = _btnOriginalColor;
            var seq = DOTween.Sequence();
            seq.Append(buyButtonGraphic.DOColor(cantAffordFlashColor, cantAffordFlashDuration * 0.5f));
            seq.Append(buyButtonGraphic.DOColor(_btnOriginalColor, cantAffordFlashDuration * 0.5f));
            _activeTween = seq;
        }
    }
}
