using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        
        descriptionText.text = _state.description;
        rewardText.text = $"+{state.rewardGold} Gold";
        Refresh();
        
        claimButton.onClick.RemoveAllListeners();
        claimButton.onClick.AddListener(() =>
        {
            if (_ctrl.TryClaimMission(state.missionId))
            {
                Refresh();
            }
        });
    }

    public void Refresh()
    {
        progressBar.maxValue = Mathf.Max(1, _state.target);
        progressBar.value = Mathf.Min(_state.target, _state.progress);
        progressText.text = $"{_state.progress}/{_state.target}";
        
        claimButton.interactable = _state.completed && !_state.claimed;
        if (claimStamp)
            claimStamp.SetActive(_state.claimed);
    }
}