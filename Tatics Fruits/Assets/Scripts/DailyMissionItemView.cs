using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class DailyMissionItemView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Button claimButton;
    [SerializeField] private GameObject claimStamp;

    private DailyMissionsController _ctrl;
    private DailyMissionState _state;
    private DailyMissionSo _def; // opcional: definição (SO) desta missão

    // ========= API =========

    // Versão completa: recebe o SO (recomendado)
    public void Setup(DailyMissionsController ctrl, DailyMissionState state, DailyMissionSo def)
    {
        _ctrl = ctrl;
        _state = state;
        _def = def;

        Wire();
    }

    // Versão antiga (continua funcionando): sem SO; o script tenta inferir level pelo id
    public void Setup(DailyMissionsController ctrl, DailyMissionState state)
    {
        _ctrl = ctrl;
        _state = state;
        _def = null;

        Wire();
    }

    private void Wire()
    {
        SetDescriptionLocalized();
        SetRewardLocalized();
        Refresh();

        if (claimButton)
        {
            claimButton.onClick.RemoveAllListeners();
            claimButton.onClick.AddListener(() =>
            {
                if (_ctrl.TryClaimMission(_state.missionId))
                    Refresh();
            });
        }

        if (Localizer.Instance != null)
        {
            Localizer.Instance.OnLanguageChanged -= OnLanguageChanged;
            Localizer.Instance.OnLanguageChanged += OnLanguageChanged;
        }
    }

    private void OnDestroy()
    {
        if (Localizer.Instance != null)
            Localizer.Instance.OnLanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged()
    {
        SetDescriptionLocalized();
        SetRewardLocalized();
    }

    // ========= UI Refresh =========

    public void Refresh()
    {
        int max = Mathf.Max(1, _state.target);
        int cur = Mathf.Clamp(_state.progress, 0, max);

        if (progressBar)
        {
            progressBar.maxValue = max;
            progressBar.value = cur;
        }

        if (progressText)
            progressText.text = $"{cur}/{max}";

        if (claimButton)
            claimButton.interactable = _state.completed && !_state.claimed;

        if (claimStamp)
            claimStamp.SetActive(_state.claimed);
    }

    // ========= Localização da descrição =========

    private void SetDescriptionLocalized()
    {
        if (!descriptionText) return;

        // 1) chave de localização
        // prioridade: descriptionKey do SO (se você quiser adicionar no SO)
        // senão, padrão "mission.<id>"
        string locKey = _def != null && !string.IsNullOrEmpty(_def.descriptionTemplate)
                        ? $"mission.{_def.id}"
                        : $"mission.{_state.missionId}";

        // 2) fallback (se não houver localizer/chave não encontrada)
        // use o template do SO, se houver; caso contrário, o snapshot salvo no dia
        string fallback = (_def != null && !string.IsNullOrEmpty(_def.descriptionTemplate))
                            ? _def.descriptionTemplate
                            : (!string.IsNullOrEmpty(_state.description) ? _state.description : _state.missionId);

        // 3) parâmetros
        // {0} = level (se aplicar), {1} = target
        int level = (_def != null) ? Mathf.Max(0, _def.levelParam) : ExtractTrailingNumber(_state.missionId);
        object[] args = (level > 0) ? new object[] { level, _state.target }
                                    : new object[] { _state.target };

        if (Localizer.Instance != null)
        {
            descriptionText.text = Localizer.Instance.TrFormat(locKey, fallback, args);
        }
        else
        {
            // fallback: tenta substituir {level} e {target} se existirem no texto
            string txt = fallback;
            txt = txt.Replace("{level}", level.ToString());
            txt = txt.Replace("{target}", _state.target.ToString());
            try { txt = string.Format(txt, args); } catch { /* ignora se não tiver placeholders */ }
            descriptionText.text = txt;
        }
    }

    private int ExtractTrailingNumber(string s)
    {
        if (string.IsNullOrEmpty(s)) return 0;
        var m = Regex.Match(s, @"(\d+)$");
        return m.Success ? int.Parse(m.Groups[1].Value) : 0;
    }

    private void SetRewardLocalized()
    {
        if (!rewardText) return;
        string goldWord = (Localizer.Instance != null)
            ? Localizer.Instance.Tr("currency.gold", "Gold")
            : "Gold";
        rewardText.text = $"+{_state.rewardGold} {goldWord}";
    }
}
