using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerProfileController : MonoBehaviour
{
    [Header("UI / Perfil")]
    [SerializeField] private GameObject _profilePanel;
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private Image _avatarImage;
    [SerializeField] private Sprite[] _avatars;
    [SerializeField] private TMP_InputField _playerNameInput;
    [SerializeField] private TextMeshProUGUI _playerNameErrorText;
    [SerializeField] private GameObject _avatarSelectionPanel;
    [SerializeField] private Button _closeAvatarPanelButton;

    [Header("UI / Economia")]
    [SerializeField] private TextMeshProUGUI _goldText;

    [Header("Deck")]
    [SerializeField] private int _deckLimit = 5;

    private const string FileName = "player_profile.json";
    private const string NameRegex = @"^[A-Za-zÀ-ÖØ-öø-ÿ\s]+$";

    // Dados em memória
    public PlayerProfileData Data { get; private set; } = new PlayerProfileData();
    
    public event Action<int> OnGoldChanged; // dispara com o valor atual de Gold 

    private void Start()
    {
        // 1) Tenta carregar JSON. Se não existir, cria novo.
        if (!JsonDataService.TryLoad<PlayerProfileData>(FileName, out var loaded))
        {
            Data = new PlayerProfileData();
            MigrateFromPlayerPrefsIfNeeded(); // primeira vez: busca PlayerPrefs antigos
            Save();                           // já salva o arquivo inicial
        }
        else
        {
            Data = loaded ?? new PlayerProfileData();
            // fallback de listas nulas (caso antigo)
            if (Data.OwnedCards == null) Data.OwnedCards = new System.Collections.Generic.List<string>();
            if (Data.EquippedDeck == null) Data.EquippedDeck = new System.Collections.Generic.List<string>();
        }

        // 2) UI inicial
        ApplyProfileUI();
        UpdateGoldUI();

        // 3) Listeners
        if (_playerNameInput != null)
            _playerNameInput.onEndEdit.AddListener(ValidatePlayerName);
        if (_closeAvatarPanelButton != null)
            _closeAvatarPanelButton.onClick.AddListener(CloseAvatarSelection);
    }

    private void MigrateFromPlayerPrefsIfNeeded()
    {
        // Migra apenas se estiver com valores padrão
        if (Data.PlayerName == "Jogador")
        {
            var legacyName = PlayerPrefs.GetString("PlayerName", "Jogador");
            if (!string.IsNullOrWhiteSpace(legacyName))
                Data.PlayerName = legacyName;
        }

        if (Data.AvatarIndex == 0)
        {
            var legacyAvatar = PlayerPrefs.GetInt("AvatarIndex", 0);
            Data.AvatarIndex = Mathf.Clamp(legacyAvatar, 0, _avatars != null && _avatars.Length > 0 ? _avatars.Length - 1 : 0);
        }

        // Se quiser, limpe os antigos:
        // PlayerPrefs.DeleteKey("PlayerName");
        // PlayerPrefs.DeleteKey("AvatarIndex");
        // PlayerPrefs.Save();
    }

    private void ApplyProfileUI()
    {
        if (_playerNameText)  _playerNameText.text = Data.PlayerName;
        if (_playerNameInput) _playerNameInput.text = Data.PlayerName;

        if (_avatars != null && _avatars.Length > 0 && _avatarImage)
        {
            var idx = Mathf.Clamp(Data.AvatarIndex, 0, _avatars.Length - 1);
            _avatarImage.sprite = _avatars[idx];
        }
    }

    private void UpdateGoldUI()
    {
        if (_goldText) _goldText.text = Data.Gold.ToString();
        OnGoldChanged?.Invoke(Data.Gold);
    }

    private void Save() => JsonDataService.Save(FileName, Data);

    // ============
    // Ações de UI
    // ============

    public void OpenProfile()  => _profilePanel?.SetActive(true);
    public void CloseProfile() => _profilePanel?.SetActive(false);

    public void OpenAvatarSelection()  => _avatarSelectionPanel?.SetActive(true);
    public void CloseAvatarSelection() => _avatarSelectionPanel?.SetActive(false);

    public void ChangeAvatar()
    {
        if (_avatars == null || _avatars.Length == 0) return;
        Data.AvatarIndex = (Data.AvatarIndex + 1) % _avatars.Length;
        _avatarImage.sprite = _avatars[Data.AvatarIndex];
        Save();
    }

    public void SelectAvatar(int avatarIndex)
    {
        if (_avatars == null || _avatars.Length == 0) return;
        Data.AvatarIndex = Mathf.Clamp(avatarIndex, 0, _avatars.Length - 1);
        _avatarImage.sprite = _avatars[Data.AvatarIndex];
        Save();
        CloseAvatarSelection();
    }

    private void ValidatePlayerName(string name)
    {
        if (!Regex.IsMatch(name, NameRegex))
        {
            if (_playerNameErrorText) _playerNameErrorText.text = "Nome inválido! Apenas letras são permitidas.";
            if (_playerNameInput)     _playerNameInput.text = Data.PlayerName; // restaura
        }
        else
        {
            if (_playerNameErrorText) _playerNameErrorText.text = "";
            if (_playerNameText)      _playerNameText.text = name;
            Data.PlayerName = name;
            Save();
        }
    }

    // ======================
    // Economia & Coleção
    // ======================

    public bool CanAfford(int price) => Data.Gold >= price;

    public void AddGold(int amount)
    {
        if (amount == 0) return;
        Data.Gold = Mathf.Max(0, Data.Gold + amount);
        Save();
        UpdateGoldUI();
    }

    public bool TrySpendGold(int amount)
    {
        if (!CanAfford(amount)) return false;
        Data.Gold -= amount;
        Save();
        UpdateGoldUI();
        return true;
    }

    public bool HasCard(string cardId)
        => !string.IsNullOrEmpty(cardId) && Data.OwnedCards.Any(id => id == cardId);

    public bool TryPurchaseCard(string cardId, int price)
    {
        if (string.IsNullOrEmpty(cardId)) return false;
        if (HasCard(cardId)) return true;         // já possui
        if (!TrySpendGold(price)) return false;   // não tem ouro

        Data.OwnedCards.Add(cardId);
        Save();
        return true;
    }

    // ======================
    // Deck (limite: _deckLimit)
    // ======================

    public bool EquipToDeck(string cardId)
    {
        if (string.IsNullOrEmpty(cardId)) return false;
        if (!HasCard(cardId)) return false;
        if (Data.EquippedDeck.Contains(cardId)) return true;
        if (Data.EquippedDeck.Count >= _deckLimit) return false;

        Data.EquippedDeck.Add(cardId);
        Save();
        return true;
    }

    public bool UnequipFromDeck(string cardId)
    {
        if (!Data.EquippedDeck.Contains(cardId)) return false;
        Data.EquippedDeck.Remove(cardId);
        Save();
        return true;
    }
}
