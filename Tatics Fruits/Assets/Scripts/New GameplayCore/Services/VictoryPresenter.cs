using System;
using UnityEngine;

namespace New_GameplayCore.Services
{
    public class VictoryPresenter
    {
        private readonly LevelConfigSO _cfg;
        private readonly IScoreService _score;
        private readonly ITimeManager  _time;
        private readonly IHighScoreService _hs;

        public Action<VictoryModel> OnModelReady;
        public Action OnNextLevel;
        public Action OnReplay;

        public VictoryPresenter(LevelConfigSO cfg, IScoreService score, ITimeManager time, IHighScoreService hs)
        {
            _cfg = cfg;
            _score = score;
            _time = time;
            _hs = hs;
        }

        public void Build()
        {
            var id = string.IsNullOrEmpty(_cfg.levelId) ? _cfg.name : _cfg.levelId;
            var total = _score.Total;
            var bestBefore = _hs.GetBest(id);
            var newRecord = _hs.TryReportScore(id, total);

            var stars = 0;
            var pct = Mathf.Clamp01(total / (float)_cfg.targetScore);
            if (pct >= _cfg.star1Threshold) 
                stars = 1;
            
            if (pct >= _cfg.star2Threshold) 
                stars = 2;
            
            if (pct >= _cfg.star3Threshold) 
                stars = 3;
            OnModelReady?.Invoke(new VictoryModel
            {
                LevelId = id,
                TotalScore = total,
                BestBefore = bestBefore,
                NewRecord = newRecord,
                TargetScore = _cfg.targetScore,
                TimeLeftSeconds = _time.TimeLeftSeconds
            });
        }
        
        public void ClickNext() => OnNextLevel?.Invoke();
        public void ClickReplay() => OnReplay?.Invoke();
    }
}