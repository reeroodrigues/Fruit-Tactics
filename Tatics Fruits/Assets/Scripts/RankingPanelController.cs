using TMPro;
using UnityEngine;

public class RankingPanelController : MonoBehaviour
{
    [Header("Agregados")]
    [SerializeField] private TextMeshProUGUI maxLevelText;
    [SerializeField] private TextMeshProUGUI maxScoreText;
    [SerializeField] private TextMeshProUGUI bestTimeText;

    [Header("Lista de runs")]
    [SerializeField] private Transform content;    // parent do ScrollView/Content
    [SerializeField] private RunRowUI rowPrefab;   // prefab de uma linha

    private void OnEnable() => Refresh();

    [ContextMenu("Refresh Now")]
    public void Refresh()
    {
        var data = RankingService.Load();

        if (maxLevelText) maxLevelText.text = data.maxLevel.ToString();
        if (maxScoreText) maxScoreText.text = data.maxScore.ToString();
        if (bestTimeText) bestTimeText.text = RankingService.FormatTime(data.bestTimeSeconds);

        // limpa lista
        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);

        // popula (limite visual opcional)
        int show = Mathf.Min(20, data.topRuns.Count);
        for (int i = 0; i < show; i++)
        {
            var row = Instantiate(rowPrefab, content);
            row.Bind(i + 1, data.topRuns[i]); // posição começa em 1
        }
    }
}