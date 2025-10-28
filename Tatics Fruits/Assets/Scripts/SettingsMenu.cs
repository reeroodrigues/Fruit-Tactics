using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum GameLanguage { PT_BR = 0, EN_US = 1, ES_ES = 2 }

public class SettingsMenu : MonoBehaviour
{
    [Header("Panel/Canvas")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image inputBlocker;
    [SerializeField] private float slideDuration = 0.35f;
    [SerializeField] private float overshoot = 0.8f;

    [Header("Dim behavior (opcional)")]
    [SerializeField] private bool blockClicksOutside = false;
    [SerializeField] private bool closeOnDimClick   = false;

    [Header("Toggles")]
    [SerializeField] private Toggle audioToggle;
    [SerializeField] private Image  audioBackground;
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private Image  sfxBackground;

    [Header("Toggle Colors")]
    [SerializeField] private Color onColor  = new Color(0.38f, 0.78f, 0.35f);
    [SerializeField] private Color offColor = new Color(0.90f, 0.30f, 0.25f);

    [Header("Languages (flags as Buttons)")]
    [SerializeField] private Button brButton;
    [SerializeField] private Button usButton;
    [SerializeField] private Button esButton;
    [SerializeField, Range(0f,1f)] private float unselectedAlpha = 0.35f;

    [Header("Other Actions")]
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button deleteAccountButton;
    [SerializeField] private Button termsButton;

    [Header("Optional: disable these while open")]
    [SerializeField] private Button[] buttonsToDisable;

    public bool IsOpen { get; private set; }

    private Vector2 _shownPos;
    private Vector2 _hiddenPos;
    private Sequence _slideTween;
    private bool _animating;

    private GameSettingsModel _settings;

    private void Awake()
    {
        _settings = SettingsRepository.Get();
        
        _shownPos  = panel.anchoredPosition;
        _hiddenPos = _shownPos + new Vector2(0f, Screen.height * 1.1f);
        
        panel.anchoredPosition = _hiddenPos;
        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        if (inputBlocker)
        {
            inputBlocker.enabled = false;
            inputBlocker.raycastTarget = false;
            var dimButton = inputBlocker.GetComponent<Button>();
            if (closeOnDimClick && dimButton != null)
            {
                dimButton.onClick.RemoveAllListeners();
                dimButton.onClick.AddListener(Hide);
                inputBlocker.raycastTarget = true;
            }
        }
        panel.gameObject.SetActive(false);
        IsOpen = false;
        _animating = false;
        
        HookButtonWithFeedback(deleteAccountButton, "Delete Account (futuro)");
        HookButtonWithFeedback(creditsButton, () => Application.OpenURL("https://example.com/credits"));
        HookButtonWithFeedback(termsButton,   () => Application.OpenURL("https://example.com/terms"));
        
        audioToggle.isOn = _settings.musicOn;
        sfxToggle.isOn   = _settings.sfxOn;
        
        ApplyMusicSetting(_settings.musicOn);
        ApplySfxSetting(_settings.sfxOn);
        
        RefreshToggleVisuals();
        
        Localizer.Instance.SetLanguage(_settings.language, save:false);
        RefreshLanguageVisual();
        
        audioToggle.onValueChanged.AddListener(OnAudioToggleChanged);
        sfxToggle.onValueChanged.AddListener(OnSfxToggleChanged);

        brButton.onClick.AddListener(() => SelectLanguage("pt-BR"));
        usButton.onClick.AddListener(() => SelectLanguage("en-US"));
        esButton.onClick.AddListener(() => SelectLanguage("es-ES"));
    }

    private void OnDisable()
    {
        _slideTween?.Kill();
        _slideTween = null;
        _animating = false;

        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        
        if (inputBlocker)
        {
            if (!closeOnDimClick) inputBlocker.raycastTarget = false;
        }

        panel.gameObject.SetActive(false);
        IsOpen = false;
        SetMainButtonsInteractable(true);
    }
    

    private void HookButtonWithFeedback(Button btn, string debugMsg)
    {
        if (!btn) return;
        btn.onClick.AddListener(() =>
        {
            var t = btn.transform;
            DOTween.Sequence()
                .Append(t.DOScale(0.95f, 0.06f))
                .Append(t.DOScale(1f,   0.10f))
                .SetEase(Ease.OutQuad);

            Debug.Log(debugMsg);
        });
    }

    private void HookButtonWithFeedback(Button btn, System.Action onClick)
    {
        if (!btn) return;
        btn.onClick.AddListener(() =>
        {
            var t = btn.transform;
            DOTween.Sequence()
                .Append(t.DOScale(0.95f, 0.06f))
                .Append(t.DOScale(1f,   0.10f))
                .OnComplete(() => onClick?.Invoke());
        });
    }

    public void Toggle()
    {
        if (_animating) return;
        if (IsOpen) Hide();
        else Show();
    }

    public void Show()
    {
        if (_animating) return;
        _animating = true;
        IsOpen = true;
        
        if (inputBlocker)
        {
            inputBlocker.enabled = true;
            inputBlocker.raycastTarget = closeOnDimClick || blockClicksOutside;
        }
        
        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        SetMainButtonsInteractable(false);

        _slideTween?.Kill();
        panel.gameObject.SetActive(true);
        panel.anchoredPosition = _hiddenPos;
        
        _slideTween = DOTween.Sequence()
            .Append(panel.DOAnchorPos(_shownPos, slideDuration).SetEase(Ease.OutBack, overshoot))
            .Join(canvasGroup ? canvasGroup.DOFade(1f, slideDuration * 0.9f) : null)
            .OnComplete(() =>
            {
                if (canvasGroup)
                {
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
                _slideTween = null;
                _animating = false;
            });
    }

    public void Hide()
    {
        if (_animating || !IsOpen) return;
        _animating = true;
        IsOpen = false;

        _slideTween?.Kill();
        if (canvasGroup)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        if (inputBlocker)
        {
            inputBlocker.raycastTarget = false;
        }

        _slideTween = DOTween.Sequence()
            .Append(panel.DOAnchorPos(_hiddenPos, slideDuration * 0.85f).SetEase(Ease.InBack))
            .Join(canvasGroup ? canvasGroup.DOFade(0f, slideDuration * 0.8f) : null)
            .OnComplete(() =>
            {
                panel.gameObject.SetActive(false);
                if (inputBlocker)
                {
                    if (!closeOnDimClick) inputBlocker.raycastTarget = false;
                }
                SetMainButtonsInteractable(true);
                _slideTween = null;
                _animating = false;
            });
    }

    private void SetMainButtonsInteractable(bool value)
    {
        if (buttonsToDisable == null) return;
        foreach (var b in buttonsToDisable)
            if (b) b.interactable = value;
    }

    
    private void OnAudioToggleChanged(bool isOn)
    {
        _settings.musicOn = isOn;
        SettingsRepository.Save(_settings);
        RefreshToggleVisuals();
        ApplyMusicSetting(isOn);
    }
    
    private void OnSfxToggleChanged(bool isOn)
    {
        _settings.sfxOn = isOn;
        SettingsRepository.Save(_settings);
        RefreshToggleVisuals();
        ApplySfxSetting(isOn);
    }

    private void RefreshToggleVisuals()
    {
        if (audioBackground) audioBackground.color = audioToggle.isOn ? onColor : offColor;
        if (sfxBackground)   sfxBackground.color   = sfxToggle.isOn   ? onColor : offColor;
    }

    private void SelectLanguage(string lang)
    {
        _settings.language = lang;
        SettingsRepository.Save(_settings);
        Localizer.Instance.SetLanguage(lang);
        RefreshLanguageVisual();
    }

    private void RefreshLanguageVisual()
    {
        string lang = Localizer.Instance != null
            ? Localizer.Instance.CurrentLanguage
            : SettingsRepository.Get().language;

        SetFlagAlpha(brButton, lang == "pt-BR");
        SetFlagAlpha(usButton, lang == "en-US");
        SetFlagAlpha(esButton, lang == "es-ES");
    }

    private void SetFlagAlpha(Button btn, bool isSelected)
    {
        if (!btn) return;
        var img = btn.image;
        var c = img.color;
        c.a = isSelected ? 1f : unselectedAlpha;
        img.color = c;
        btn.interactable = !isSelected;
    }
    
    
    private void ApplyMusicSetting(bool isOn)
    {
        // AudioController.SetMusicMuted(!isOn);
        
        Debug.Log($"Preferência de Música SALVA e APLICADA: {(isOn ? "ON" : "OFF")}");
    }
    
    private void ApplySfxSetting(bool isOn)
    {
        // AudioController.SetSfxMuted(!isOn);
        
        // Outra opção: AudioListener.volume = isOn ? 1f : 0f;
        
        Debug.Log($"Preferência de SFX SALVA e APLICADA: {(isOn ? "ON" : "OFF")}");
    }
}