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

        [Header("Animation Settings")]
        [SerializeField] private float _scaleDuration = 0.2f;
        [SerializeField] private float _scaleAmount = 1.2f;

        public event Action OnLevelCompleted;

        private void Start()
        {
            _highScore = FindObjectOfType<HighScore>();
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
            if (_progressBar == null) return;

            float fillAmount = Mathf.Clamp((float)_score / _scoreToNextLevel, 0f, 1f);
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

        private void CheckLevelProgression()
        {
            if (_score >= _scoreToNextLevel)
            {
                Debug.Log($"🎉 Parabéns! Você atingiu {_scoreToNextLevel} pontos e completou o nível {_currentLevel}!");
                OnLevelCompleted?.Invoke();
                AdvanceToNextLevel();
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
