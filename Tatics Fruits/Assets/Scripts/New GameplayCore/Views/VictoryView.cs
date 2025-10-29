using New_GameplayCore.Services;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace New_GameplayCore.Views
{
    public class VictoryView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image blocker;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI recordText;
        [SerializeField] private Image star1;
        [SerializeField] private Image star2;
        [SerializeField] private Image star3;
        
        [Header("Buttons")]
        [SerializeField] private Button nextButton;
        [SerializeField] private Button replayButton;
        [SerializeField] private Button menuButton;

        private VictoryPresenter _presenter;

        public void Bind(VictoryPresenter presenter, VictoryModel model)
        {
            _presenter = presenter;

            if (titleText)
                titleText.text = "Vitória!";

            if (scoreText)
                scoreText.text = $"{model.TotalScore} / {model.TargetScore}";
            
            if(recordText)
                recordText.text = model.NewRecord ? "Novo Recorde!" : $"Recorde: {model.BestBefore}";

            SetStar(star1, model.StarsEarned >= 1);
            SetStar(star2, model.StarsEarned >= 2);
            SetStar(star3, model.StarsEarned >= 3);
            
            nextButton.onClick.AddListener(_presenter.ClickNext);
            replayButton.onClick.AddListener(_presenter.ClickReplay);
            menuButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("MainMenu");
            });
        }

        private void SetStar(Image img, bool on)
        {
            if(!img)
                return;
            
            img.enabled = on;
        }

        private void Show()
        {
            gameObject.SetActive(true);
            if (canvasGroup)
                canvasGroup.alpha = 1f;

            if (blocker)
                blocker.raycastTarget = true;
        }

    }
}