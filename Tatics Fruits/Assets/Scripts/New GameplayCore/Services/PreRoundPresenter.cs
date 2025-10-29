using UnityEngine;

namespace New_GameplayCore.Services
{
    public class PreRoundPresenter : IPreRoundPresenter
    {
        private readonly Controllers.GameController _controller;
        private readonly LevelConfigSO _cfg;
        private readonly IDeckService _deck;
        private readonly IHighScoreService  _highscores;
        private PreRoundModel _model;

        public System.Action<PreRoundModel> OnModelReady;
        public System.Action OnRequestClose;

        public PreRoundPresenter(Controllers.GameController controller, LevelConfigSO cfg, IDeckService deck,
            IHighScoreService highscores)
        {
            _controller = controller;
            _cfg = cfg;
            _deck = deck;
            _highscores = highscores;
        }

        public PreRoundModel BuildModel(LevelConfigSO cfg, IDeckService deck, IHighScoreService hs)
        {
            var total = 0;
            var list = new System.Collections.Generic.List<DeckEntrySummary>();
            foreach (var e in cfg.deck.Entries)
            {
                list.Add(new DeckEntrySummary { type = e.type, quantity = e.quantity });
                total += e.quantity;
            }

            var s1 = Mathf.CeilToInt(cfg.targetScore * cfg.star1Threshold);
            var s2 = Mathf.CeilToInt(cfg.targetScore * cfg.star2Threshold);
            var s3 = cfg.targetScore;
            
            var levelId = string.IsNullOrEmpty(cfg.levelId) ? cfg.name : cfg.levelId;
            var best = hs.GetBest(levelId);

            _model = new PreRoundModel()
            {
                LevelId = levelId,
                DisplayName = string.IsNullOrEmpty(cfg.displayName) ? cfg.name : cfg.displayName,
                Description = cfg.description,

                TargetScore = cfg.targetScore,
                Star1Score = s1,
                Star2Score = s2,
                Star3Score = s3,

                InitialTimeSec = cfg.initialTimeSeconds,
                HandSize = cfg.handSize,
                TimeBonusOnPair = cfg.timeBonusOnPair,
                SwapAllPenalty = cfg.swapAllTimePenalty,
                SwapRandomPenalty = cfg.swapRandomTimePenalty,
                AllowRefillFromDiscard = cfg.allowEmptyDeckRefill,

                DeckTotalCount = total,
                Composition = list.ToArray(),
                
                BestScore = best,
                
                UseFixedSeed = cfg.useFixedSeed,
                EffectiveSeed = cfg.useFixedSeed ? cfg.fixedSeed : UnityEngine.Random.Range(int.MinValue, int.MaxValue),
            };
            
            OnModelReady?.Invoke(_model);
            return _model;
        }

        public void OnStartClicked()
        {
            _controller.BeginPlayFromPreRound(_model);
            OnRequestClose?.Invoke();
        }

        public void OnBackClicked()
        {
            _controller.BackToLevelSelect();
            OnRequestClose?.Invoke();
        }

        public void OnToggleUseFixedSeed(bool value)
        {
            _model.UseFixedSeed = value;
            _model.EffectiveSeed = value ? _cfg.fixedSeed : UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            OnModelReady?.Invoke(_model);
        }
    }
}