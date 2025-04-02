using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace DefaultNamespace
{
    public class ScoreManager : MonoBehaviour
    {
        private int _score;
        private HighScore _highScore;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private Slider _progressBar;
        [SerializeField] private GameObject[] _stars;
        [SerializeField] private TextMeshProUGUI _levelText;

        [Header("Level Settings")]
        private readonly int[] _starThresholds = { 25, 50, 100 };
        private int _currentLevel = 1;
        private int _scoreToNextLevel = 100;
        private int _targetScore;

        [Header("Animation Settings")]
        [SerializeField] private float _scaleDuration = 0.2f;
        [SerializeField] private float _scaleAmount = 1.2f;

        [Header("Level Completed!")] 
        [SerializeField] private GameObject _levelCompletedPrefab;
        [SerializeField] private Transform _uiCanvas;
        [SerializeField] private Timer _timer;

        public event Action OnLevelCompleted;

        private void Start()
        {
            _highScore = FindObjectOfType<HighScore>();

            if (_targetScore == 0) // Evita que seja 0
            {
                _targetScore = 100;
                Debug.LogWarning("Target Score não definido, atribuindo 100 por padrão.");
            }

            UpdateScoreUI();
            UpdateProgress();
            UpdateLevelUI();
        }

        public void AddScore(int value)
        {
            _score += value;
            UpdateScoreUI();
            AnimateScore();
            UpdateProgress();
            CheckLevelProgression();
        }

        public int GetScore() => _score;

        public void SaveHighScore()
        {
            if (_highScore != null)
            {
                _highScore.TrySetHighScore(_score);
            }
        }

        public void ResetScore()
        {
            SaveHighScore();
            _score = 0;
            UpdateScoreUI();
            UpdateProgress();

            _scoreText.transform.DOScale(1.3f, 0.2f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        private void UpdateScoreUI()
        {
            if (_scoreText != null)
                _scoreText.text = _score.ToString();
        }

        private void AnimateScore()
        {
            if (_scoreText == null) return;

            _scoreText.transform.DOScale(_scaleAmount, _scaleDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() => _scoreText.transform.DOScale(1f, _scaleDuration));
        }

        private void UpdateProgress()
        {
            if (_progressBar == null || _targetScore == 0) return;

            float fillAmount = Mathf.Clamp((float)_score / _targetScore, 0f, 1f);
            Debug.Log($"Atualizando barra de progresso: {_score}/{_targetScore} ({fillAmount * 100}%)");
            _progressBar.DOValue(fillAmount, 0.5f);
            ActivateStars(_score);
        }

        private void ActivateStars(int score)
        {
            for (int i = 0; i < _stars.Length; i++)
            {
                if (i < _starThresholds.Length && score >= _starThresholds[i])
                {
                    if (!_stars[i].activeSelf)
                    {
                        _stars[i].SetActive(true);
                        AnimateStar(_stars[i]);
                    }
                }
                else
                {
                    _stars[i].SetActive(false);
                }
            }
        }
        
        public void SetTargetScore(int targetScore)
        {
            _targetScore = targetScore;
            _scoreToNextLevel = _targetScore;
            Debug.Log($"Target Score definido: {_targetScore}");
            UpdateProgress();
        }

        private void CheckLevelProgression()
        {
            Debug.Log($"Pontuação atual: {_score}, Objetivo: {_targetScore}");
            if (_score >= _targetScore)
            {
                Debug.Log("Objetivo alcançado!");
                OnLevelCompleted?.Invoke();
            }
        }

        private void AdvanceToNextLevel()
        {
            _currentLevel++;
            ResetScore();
            _scoreToNextLevel += Mathf.RoundToInt(_scoreToNextLevel * 0.5f);
            _progressBar.value = 0f;

            UpdateLevelUI();
            AnimateLevelUp();
        }

        private void UpdateLevelUI()
        {
            if (_levelText != null)
                _levelText.text = "Level " + _currentLevel;
        }

        private void AnimateLevelUp()
        {
            _levelText.transform.DOScale(1.5f, 0.3f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutBounce);
        }

        private void AnimateStar(GameObject star)
        {
            var image = star.GetComponent<Image>();

            if (image == null) return;

            star.transform.DORotate(new Vector3(0, 0, 15), 0.3f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);

            image.DOFade(1f, 0.2f)
                .From(0.5f)
                .SetLoops(2, LoopType.Yoyo);
        }
    }
}
