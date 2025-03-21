using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class ObjectiveManager : MonoBehaviour
    {
        public Slider _progressBar;
        public ScoreManager _scoreManager;
        public GameObject[] _stars;
        public TextMeshProUGUI _levelText;
        
        private readonly int[] _starThresholds = { 25, 50, 100 };
        private int _currentLevel = 1;
        private int _scoreToNextLevel = 100;

        private void Start()
        {
            UpdateProgress();
            UpdateLevelUI();
        }

        private void Update()
        {
            UpdateProgress();
            CheckLevelProgression();
        }

        private void UpdateProgress()
        {
            if (_scoreManager == null || _progressBar == null)
                return;

            var score = _scoreManager.GetScore();
            var fillAmount = Mathf.Clamp((float) score / _scoreToNextLevel, 0f, 1f);

            _progressBar.DOValue(fillAmount, 0.5f);
            ActivateStars(score);
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
            if (_scoreManager.GetScore() >= _scoreToNextLevel)
            {
                AdvanceToNextLevel();
            }
        }

        private void AdvanceToNextLevel()
        {
            _currentLevel++;
            _scoreManager.ResetScore();
            _scoreToNextLevel += Mathf.RoundToInt(_scoreToNextLevel * 0.5f);
            _progressBar.value = 0f;

            UpdateLevelUI();
            AnimateLevelUp();
        }

        private void AnimateLevelUp()
        {
            _levelText.transform.DOScale(1.5f, 0.3f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutBounce);
        }

        private void UpdateLevelUI()
        {
            if (_levelText != null)
                _levelText.text = "Level" + _currentLevel;
        }

        private void AnimateStar(GameObject star)
        {
            var image = star.GetComponent<Image>();

            if (image == null) return;
            
            star.transform.DORotate(new Vector3(0, 0, 15), 0.3f)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
            
            image.DOFade(1f, 0.2f).From(0.5f).SetLoops(2, LoopType.Yoyo);
        }
    }
}
