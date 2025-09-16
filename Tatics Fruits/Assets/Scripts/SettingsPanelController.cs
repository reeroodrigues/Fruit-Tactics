using System.Collections;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Audio;
using TMPro;

public class SettingsPanelController : MonoBehaviour
{
    [Header("Root / Navegação")]
    [SerializeField] private RectTransform settingsPanel;          // Arraste o RectTransform do SettingsPanel
    [SerializeField] private Button backButton;                    // X (Back_Button)
    [SerializeField] private UnityEvent onOpenCredits;             // Configure no Inspector (abrir tela de créditos)
    [SerializeField] private SettingsMenu slideController;         // (opcional) o script que faz o painel descer/subir

    [Header("Áudio")]
    [SerializeField] private Slider musicSlider;                   // SettingsPanel/ Music_Group / Slider
    [SerializeField] private Slider sfxSlider;                     // SettingsPanel/ SFX_Group / Slider
    [SerializeField] private AudioMixer audioMixer;                // (opcional) arraste seu mixer
    [SerializeField] private string musicMixerParam = "MusicVol";  // nome do exposed param (dB)
    [SerializeField] private string sfxMixerParam = "SfxVol";      // nome do exposed param (dB)

    [Header("Idioma (bandeiras)")]
    [Tooltip("Ordem recomendada: pt-BR, en, es, fr")]
    [SerializeField] private Button[] flagButtons;                 // Language_Group/Content_Flags/Flag_Button (1..4)
    [SerializeField] private Image[] flagImages;                   // (opcional) as Images dos botões p/ destacar seleção
    [SerializeField] private string[] languageCodes = { "pt-BR", "en", "es", "fr" };

    [Header("Vibração")]
    [SerializeField] private Button vibrationButton;               // Vibration_Group/Vibration_Button
    [SerializeField] private TextMeshProUGUI vibrationText;        // Vibration_Group/Text (TMP)

    [Header("Erase / Créditos")]
    [SerializeField] private Button eraseButton;                   // SaveData_Group/Save_Button (rótulo "Erase")
    [SerializeField] private TextMeshProUGUI eraseButtonText;      // SaveData_Group/Text (TMP) do botão "Erase"
    [SerializeField] private Button creditsButton;                 // Credits_Group/Credits_Button

    // ---- Estado / Constantes ----
    private const string KEY_MUSIC = "settings.music";
    private const string KEY_SFX = "settings.sfx";
    private const string KEY_LANG = "settings.lang";
    private const string KEY_VIBR = "settings.vibration";

    private const float DEFAULT_MUSIC = 1f;
    private const float DEFAULT_SFX = 1f;
    private const string DEFAULT_LANG = "pt-BR";

    private enum VibrationMode { Off = 0, Normal = 1, Strong = 2 }
    private VibrationMode _vibration = VibrationMode.Normal;

    private bool _awaitingEraseConfirm;
    private Coroutine _eraseRoutine;

    // ------------- Ciclo de Vida -------------
    private void Awake()
    {
        // Listeners básicos
        if (backButton) backButton.onClick.AddListener(ClosePanel);
        if (creditsButton) creditsButton.onClick.AddListener(() => onOpenCredits?.Invoke());

        if (musicSlider)
        {
            musicSlider.minValue = 0f;
            musicSlider.maxValue = 1f;
            musicSlider.wholeNumbers = false;
            musicSlider.onValueChanged.AddListener(OnMusicChanged);
        }
        if (sfxSlider)
        {
            sfxSlider.minValue = 0f;
            sfxSlider.maxValue = 1f;
            sfxSlider.wholeNumbers = false;
            sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        }

        if (vibrationButton) vibrationButton.onClick.AddListener(CycleVibration);

        if (eraseButton) eraseButton.onClick.AddListener(OnEraseClick);

        // Bandeiras
        for (int i = 0; i < flagButtons.Length; i++)
        {
            int idx = i; // captura
            flagButtons[i].onClick.AddListener(() => OnFlagClicked(idx));
        }

        LoadSettingsToUI();
    }

    // ------------- Carregar / Salvar -------------
    private void LoadSettingsToUI()
    {
        // Música
        float music = PlayerPrefs.GetFloat(KEY_MUSIC, DEFAULT_MUSIC);
        if (musicSlider) musicSlider.SetValueWithoutNotify(music);
        ApplyMusic(music);

        // SFX
        float sfx = PlayerPrefs.GetFloat(KEY_SFX, DEFAULT_SFX);
        if (sfxSlider) sfxSlider.SetValueWithoutNotify(sfx);
        ApplySfx(sfx);

        // Idioma
        string lang = PlayerPrefs.GetString(KEY_LANG, DEFAULT_LANG);
        int langIndex = IndexOfLang(lang);
        if (langIndex < 0) langIndex = 0;
        HighlightSelectedFlag(langIndex);
        ApplyLanguage(lang);

        // Vibração
        _vibration = (VibrationMode)PlayerPrefs.GetInt(KEY_VIBR, (int)VibrationMode.Normal);
        UpdateVibrationUI();
    }

    private void SaveAudioPrefs(float music, float sfx)
    {
        PlayerPrefs.SetFloat(KEY_MUSIC, music);
        PlayerPrefs.SetFloat(KEY_SFX, sfx);
        PlayerPrefs.Save();
    }

    private void SaveLanguagePref(string code)
    {
        PlayerPrefs.SetString(KEY_LANG, code);
        PlayerPrefs.Save();
    }

    private void SaveVibrationPref(VibrationMode mode)
    {
        PlayerPrefs.SetInt(KEY_VIBR, (int)mode);
        PlayerPrefs.Save();
    }

    // ------------- Handlers: Áudio -------------
    private void OnMusicChanged(float v)
    {
        ApplyMusic(v);
        SaveAudioPrefs(v, PlayerPrefs.GetFloat(KEY_SFX, DEFAULT_SFX));
    }

    private void OnSfxChanged(float v)
    {
        ApplySfx(v);
        SaveAudioPrefs(PlayerPrefs.GetFloat(KEY_MUSIC, DEFAULT_MUSIC), v);
    }

    private void ApplyMusic(float v)
    {
        // Se tiver AudioMixer, converte 0..1 -> dB; senão, ajusta AudioListener (global)
        if (audioMixer)
        {
            float db = LinearToDecibels(v);
            audioMixer.SetFloat(musicMixerParam, db);
        }
        else
        {
            // fallback simples (global) – opcional: troque por seu próprio gerenciador de música
            AudioListener.volume = Mathf.Clamp01(v);
        }
    }

    private void ApplySfx(float v)
    {
        if (audioMixer)
        {
            float db = LinearToDecibels(v);
            audioMixer.SetFloat(sfxMixerParam, db);
        }
        // Caso não esteja usando Mixer, salve o valor; seus AudioSources podem ler PlayerPrefs KEY_SFX
    }

    private static float LinearToDecibels(float linear)
    {
        // evita -Inf quando v=0
        if (linear <= 0.0001f) return -80f; // valor comum para "mute"
        return Mathf.Log10(linear) * 20f;
    }

    // ------------- Handlers: Idioma -------------
    private void OnFlagClicked(int index)
    {
        if (index < 0 || index >= languageCodes.Length) return;
        string code = languageCodes[index];
        HighlightSelectedFlag(index);
        ApplyLanguage(code);
        SaveLanguagePref(code);
    }

    private void HighlightSelectedFlag(int selected)
    {
        if (flagImages == null || flagImages.Length == 0) return;

        for (int i = 0; i < flagImages.Length; i++)
        {
            if (!flagImages[i]) continue;
            var c = flagImages[i].color;
            c.a = (i == selected) ? 1f : 0.45f;
            flagImages[i].color = c;
            // dica: se quiser, aqui dá pra fazer uma leve escala/outline no selecionado
        }
    }

    private int IndexOfLang(string code)
    {
        for (int i = 0; i < languageCodes.Length; i++)
            if (languageCodes[i] == code) return i;
        return -1;
    }

    private void ApplyLanguage(string code)
    {
        // Integra com seu Localizer
        // Ex.: Localizer.Instance.SetLanguage(code);
        //      Localizer.Instance.RefreshAll();
        Debug.Log($"[Settings] Language set: {code}");
        try
        {
            var loc = FindObjectOfType<Localizer>(); // se tiver singleton, troque por Localizer.Instance
            if (loc != null)
            {
                // Supondo que haja um método com essa assinatura:
                // loc.SetLanguage(code);
                // loc.RefreshAll();
                var setLang = loc.GetType().GetMethod("SetLanguage");
                var refresh = loc.GetType().GetMethod("RefreshAll");
                setLang?.Invoke(loc, new object[] { code });
                refresh?.Invoke(loc, null);
            }
        }
        catch { /* seguro se não existir Localizer ainda */ }
    }

    // ------------- Handlers: Vibração -------------
    private void CycleVibration()
    {
        _vibration = (VibrationMode)(((int)_vibration + 1) % 3);
        UpdateVibrationUI();
        SaveVibrationPref(_vibration);

        // Feedback sutil (opcional)
#if UNITY_ANDROID || UNITY_IOS
        if (_vibration != VibrationMode.Off)
        {
            // NÃO vibre forte aqui: só um toque curto (quando disponível)
            Handheld.Vibrate();
        }
#endif
    }

    private void UpdateVibrationUI()
    {
        if (!vibrationText) return;
        switch (_vibration)
        {
            case VibrationMode.Off:    vibrationText.text = "Off";    break;
            case VibrationMode.Normal: vibrationText.text = "Normal"; break;
            case VibrationMode.Strong: vibrationText.text = "Forte";  break;
        }
    }

    // ------------- Handlers: Erase -------------
    private void OnEraseClick()
    {
        if (!_awaitingEraseConfirm)
        {
            // 1º clique: pede confirmação
            _awaitingEraseConfirm = true;
            if (eraseButtonText) eraseButtonText.text = "Confirmar?";
            if (_eraseRoutine != null) StopCoroutine(_eraseRoutine);
            _eraseRoutine = StartCoroutine(EraseConfirmWindow(2.0f));
        }
        else
        {
            // 2º clique (dentro da janela): apagar
            if (_eraseRoutine != null) StopCoroutine(_eraseRoutine);
            _eraseRoutine = null;
            _awaitingEraseConfirm = false;
            if (eraseButtonText) eraseButtonText.text = "Erase";

            // Apaga PlayerPrefs (ajuste aqui se usar JsonDataService)
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            // Recarrega valores padrão na UI
            LoadSettingsToUI();
            Debug.Log("[Settings] Dados apagados.");
        }
    }

    private IEnumerator EraseConfirmWindow(float seconds)
    {
        float t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        _awaitingEraseConfirm = false;
        if (eraseButtonText) eraseButtonText.text = "Erase";
        _eraseRoutine = null;
    }

    // ------------- Fechar Painel -------------
    private void ClosePanel()
    {
        if (slideController != null) { slideController.Close(); return; }

        // fallback: só desativa
        if (settingsPanel) settingsPanel.gameObject.SetActive(false);
        else gameObject.SetActive(false);
    }
}
