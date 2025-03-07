using UnityEngine;
using TMPro;
using DG.Tweening; // Certifique-se de importar DOTween

namespace DefaultNamespace
{
    public class ScoreManager : MonoBehaviour
    {
        private int _score;
        private HighScore _highScore;
        
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _scoreText; // Referência ao placar

        [Header("Animation Settings")]
        [SerializeField] private float _scaleDuration = 0.2f; // Tempo da animação
        [SerializeField] private float _scaleAmount = 1.2f; // Tamanho ao crescer

        private void Start()
        {
            _highScore = FindObjectOfType<HighScore>();
            UpdateScoreUI();
        }

        public void AddScore(int value)
        {
            _score += value;
            UpdateScoreUI();
            AnimateScore();
        }

        public int GetScore()
        {
            return _score;
        }

        public void SaveHighScore()
        {
            if (_highScore != null)
            {
                _highScore.TrySetHighScore(_score);
            }
        }

        private void UpdateScoreUI()
        {
            if (_scoreText != null)
            {
                _scoreText.text = _score.ToString();
            }
        }

        private void AnimateScore()
        {
            if (_scoreText == null) return;

            _scoreText.transform.DOScale(_scaleAmount, _scaleDuration) // Aumenta o tamanho
                .SetEase(Ease.OutBack)
                .OnComplete(() => _scoreText.transform.DOScale(1f, _scaleDuration)); // Volta ao normal
        }
    }
}