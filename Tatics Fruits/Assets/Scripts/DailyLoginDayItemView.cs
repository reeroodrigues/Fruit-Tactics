using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyLoginDayItemView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI dayLabel;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Button claimButton;
    [SerializeField] private CanvasGroup dimGroup;
    [SerializeField] private GameObject glow;

    private DailyMissionsController _ctrl;
    private int _index;
    private bool _claiming;

    private DailyMissionsController.DailyLoginDayInfo _info;

    public void Setup(DailyMissionsController ctrl, DailyMissionsController.DailyLoginDayInfo info)
    {
        _ctrl = ctrl;
        _index = info.Index;
        _info = info;

        if (dayLabel)   dayLabel.text = $"Dia {_index + 1}";
        if (rewardText) rewardText.text = $"+{info.Reward} Gold";

        ApplyState(info);

        claimButton.onClick.RemoveAllListeners();
        claimButton.onClick.AddListener(OnClaimClicked);
    }

    public void Refresh(DailyMissionsController.DailyLoginDayInfo info)
    {
        _info = info;
        if (rewardText) rewardText.text = $"+{info.Reward} Gold";
        ApplyState(info);
    }

    private void ApplyState(DailyMissionsController.DailyLoginDayInfo info)
    {
        if (dimGroup) dimGroup.alpha = info.Claimed ? 0.4f : 1f;
        if (glow)     glow.SetActive(info.Claimable);
        if (claimButton) claimButton.interactable = info.Claimable && !_claiming;
    }

    private void OnClaimClicked()
    {
        if (_claiming) return;
        _claiming = true;
        if (claimButton) claimButton.interactable = false;
        
        bool ok = _ctrl != null && _ctrl.TryClaimDailyLoginDay(_index);

        if (ok)
        {
            _info.Claimed = true;
            _info.Claimable = false;
            Refresh(_info);
        }
        else
        {
            _claiming = false;
            if (claimButton) claimButton.interactable = _info.Claimable;
        }
    }
}
