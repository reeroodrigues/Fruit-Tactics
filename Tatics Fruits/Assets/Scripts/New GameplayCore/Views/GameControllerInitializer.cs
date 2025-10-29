using System;
using DefaultNamespace.New_GameplayCore;
using New_GameplayCore.GameState;
using New_GameplayCore.Services;
using UnityEngine;

namespace New_GameplayCore.Views
{
    public class GameControllerInitializer : MonoBehaviour
    {
        [SerializeField] private LevelConfigSO levelConfig;
        [SerializeField] private DeckConfigSO deckConfig;
        [SerializeField] private HUDView hudView;
        [SerializeField] private HandView handView;
        [SerializeField] private PreRoundView preRoundView;
        [SerializeField] private Transform uiRoot;
        [SerializeField] private VictoryView victoryPrefab;

        public IRuleEngine RuleEngine => _rule;
        public IGameController Controller => _controller;
        public bool IsReady { get; private set; }
        public event Action OnReady;
        public event Action<EndCause> OnLevelEnd;

        public IHandService Hand => _hand;
        public IDeckService Deck => _deck;
        public LevelConfigSO LevelConfig => levelConfig;
        public IScoreService Score => _score;
        public IHighScoreService Highscores => _highscores;

        private GameStateMachine _fsm;
        private TimeManager _time;
        private ScoreService _score;
        private ComboTracker _combo;
        private DeckService _deck;
        private HandService _hand;
        private SwapService _swap;
        private RuleEngine _rule;
        private IGameController _controller;
        private IHighScoreService _highscores;
        private PreRoundPresenter _preRoundPresenter;
        private PreRoundView _preRoundInstance;

        private void Awake()
        {
            _fsm   = new GameStateMachine();
            _time  = new TimeManager(levelConfig.initialTimeSeconds);
            _score = new ScoreService(levelConfig);
            _combo = new ComboTracker(levelConfig);
            _deck  = new DeckService();
            _hand  = new HandService(levelConfig.handSize);
            _swap  = new SwapService(_hand, _deck, _time, levelConfig);
            _rule  = new RuleEngine(_hand, _deck, _score, _time, _combo, levelConfig);
            _controller = new New_GameplayCore.Controllers.GameController(_fsm, _time, _deck, _hand, _rule, _swap, levelConfig, _score);
            _highscores = new JsonHighScoreService();

            IsReady = true;
            OnReady?.Invoke();
            _controller.OnEnterPreRound += HandleEnterPreRound;
            _controller.OnLevelEnded += HandleLevelEnded;
        }

        private void HandleLevelEnded(EndCause cause)
        {
            if(cause != EndCause.TargetReached)
                return;
                
            var presenter = new VictoryPresenter(levelConfig, _score, _time, _highscores);
            presenter.OnNextLevel += () =>
            {
                //TODO: Load next phase
            };
            presenter.OnReplay += () =>
            {
                //TODO: Replay phase
            };
            
            var model = default(VictoryModel);
            presenter.OnModelReady += m => model = m;
            presenter.Build();
            
            var view = Instantiate(victoryPrefab, uiRoot);
            view.Bind(presenter, model);
        }

        private void Start()
        {
            _controller.StartLevel(levelConfig, deckConfig);
            hudView.Initialize(_time, _score, _swap);
            handView.Initialize(_hand, _controller);

            _score.OnScoreChanged += (total, delta) =>
            {
                _highscores.TryReportScore(GetLevelId(), total);
            };
        }

        private void OnDestroy()
        {
            if (_controller != null)
                _controller.OnEnterPreRound -= HandleEnterPreRound;
            
            if (_controller != null)
                _controller.OnLevelEnded -= HandleLevelEnded;
        }

        private void HandleEnterPreRound()
        {
            _preRoundPresenter = new PreRoundPresenter(
                _controller as Controllers.GameController,
                levelConfig,
                _deck,
                _highscores);

            var model = _preRoundPresenter.BuildModel(levelConfig, _deck, _highscores);

            _preRoundInstance = Instantiate(preRoundView,uiRoot);
            _preRoundInstance.Bind(_preRoundPresenter, model);
        }

        private void Update()
        {
            _controller.UpdateTick(Time.deltaTime);
        }
    
        private string GetLevelId() => string.IsNullOrEmpty(levelConfig.levelId) ? levelConfig.name : levelConfig.levelId;
    }
}