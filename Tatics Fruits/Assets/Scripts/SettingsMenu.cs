using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingsMenu : MonoBehaviour
{
    [Header("Música")]
    [SerializeField] private Button musicButton;
    [SerializeField] private Image musicIcon;
    [SerializeField] private Sprite musicOnSprite;
    [SerializeField] private Sprite musicOffSprite;

    [Header("Outros Botões")]
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button leaderboardButton;

    // [Header("Painéis")]
    // [SerializeField] private GameObject creditsPanel;
    // [SerializeField] private GameObject leaderboardPanel;

    private bool isMusicOn;

    private void Start()
    {
        isMusicOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        UpdateMusicIcon();

        musicButton.onClick.AddListener(ToggleMusic);
        // creditsButton.onClick.AddListener(OpenCredits);
        // leaderboardButton.onClick.AddListener(OpenLeaderboard);
    }

    private void ToggleMusic()
    {
        isMusicOn = !isMusicOn;
        PlayerPrefs.SetInt("MusicEnabled", isMusicOn ? 1 : 0);
        UpdateMusicIcon();
        
    }

    private void UpdateMusicIcon()
    {
        musicIcon.sprite = isMusicOn ? musicOnSprite : musicOffSprite;
    }

    // private void OpenCredits()
    // {
    //     creditsPanel.SetActive(true);
    // }
    
    // private void OpenLeaderboard()
    // {
    //     leaderboardPanel.SetActive(true);
    // }

    public void ClosePopup()
    {
        PlayerPrefs.Save();
        transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}