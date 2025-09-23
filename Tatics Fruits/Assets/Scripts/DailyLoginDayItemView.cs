using System;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
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

    private void SetDayLabel()
    {
        if (!dayLabel)
            return;

        int n = _index + 1;
        if (Localizer.Instance != null)
            dayLabel.text = Localizer.Instance.TrFormat("day_text", "Dia {0}", n);
        else
        {
            dayLabel.text = $"Dia {n}";
        }
    }

    public void Setup(DailyMissionsController ctrl, DailyMissionsController.DailyLoginDayInfo info)
    {
        _ctrl = ctrl;
        _index = info.Index;
        _info = info;
        
        SetDayLabel();
        if (rewardText)
            rewardText.text = $"+{info.Reward} {Localizer.Instance.Tr("prize_text","Gold")}";

        ApplyState(info);

        claimButton.onClick.RemoveAllListeners();
        claimButton.onClick.AddListener(OnClaimClicked);
    }

    private void OnEnable()
    {
        if (Localizer.Instance != null)
            Localizer.Instance.OnLanguageChanged += SetDayLabel;
        SetDayLabel();
    }

    private void OnDisable()
    {
        if (Localizer.Instance != null)
            Localizer.Instance.OnLanguageChanged -= SetDayLabel;
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
