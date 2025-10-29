using UnityEngine;
using UnityEngine.SceneManagement;

namespace New_GameplayCore.Services
{
    [System.Serializable]
    public struct DefeatModel
    {
        public string levelId;
        public int totalScore;
        public int targetScore;
        public int starsEarned;
        public int bestBefore;
        public bool newRecord;
        public int timeLeftSeconds;
    }

    public class DefeatPresenter
    {
        private readonly LevelConfigSO _cfg;
        private readonly IScoreService _score;
        private readonly ITimeManager _time;
        private readonly IHighScoreService _hs;

        public System.Action<DefeatModel> OnModelReady;
        public System.Action OnReplay;
        public System.Action OnMenu;

        public DefeatPresenter(LevelConfigSO cfg, IScoreService score, ITimeManager time, IHighScoreService hs)
        {
            _cfg = cfg; _score = score; _time = time; _hs = hs;
        }

        public void Build()
        {
            var id = string.IsNullOrEmpty(_cfg.levelId) ? _cfg.name : _cfg.levelId;
            var total = _score.Total;
            var bestBefore = _hs.GetBest(id);
            var newRecord = _hs.TryReportScore(id, total);
            
            var pct = Mathf.Clamp01(total / (float)_cfg.targetScore);
            var stars = 0;
            if (pct >= _cfg.star1Threshold) stars = 1;
            if (pct >= _cfg.star2Threshold) stars = 2;
            if (pct >= _cfg.star3Threshold) stars = 3;

            OnModelReady?.Invoke(new DefeatModel
            {
                levelId = id,
                totalScore = total,
                targetScore = _cfg.targetScore,
                starsEarned = stars,
                bestBefore = bestBefore,
                newRecord = newRecord,
                timeLeftSeconds = _time.TimeLeftSeconds
            });
        }

        public void ClickReplay() => OnReplay?.Invoke();

        public void ClickMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}