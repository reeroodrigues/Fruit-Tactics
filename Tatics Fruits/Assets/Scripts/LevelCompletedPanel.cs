using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class LevelCompletedPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject _successUI;
        [SerializeField] private GameObject _failUI;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Sprite _winSprite;
        [SerializeField] private Sprite _loseSprite;
        [SerializeField] private GameObject _starsPanel;
        [SerializeField] private GameObject[] _stars;
        [SerializeField] private Button _nextPhaseButton;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _mainMenuButton; // 👈 Novo botão!

        public void Setup(bool isSuccess, int starCount = 0)
        {
            gameObject.SetActive(true);

            _successUI.SetActive(isSuccess);
            _failUI.SetActive(!isSuccess);

            _mainMenuButton.gameObject.SetActive(true); // Mostra o botão sempre
            _mainMenuButton.onClick.RemoveAllListeners();
            _mainMenuButton.onClick.AddListener(ReturnToMainMenu); // 👈 Função para voltar

            if (isSuccess)
            {
                _titleText.text = "Parabéns!";
                _messageText.text = "Você completou a fase!";
                _iconImage.sprite = _winSprite;
                _starsPanel.SetActive(true);
                _nextPhaseButton.gameObject.SetActive(true);
                _retryButton.gameObject.SetActive(true);
                ShowStars(starCount);

                _nextPhaseButton.onClick.RemoveAllListeners();
                _nextPhaseButton.onClick.AddListener(NextPhase);
            }
            else
            {
                _titleText.text = "Tempo Esgotado!";
                _messageText.text = "Não foi dessa vez...";
                _iconImage.sprite = _loseSprite;
                _starsPanel.SetActive(false);
                _nextPhaseButton.gameObject.SetActive(true);
                _retryButton.gameObject.SetActive(true);

                _retryButton.onClick.RemoveAllListeners();
                _retryButton.onClick.AddListener(RetryPhase);
            }
        }

        private void ShowStars(int starCount)
        {
            for (int i = 0; i < _stars.Length; i++)
            {
                _stars[i].SetActive(i < starCount);
            }
        }

        private void HideAllStars()
        {
            foreach (var star in _stars)
            {
                star.SetActive(false);
            }
        }

        private void NextPhase()
        {
            var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            var nextSceneIndex = currentSceneIndex + 1;

            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                SceneManager.LoadScene("MainMenu");
            }
        }

        private void RetryPhase()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void ReturnToMainMenu()
        {
            SceneManager.LoadScene("MainMenu"); // 👈 Nome da cena do menu
        }
    }
}
