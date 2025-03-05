using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class ObjectiveManager : MonoBehaviour
    {
        public Slider _progressBar;
        public TextMeshProUGUI _progressText;
        public ScoreManager _scoreManager;

        private readonly int[] _milestones = { 50, 100, 150, 200 };
        private int _currentMilestoneIndex = 0;

        private void Start()
        {
            UpdateProgress();
        }

        private void Update()
        {
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            if (_scoreManager == null || _progressBar == null || _progressText == null)
                return;

            var score = _scoreManager.GetScore();
            var currentMilestone = _milestones[_currentMilestoneIndex];
            
            var fillAmount = Mathf.Clamp01((float)score / currentMilestone);
            
            _progressBar.DOValue(fillAmount, 0.5f);
            
            _progressText.text = $"{score} / {currentMilestone}";
            
            if (score >= currentMilestone && _currentMilestoneIndex < _milestones.Length - 1)
            {
                _currentMilestoneIndex++;
            }
        }

    }
}