using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyLoginDayItemView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI dayLabel;    // "Dia 1" .. "Dia 7"
    [SerializeField] private TextMeshProUGUI rewardText;  // "+XX Gold"
    [SerializeField] private Button claimButton;          // botão de coletar
    [SerializeField] private CanvasGroup dimGroup;        // alpha = 0.4 quando claimed
    [SerializeField] private GameObject glow;             // ativar quando claimable

    private DailyMissionsController _ctrl;
    private int _index;

    public void Setup(DailyMissionsController ctrl, DailyMissionsController.DailyLoginDayInfo info)
    {
        _ctrl = ctrl;
        _index = info.index;

        if (dayLabel)   dayLabel.text = $"Dia {_index + 1}";
        if (rewardText) rewardText.text = $"+{info.reward} Gold";

        ApplyState(info);

        claimButton.onClick.RemoveAllListeners();
        claimButton.onClick.AddListener(OnClaimClicked);
    }

    public void Refresh(DailyMissionsController.DailyLoginDayInfo info)
    {
        if (rewardText) rewardText.text = $"+{info.reward} Gold";
        ApplyState(info);
    }

    private void ApplyState(DailyMissionsController.DailyLoginDayInfo info)
    {
        if (dimGroup) dimGroup.alpha = info.claimed ? 0.4f : 1f;
        if (glow)     glow.SetActive(info.claimable);
        if (claimButton) claimButton.interactable = info.claimable;
    }

    private void OnClaimClicked()
    {
        if (_ctrl != null && _ctrl.TryClaimDailyLoginDay(_index))
        {
            // a grid vai receber o callback e chamar Refresh()
        }
    }
}