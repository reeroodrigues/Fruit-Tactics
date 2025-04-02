using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileController : MonoBehaviour
{
    [SerializeField] private GameObject _profilePanel;
    [SerializeField] private TextMeshProUGUI _playerNameText;
    [SerializeField] private Image _avatarImage;
    [SerializeField] private Sprite[] _avatars;
    [SerializeField] private TMP_InputField _playerNameInput;
    [SerializeField] private TextMeshProUGUI _playerNameErrorText;
    [SerializeField] private GameObject _avatarSelectionPanel;
    [SerializeField] private Button _closeAvatarPanelButton;

    private int _currentAvatarIndex = 0;

    private void Start()
    {
        var savedName = PlayerPrefs.GetString("PlayerName", "Jogador");
        _playerNameText.text = savedName;
        _playerNameInput.text = savedName;
        
        _currentAvatarIndex = PlayerPrefs.GetInt("AvatarIndex", 0);
        _avatarImage.sprite = _avatars[_currentAvatarIndex];
        
        _playerNameInput.onEndEdit.AddListener(ValidatePlayerName);
        
        _closeAvatarPanelButton.onClick.AddListener(CloseAvatarSelection);
    }

    public void OpenProfile()
    {
        _profilePanel.SetActive(true);
    }

    public void CloseProfile()
    {
        _profilePanel.SetActive(false);
    }

    public void ChangeAvatar()
    {
        _currentAvatarIndex = (_currentAvatarIndex + 1) % _avatars.Length;
        _avatarImage.sprite = _avatars[_currentAvatarIndex];
        PlayerPrefs.SetInt("AvatarIndex", _currentAvatarIndex);
        PlayerPrefs.Save();
    }

    public void OpenAvatarSelection()
    {
        _avatarSelectionPanel.SetActive(true);
    }

    public void CloseAvatarSelection()
    {
        _avatarSelectionPanel.SetActive(false);
    }

    public void SelectAvatar(int avatarIndex)
    {
        _currentAvatarIndex = avatarIndex;
        _avatarImage.sprite = _avatars[_currentAvatarIndex];
        PlayerPrefs.SetInt("AvatarIndex", _currentAvatarIndex);
        PlayerPrefs.Save();
        CloseAvatarSelection();
    }

    private void ValidatePlayerName(string name)
    {
        if (!Regex.IsMatch(name, @"^[A-Za-zÀ-ÖØ-öø-ÿ\s]+$"))
        {
            _playerNameErrorText.text = "Nome inválido! Apenas letras são permitidas.";
            _playerNameInput.text = PlayerPrefs.GetString("PlayerName", "Jogador");
        }
        else
        {
            _playerNameErrorText.text = "";
            _playerNameText.text = name;
            PlayerPrefs.SetString("PlayerName", name);
            PlayerPrefs.Save();
        }
    }
}
