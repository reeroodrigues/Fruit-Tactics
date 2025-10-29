using System;
using New_GameplayCore.Services;
using TMPro;
using UnityEngine;

namespace New_GameplayCore.Views
{
    public class HighScoreView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private GameControllerInitializer bootstrap;

        private IHighScoreService _hs;
        private IScoreService _score;
        private string _levelId;

        private void Start()
        {
            _hs = bootstrap.Highscores;
            _score = bootstrap.Score;
            _levelId = string.IsNullOrEmpty(bootstrap.LevelConfig.levelId) ? bootstrap.LevelConfig.name : bootstrap.LevelConfig.levelId;

            RefreshLabel();

            _hs.OnHighScoreChanged += OnHighScoreChanged;
            _score.OnScoreChanged += OnScoreChanged;
        }

        private void OnDestroy()
        {
            if (_hs != null)
                _hs.OnHighScoreChanged -= OnHighScoreChanged;

            if (_score != null)
                _score.OnScoreChanged -= OnScoreChanged;
        }

        private void OnHighScoreChanged(string levelId, int best)
        {
            if (levelId == _levelId)
                RefreshLabel();
        }

        private void OnScoreChanged(int score, int delta)
        {
            
        }

        private void RefreshLabel()
        {
            var best = _hs.GetBest(_levelId);
            if (label)
                label.text = best.ToString();
        }
    }
}