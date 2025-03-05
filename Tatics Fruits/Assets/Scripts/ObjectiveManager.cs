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
        public ScoreManager _scoreManager;

        public GameObject[] _stars;

        private readonly int[] _milestones = { 25, 50, 100};
        private readonly int[] _starThresholds = { 25, 50, 100 };
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
            if (_scoreManager == null || _progressBar == null)
                return;

            var score = _scoreManager.GetScore();
            var currentMilestone = _milestones[_currentMilestoneIndex];
            
            var fillAmount = Mathf.Clamp01((float)score / currentMilestone);
            
            _progressBar.DOValue(fillAmount, 0.5f);

            ActivateStars(score);
            
            if (score >= currentMilestone && _currentMilestoneIndex < _milestones.Length - 1)
            {
                _currentMilestoneIndex++;
            }
        }

        private void ActivateStars(int score)
        {
            for (int i = 0; i < _stars.Length; i++)
            {
                if (i < _starThresholds.Length && score >= _starThresholds[i])
                {
                    _stars[i].SetActive(true);
                }
                else
                {
                    _stars[i].SetActive(false);
                }
            }
        }
    }
}