using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ToggleSettingsGameplay : MonoBehaviour
{
    [Header("Toggle Menu")] 
    public GameObject settingsMenu;
    public Button settingsButton;
    public Timer timer;

    [Header("Animations")]
    [Range(0.1f, 1f)] public float animationDuration = 0.3f;
    
    [Header("Audio Toggles")]
    public Toggle musicToggle;
    public Toggle sfxToggle;

    [Header("Scene Management")]
    public string mainMenuScene = "MainMenu";

    [SerializeField] private bool isPaused = false;
    
    private CanvasGroup _settingsCanvasGroup;
    private Coroutine _currentToggleCoroutine;
    private GameSettingsModel _settings;
    
    private Vector3 _targetPosition; 

    private void Start()
    {
        _settings = SettingsRepository.Get(); 

        _settingsCanvasGroup = settingsMenu.GetComponent<CanvasGroup>();
        if (_settingsCanvasGroup == null)
        {
            _settingsCanvasGroup = settingsMenu.AddComponent<CanvasGroup>();
        }
        
        _targetPosition = settingsMenu.transform.localPosition;
        
        settingsMenu.SetActive(false);
        _settingsCanvasGroup.alpha = 0f;
        _settingsCanvasGroup.interactable = false;
        _settingsCanvasGroup.blocksRaycasts = false;

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(ToggleSettingsMenu);
        }

        if (musicToggle != null)
        {
            musicToggle.isOn = _settings.musicOn;
            SetMusicMute(_settings.musicOn); 
            musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
        }

        if (sfxToggle != null)
        {
            sfxToggle.isOn = _settings.sfxOn;
            SetSfxMute(_settings.sfxOn); 
            sfxToggle.onValueChanged.AddListener(OnSfxToggleChanged);
        }

        if (timer == null)
        {
            timer = FindAnyObjectByType<Timer>();
        }
    }

    private void OnMusicToggleChanged(bool isOn)
    {
        _settings.musicOn = isOn;
        SettingsRepository.Save(_settings);
        SetMusicMute(isOn);
    }

    private void OnSfxToggleChanged(bool isOn)
    {
        _settings.sfxOn = isOn;
        SettingsRepository.Save(_settings);
        SetSfxMute(isOn);
    }

    public void ToggleSettingsMenu()
    {
        if (_currentToggleCoroutine != null)
        {
            StopCoroutine(_currentToggleCoroutine);
        }

        isPaused = !isPaused;

        if (timer != null)
        {
            timer.IsPaused = isPaused; 
        }
        
        Time.timeScale = isPaused ? 0f : 1f;

        _currentToggleCoroutine = StartCoroutine(AnimatePanelToggle(isPaused));
    }

    private IEnumerator AnimatePanelToggle(bool isOpening)
    {
        Vector3 startPositionOffset = _targetPosition + new Vector3(0, 100f, 0); 
        
        if (isOpening)
        {
            settingsMenu.SetActive(true);
            _settingsCanvasGroup.blocksRaycasts = true;
            
            var startTime = Time.unscaledTime;
            
            settingsMenu.transform.localPosition = startPositionOffset;
            _settingsCanvasGroup.alpha = 0f;

            while (Time.unscaledTime < startTime + animationDuration)
            {
                var t = (Time.unscaledTime - startTime) / animationDuration;
                
                settingsMenu.transform.localPosition = Vector3.Lerp(startPositionOffset, _targetPosition, t);
                
                _settingsCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
                
                yield return null;
            }
            
            settingsMenu.transform.localPosition = _targetPosition;
            _settingsCanvasGroup.alpha = 1f;
            _settingsCanvasGroup.interactable = true;
        }
        else
        {
            _settingsCanvasGroup.interactable = false;
            
            var startTime = Time.unscaledTime;
            var currentPosition = settingsMenu.transform.localPosition;
            
            while (Time.unscaledTime < startTime + animationDuration)
            {
                var t = (Time.unscaledTime - startTime) / animationDuration;
                
                settingsMenu.transform.localPosition = Vector3.Lerp(currentPosition, startPositionOffset, t);
                
                _settingsCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }
            
            settingsMenu.transform.localPosition = startPositionOffset; 
            _settingsCanvasGroup.alpha = 0f;
            _settingsCanvasGroup.blocksRaycasts = false;
            settingsMenu.SetActive(false);
        }
    }
    
    private void SetMusicMute(bool isOn)
    {
        // AudioManager.Instance.SetMusicVolume(isOn ? 1f : 0f);
        Debug.Log("Musica: " + (isOn ? "ON" : "OFF"));
    }
    
    private void SetSfxMute(bool isOn)
    {
        // AudioManager.Instance.SetSfxVolume(isOn ? 1f : 0f);
        Debug.Log("SFX: " + (isOn ? "ON" : "OFF"));
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        
        SceneManager.LoadScene(mainMenuScene);
    }
}