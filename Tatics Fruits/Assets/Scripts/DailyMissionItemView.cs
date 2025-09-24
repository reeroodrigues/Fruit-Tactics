using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class DailyMissionItemView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Button claimButton;
    [SerializeField] private GameObject claimStamp;

    private DailyMissionsController _ctrl;
    private DailyMissionState _state;

    public void Setup(DailyMissionsController ctrl, DailyMissionState state)
    {
        _ctrl = ctrl;
        _state = state;

        SetDescriptionLocalized();
        SetRewardLocalized();

        Refresh();

        claimButton.onClick.RemoveAllListeners();
        claimButton.onClick.AddListener(() =>
        {
            if (_ctrl.TryClaimMission(state.missionId))
                Refresh();
        });

        if (Localizer.Instance != null)
            Localizer.Instance.OnLanguageChanged += OnLanguageChanged;
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

    public void Refresh()
    {
        progressBar.maxValue = Mathf.Max(1, _state.target);
        progressBar.value = Mathf.Min(_state.target, _state.progress);
        progressText.text = $"{_state.progress}/{_state.target}";

        claimButton.interactable = _state.completed && !_state.claimed;
        if (claimStamp) claimStamp.SetActive(_state.claimed);
    }

    // ---------- Localização da descrição ----------

    private void SetDescriptionLocalized()
    {
        if (!descriptionText) return;

        string keySpecific = $"mission.{_state.missionId}";
        string fallback = _state.description; // snapshot do dia (continua como fallback)

        // tenta extrair um possível "nível" do ID (ex.: "win_level_3" => 3)
        int levelParam = ExtractTrailingNumber(_state.missionId);

        if (Localizer.Instance != null)
        {
            // Convenção: {0} = level (se houver), {1} = target (se necessário)
            if (levelParam > 0)
                descriptionText.text = Localizer.Instance.TrFormat(keySpecific, fallback, levelParam, _state.target);
            else
                descriptionText.text = Localizer.Instance.TrFormat(keySpecific, fallback, _state.target);
        }
        else
        {
            descriptionText.text = fallback;
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
        string goldWord = (Localizer.Instance != null) ? Localizer.Instance.Tr("currency.gold", "Gold") : "Gold";
        rewardText.text = $"+{_state.rewardGold} {goldWord}";
    }
}
