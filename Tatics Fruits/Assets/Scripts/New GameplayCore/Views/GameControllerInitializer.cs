using DefaultNamespace.New_GameplayCore;
using New_GameplayCore;
using New_GameplayCore.GameState;
using New_GameplayCore.Services;
using New_GameplayCore.Views;
using UnityEngine;

public class GameControllerInitializer : MonoBehaviour
{
    [SerializeField] private LevelConfigSO levelConfig;
    [SerializeField] private DeckConfigSO deckConfig;
    [SerializeField] private HUDView hudView;
    [SerializeField] private HandView handView;

    public IRuleEngine RuleEngine => _rule;
    public IGameController Controller => _controller;
    public bool IsReady { get; private set; }
    public event System.Action OnReady;
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
        _controller = new New_GameplayCore.Controllers.GameController(_fsm, _time, _deck, _hand, _rule, _swap, levelConfig);
        _highscores = new JsonHighScoreService();

        IsReady = true;
        OnReady?.Invoke();
    }

    private void Start()
    {
        hudView.Initialize(_time, _score, _swap);
        handView.Initialize(_hand, _controller);
        _controller.StartLevel(levelConfig, deckConfig);

        _score.OnScoreChanged += (total, delta) =>
        {
            _highscores.TryReportScore(GetLevelId(), total);
        };
    }

    private void Update()
    {
        _controller.UpdateTick(Time.deltaTime);
    }
    
    private string GetLevelId() => string.IsNullOrEmpty(levelConfig.levelId) ? levelConfig.name : levelConfig.levelId;
}