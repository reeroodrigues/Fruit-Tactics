using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Toggle _vibrationToggle;
    [SerializeField] private Button _exitButton;
    

    private void Start()
    {
        _musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        _sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        _vibrationToggle.isOn = PlayerPrefs.GetInt("Vibration", 1) == 1;
        
        _musicSlider.onValueChanged.AddListener(SetMusicVolume);
        _sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        _vibrationToggle.onValueChanged.AddListener(SetVibration);
        _exitButton.onClick.AddListener(CloseSettings);
    }

    public void OpenSettings()
    {
        _settingsPanel.SetActive(true);
        _canvasGroup.alpha = 0;
        _canvasGroup.DOFade(1, 0.3f).OnComplete(() => _canvasGroup.interactable = true);
    }

    public void CloseSettings()
    {
        if (_settingsPanel.activeSelf)
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.DOFade(0, 0.3f).OnComplete(() =>
            {
                _settingsPanel.SetActive(false);
            });
        }
    }

    private void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("Music Volume", volume);
        PlayerPrefs.Save();
        Debug.Log("Volume ajustado para: " + volume);
    }

    private void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFX Volume", volume);
        PlayerPrefs.Save();
        Debug.Log("Volume dos efeitos ajustado para: " + volume);
    }

    private void SetVibration(bool isVibrationOn)
    {
        PlayerPrefs.SetInt("Vibration", isVibrationOn ? 1: 0);
        PlayerPrefs.Save();
        Debug.Log("Vibração ativada: " + isVibrationOn);

        if (isVibrationOn)
        {
            Handheld.Vibrate();
        }
    }
}
