using TMPro;
using UnityEngine;

public class RunRowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI posText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;

    public void Bind(int position, RunRecord rec)
    {
        if (posText)   posText.text = position.ToString();
        if (levelText) levelText.text = $"Lv {rec.level}";
        if (scoreText) scoreText.text = rec.score.ToString();
        if (timeText)  timeText.text = RankingService.FormatTime(rec.timeSeconds);
    }
}