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
        [SerializeField] private Sprite starOn;
        [SerializeField] private Sprite starOff;
        
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
                scoreText.text = $"Você fez: {model.totalScore} pontos!";
            
            if(recordText)
                recordText.text = model.newRecord ? "Novo Recorde!" : $"{model.bestBefore}";

            SetStar(star1, model.starsEarned >= 1);
            SetStar(star2, model.starsEarned >= 2);
            SetStar(star3, model.starsEarned >= 3);
            
            nextButton.onClick.AddListener(_presenter.ClickNext);
            replayButton.onClick.AddListener(_presenter.ClickReplay);
            menuButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("MainMenu");
            });
            
            Show();
        }

        private void SetStar(Image img, bool on)
        {
            if (!img) return;
            img.enabled = true;
            img.sprite  = on ? starOn : starOff;
            img.color   = Color.white;
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