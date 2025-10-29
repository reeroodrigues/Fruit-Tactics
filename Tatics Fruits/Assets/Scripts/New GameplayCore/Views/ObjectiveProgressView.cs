using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace New_GameplayCore.Views
{
    public class ObjectiveProgressView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image progressFill;
        [SerializeField] private Image star1;
        [SerializeField] private Image star2;
        [SerializeField] private Image star3;
        
        [Header("Refs")]
        [SerializeField] private GameControllerInitializer bootstrap;

        private IScoreService _score;
        private LevelConfigSO _cfg;

        private void Start()
        {
            _score = bootstrap.Score;
            _cfg = bootstrap.LevelConfig;
            
            Refresh(_score.Total, 0);

            _score.OnScoreChanged += Refresh;
        }

        private void OnDestroy()
        {
            if (_score != null)
                _score.OnScoreChanged -= Refresh;
        }

        private void Refresh(int total, int delta)
        {
            var target = Mathf.Max(1, _cfg.targetScore);
            var pct = Mathf.Clamp01(total / (float)target);

            if (progressFill)
                progressFill.DOFillAmount(pct, 0.25f);

            if (star1)
                star1.enabled = pct >= _cfg.star1Threshold;
            
            if (star2)
                star2.enabled = pct >= _cfg.star2Threshold;
            
            if(star3)
                star3.enabled = pct >= _cfg.star3Threshold;
        }
    }
}