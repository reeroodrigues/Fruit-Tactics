using New_GameplayCore.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace New_GameplayCore.Views
{
    public class DefeatView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image blocker;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI recordText;
        [SerializeField] private Image star1, star2, star3;

        [Header("Buttons")]
        [SerializeField] private Button replayButton;
        [SerializeField] private Button menuButton;

        private DefeatPresenter _presenter;

        public void Bind(DefeatPresenter presenter, DefeatModel model)
        {
            _presenter = presenter;

            if (titleText)  titleText.text = "Tempo esgotado! Não foi dessa vez.";
            if (scoreText)  scoreText.text = $"Você fez: {model.totalScore} pontos.";
            if (recordText) recordText.text = model.newRecord ? "Novo recorde!" : $"Recorde: {model.bestBefore}";

            SetStar(star1, model.starsEarned >= 1);
            SetStar(star2, model.starsEarned >= 2);
            SetStar(star3, model.starsEarned >= 3);

            if (replayButton) { replayButton.onClick.RemoveAllListeners(); replayButton.onClick.AddListener(_presenter.ClickReplay); }
            if (menuButton)   { menuButton.onClick.RemoveAllListeners();   menuButton.onClick.AddListener(_presenter.ClickMenu); }

            Show();
        }

        private void SetStar(Image img, bool on)
        {
            if (!img) return;
            img.enabled = true;
        }

        private void Show()
        {
            gameObject.SetActive(true);
            if (canvasGroup) canvasGroup.alpha = 1f;
            if (blocker) blocker.raycastTarget = true;
        }
    }
}