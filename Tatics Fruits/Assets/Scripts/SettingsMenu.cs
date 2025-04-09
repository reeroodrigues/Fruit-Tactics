using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingsMenu : MonoBehaviour
{
    [Header("Música")]
    [SerializeField] private Slider musicToggle;
    [SerializeField] private Image musicHandle;
    [SerializeField] private Sprite musicOnSprite;
    [SerializeField] private Sprite musicOffSprite;

    [Header("Efeitos Sonoros")]
    [SerializeField] private Slider sfxToggle;
    [SerializeField] private Image sfxHandle;
    [SerializeField] private Sprite sfxOnSprite;
    [SerializeField] private Sprite sfxOffSprite;

    [Header("Vibração")]
    [SerializeField] private Slider vibrationToggle;
    [SerializeField] private Image vibrationHandle;
    [SerializeField] private Sprite vibrationOnSprite;
    [SerializeField] private Sprite vibrationOffSprite;

    private void Start()
    {
        musicToggle.value = PlayerPrefs.GetInt("MusicEnabled", 1);
        sfxToggle.value = PlayerPrefs.GetInt("SFXEnabled", 1);
        vibrationToggle.value = PlayerPrefs.GetInt("VibrationEnabled", 1);

        UpdateAllSprites();

        musicToggle.onValueChanged.AddListener((v) => AnimateToggle(musicToggle, musicHandle, musicOnSprite, musicOffSprite));
        sfxToggle.onValueChanged.AddListener((v) => AnimateToggle(sfxToggle, sfxHandle, sfxOnSprite, sfxOffSprite));
        vibrationToggle.onValueChanged.AddListener((v) => AnimateToggle(vibrationToggle, vibrationHandle, vibrationOnSprite, vibrationOffSprite));
    }

    private void UpdateAllSprites()
    {
        AnimateToggle(musicToggle, musicHandle, musicOnSprite, musicOffSprite, false);
        AnimateToggle(sfxToggle, sfxHandle, sfxOnSprite, sfxOffSprite, false);
        AnimateToggle(vibrationToggle, vibrationHandle, vibrationOnSprite, vibrationOffSprite, false);
    }

    private void AnimateToggle(Slider slider, Image handle, Sprite onSprite, Sprite offSprite, bool animate = true)
    {
        float targetValue = slider.value;

        if (animate)
        {
            slider.DOValue(targetValue, 0.2f).SetEase(Ease.InOutSine);
        }
        else
        {
            slider.value = targetValue;
        }

        handle.sprite = targetValue == 1 ? onSprite : offSprite;
    }

    public void SaveSettingsAndClose()
    {
        PlayerPrefs.SetInt("MusicEnabled", (int)musicToggle.value);
        PlayerPrefs.SetInt("SFXEnabled", (int)sfxToggle.value);
        PlayerPrefs.SetInt("VibrationEnabled", (int)vibrationToggle.value);
        PlayerPrefs.Save();

        gameObject.SetActive(false);
    }
}
