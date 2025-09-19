using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum GameLanguage { PT_BR = 0, EN_US = 1, ES_ES = 2 }

public class SettingsMenu : MonoBehaviour
{
    [Header("Panel/Canvas")]
    [SerializeField] private RectTransform panel;      // container que desliza
    [SerializeField] private CanvasGroup canvasGroup;  // para fade/blocksRaycasts
    [SerializeField] private Image inputBlocker;       // Image full-screen (cor transparente), Raycast Target ON
    [SerializeField] private float slideDuration = 0.35f;
    [SerializeField] private float overshoot = 0.8f;   // Ease.OutBack

    [Header("Toggles")]
    [SerializeField] private Toggle audioToggle;       // Master/Music
    [SerializeField] private Image  audioBackground;   // Image do fundo do toggle (vai trocar a cor)
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private Image  sfxBackground;

    [Header("Toggle Colors")]
    [SerializeField] private Color onColor  = new Color(0.38f, 0.78f, 0.35f); // verde folha
    [SerializeField] private Color offColor = new Color(0.90f, 0.30f, 0.25f); // vermelho

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
    private Tweener _slideTween;

    // PlayerPrefs keys
    private const string K_MUSIC = "MusicEnabled";
    private const string K_SFX   = "SfxEnabled";
    private const string K_LANG  = "Lang";

    private void Awake()
    {
        // posições
        _shownPos = panel.anchoredPosition;                     // posição visível (defina no editor)
        _hiddenPos = _shownPos + new Vector2(0f, Screen.height * 1.1f); // off-screen acima

        // estado inicial fechado
        panel.anchoredPosition = _hiddenPos;
        if (canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        if (inputBlocker) inputBlocker.enabled = false;
        IsOpen = false;

        // defaults de prefs (primeira execução)
        if (!PlayerPrefs.HasKey(K_MUSIC)) PlayerPrefs.SetInt(K_MUSIC, 1);
        if (!PlayerPrefs.HasKey(K_SFX))   PlayerPrefs.SetInt(K_SFX,   1);
        if (!PlayerPrefs.HasKey(K_LANG))  PlayerPrefs.SetInt(K_LANG,  (int)GameLanguage.PT_BR);

        // liga listeners
        audioToggle.onValueChanged.AddListener(OnAudioToggleChanged);
        sfxToggle.onValueChanged.AddListener(OnSfxToggleChanged);

        brButton.onClick.AddListener(() => SetLanguage(GameLanguage.PT_BR));
        usButton.onClick.AddListener(() => SetLanguage(GameLanguage.EN_US));
        esButton.onClick.AddListener(() => SetLanguage(GameLanguage.ES_ES));

        HookButtonWithFeedback(creditsButton, "Abrir Créditos");
        HookButtonWithFeedback(deleteAccountButton, "Delete Account (futuro)");
        HookButtonWithFeedback(termsButton, "Abrir Termos");

        // aplica estado salvo
        audioToggle.isOn = PlayerPrefs.GetInt(K_MUSIC, 1) == 1;
        sfxToggle.isOn   = PlayerPrefs.GetInt(K_SFX,   1) == 1;
        RefreshToggleVisuals();

        RefreshLanguageVisual(); // respeita o idioma salvo
    }

    private void HookButtonWithFeedback(Button btn, string debugMsg)
    {
        if (!btn) return;
        btn.onClick.AddListener(() =>
        {
            // feedback visual rápido
            var t = btn.transform;
            DOTween.Sequence()
                .Append(t.DOScale(0.95f, 0.06f))
                .Append(t.DOScale(1f,   0.10f))
                .SetEase(Ease.OutQuad);

            Debug.Log(debugMsg);
        });
    }

    // ---------- Open / Close ----------
    public void Toggle()
    {
        if (IsOpen) Hide();
        else Show();
    }

    public void Show()
    {
        IsOpen = true;

        if (inputBlocker)
        {
            inputBlocker.enabled = true;                 // ativa o bloqueio de clique por baixo
            inputBlocker.raycastTarget = true;
        }
        if (canvasGroup)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;           // garante que o próprio painel capture os cliques
        }
        SetMainButtonsInteractable(false);

        _slideTween?.Kill();
        panel.gameObject.SetActive(true);
        panel.anchoredPosition = _hiddenPos;
        if (canvasGroup) canvasGroup.alpha = 0f;

        _slideTween = panel.DOAnchorPos(_shownPos, slideDuration)
            .SetEase(Ease.OutBack, overshoot);

        if (canvasGroup) canvasGroup.DOFade(1f, slideDuration * 0.9f);
    }

    public void Hide()
    {
        IsOpen = false;

        _slideTween?.Kill();
        _slideTween = panel.DOAnchorPos(_hiddenPos, slideDuration * 0.85f)
            .SetEase(Ease.InBack);

        if (canvasGroup) canvasGroup.DOFade(0f, slideDuration * 0.8f);

        _slideTween.OnComplete(() =>
        {
            panel.gameObject.SetActive(false);
            if (canvasGroup)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            if (inputBlocker)
            {
                inputBlocker.enabled = false;
                inputBlocker.raycastTarget = false;
            }
            SetMainButtonsInteractable(true);
        });
    }

    private void SetMainButtonsInteractable(bool value)
    {
        if (buttonsToDisable == null) return;
        foreach (var b in buttonsToDisable)
            if (b) b.interactable = value;
    }

    // ---------- Toggles ----------
    private void OnAudioToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt(K_MUSIC, isOn ? 1 : 0);
        PlayerPrefs.Save();
        // TODO: AudioManager.Instance?.SetMusicEnabled(isOn);
        RefreshToggleVisuals();
    }

    private void OnSfxToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt(K_SFX, isOn ? 1 : 0);
        PlayerPrefs.Save();
        // TODO: AudioManager.Instance?.SetSfxEnabled(isOn);
        RefreshToggleVisuals();
    }

    private void RefreshToggleVisuals()
    {
        if (audioBackground) audioBackground.color = audioToggle.isOn ? onColor : offColor;
        if (sfxBackground)   sfxBackground.color   = sfxToggle.isOn   ? onColor : offColor;
    }

    // ---------- Idiomas ----------
    private void SetLanguage(GameLanguage lang)
    {
        PlayerPrefs.SetInt(K_LANG, (int)lang);
        PlayerPrefs.Save();
        // TODO: Localizer.Instance?.SetLanguage(lang);
        RefreshLanguageVisual();
    }

    private void RefreshLanguageVisual()
    {
        var selected = (GameLanguage)PlayerPrefs.GetInt(K_LANG, (int)GameLanguage.PT_BR);

        SetFlagAlpha(brButton, selected == GameLanguage.PT_BR);
        SetFlagAlpha(usButton, selected == GameLanguage.EN_US);
        SetFlagAlpha(esButton, selected == GameLanguage.ES_ES);
    }

    private void SetFlagAlpha(Button btn, bool isSelected)
    {
        if (!btn) return;
        var img = btn.image;
        var c = img.color;
        c.a = isSelected ? 1f : unselectedAlpha;
        img.color = c;
        btn.interactable = !isSelected; // evita clique no já-selecionado
    }
}
