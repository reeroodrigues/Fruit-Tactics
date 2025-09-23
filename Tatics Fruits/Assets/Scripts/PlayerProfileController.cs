using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerProfileController : MonoBehaviour
{
    [Header("UI / Perfil")]
    [SerializeField] private GameObject profilePanel;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Image avatarImage;
    [SerializeField] private Sprite[] avatars;
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private TextMeshProUGUI playerNameErrorText;
    [SerializeField] private GameObject avatarSelectionPanel;
    [SerializeField] private Button closeAvatarPanelButton;

    [Header("UI / Economia")]
    [SerializeField] private TextMeshProUGUI _goldText;

    [Header("Deck")]
    [SerializeField] private int _deckLimit = 5;

    private const string FileName = "player_profile.json";
    private const string NameRegex = @"^[A-Za-zÀ-ÖØ-öø-ÿ\s]+$";
    public bool IsLoaded { get; private set; }
    public event Action OnProfileLoaded;
    
    public PlayerProfileData Data { get; private set; } = new PlayerProfileData();
    
    public event Action<int> OnGoldChanged;
    
    private void Awake()
    {
        if (!JsonDataService.TryLoad<PlayerProfileData>(FileName, out var loaded))
        {
            Data = new PlayerProfileData();
            MigrateFromPlayerPrefsIfNeeded();
            Save();
        }
        else
        {
            Data = loaded ?? new PlayerProfileData();
            if (Data.OwnedCards == null)    Data.OwnedCards = new System.Collections.Generic.List<string>();
            if (Data.EquippedDeck == null)  Data.EquippedDeck = new System.Collections.Generic.List<string>();
            if (Data.Daily == null)         Data.Daily = new DailySystemData();
            if (Data.Daily.Login == null)   Data.Daily.Login = new DailyLoginData();
        }

        IsLoaded = true;
        OnProfileLoaded?.Invoke();
    }

    private void Start()
    {
        if (!JsonDataService.TryLoad<PlayerProfileData>(FileName, out var loaded))
        {
            Data = new PlayerProfileData();
            MigrateFromPlayerPrefsIfNeeded();
            Save();
        }
        else
        {
            Data = loaded ?? new PlayerProfileData();
            if (Data.OwnedCards == null) Data.OwnedCards = new System.Collections.Generic.List<string>();
            
            if (Data.EquippedDeck == null) Data.EquippedDeck = new System.Collections.Generic.List<string>();
        }
        
        ApplyProfileUI();
        UpdateGoldUI();
        
        if (playerNameInput != null)
            playerNameInput.onEndEdit.AddListener(ValidatePlayerName);
        
        if (closeAvatarPanelButton != null)
            closeAvatarPanelButton.onClick.AddListener(CloseAvatarSelection);
    }

    private void MigrateFromPlayerPrefsIfNeeded()
    {
        if (Data.PlayerName == "Jogador")
        {
            var legacyName = PlayerPrefs.GetString("PlayerName", "Jogador");
            if (!string.IsNullOrWhiteSpace(legacyName))
                Data.PlayerName = legacyName;
        }

        if (Data.AvatarIndex == 0)
        {
            var legacyAvatar = PlayerPrefs.GetInt("AvatarIndex", 0);
            Data.AvatarIndex = Mathf.Clamp(legacyAvatar, 0, avatars != null && avatars.Length > 0 ? avatars.Length - 1 : 0);
        }
    }

    private void ApplyProfileUI()
    {
        if (playerNameText)
            playerNameText.text = Data.PlayerName;
        
        if (playerNameInput) 
            playerNameInput.text = Data.PlayerName;

        if (avatars != null && avatars.Length > 0 && avatarImage)
        {
            var idx = Mathf.Clamp(Data.AvatarIndex, 0, avatars.Length - 1);
            avatarImage.sprite = avatars[idx];
        }
    }

    private void UpdateGoldUI()
    {
        if (_goldText) _goldText.text = Data.Gold.ToString();
        OnGoldChanged?.Invoke(Data.Gold);
    }

    private void Save() => JsonDataService.Save(FileName, Data);
    

    public void OpenProfile()  => profilePanel?.SetActive(true);
    public void CloseProfile() => profilePanel?.SetActive(false);

    public void OpenAvatarSelection()  => avatarSelectionPanel?.SetActive(true);
    public void CloseAvatarSelection() => avatarSelectionPanel?.SetActive(false);

    public void ChangeAvatar()
    {
        if (avatars == null || avatars.Length == 0) 
            return;
        
        Data.AvatarIndex = (Data.AvatarIndex + 1) % avatars.Length;
        avatarImage.sprite = avatars[Data.AvatarIndex];
        Save();
    }

    public void SelectAvatar(int avatarIndex)
    {
        if (avatars == null || avatars.Length == 0) 
            return;
        
        Data.AvatarIndex = Mathf.Clamp(avatarIndex, 0, avatars.Length - 1);
        avatarImage.sprite = avatars[Data.AvatarIndex];
        Save();
        CloseAvatarSelection();
    }

    private void ValidatePlayerName(string name)
    {
        if (!Regex.IsMatch(name, NameRegex))
        {
            if (playerNameErrorText) playerNameErrorText.text = "Nome inválido! Apenas letras são permitidas.";
            if (playerNameInput)     playerNameInput.text = Data.PlayerName;
        }
        else
        {
            if (playerNameErrorText) playerNameErrorText.text = "";
            if (playerNameText)      playerNameText.text = name;
            Data.PlayerName = name;
            Save();
        }
    }

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
        if (HasCard(cardId)) return true;
        if (!TrySpendGold(price)) return false;

        Data.OwnedCards.Add(cardId);
        Save();
        return true;
    }

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

    public void SaveProfile()
    {
        JsonDataService.Save("player_profile.json", Data);
    }

    public void AddGoldAndSave(int amount)
    {
        if (amount == 0)
            return;
        
        Data.Gold = Mathf.Max(0, Data.Gold + amount);
        SaveProfile();
        UpdateGoldUI();
    }
}
