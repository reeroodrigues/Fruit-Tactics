using System;
using DefaultNamespace;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private int _score;
    private HighScore _highScore;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Level Settings")]
    private readonly int[] _starThresholds = { 25, 50, 100 };
    private int _currentLevel = 1;
    public int CurrentLevel => _currentLevel;
    private int _scoreToNextLevel = 100;
    private int _targetScore;

    [Header("Level Completed Panels")]
    [SerializeField] private GameObject victoryPanelPrefab;
    [SerializeField] private GameObject defeatPanelPrefab;
    [SerializeField] private Transform uiCanvas;
    [SerializeField] private Timer timer;
    [SerializeField] private GameController gameController;

    [Header("Score Animation")]
    [SerializeField] private float scaleAmount = 1.3f;
    [SerializeField] private float scaleDuration = 0.2f;

    private GameObject _currentPanel;
    private bool _hasEnded = false;

    public event Action OnLevelCompleted;

    [Obsolete("Obsolete")]
    private void Start()
    {
        _highScore = FindObjectOfType<HighScore>();
        
        _currentLevel = GameSession._currentLevel > 0 ? GameSession._currentLevel : 1;

        if (_targetScore == 0)
        {
            _targetScore = 100;
        }

        UpdateScoreUI();
        UpdateProgress();
        UpdateLevelUI();

        OnLevelCompleted += HandleEndOfRound;

        if (timer != null)
        {
            timer.OnRoundEnd += HandleEndOfRound;
        }
    }

    public void AddScore(int value)
    {
        _score += value;
        UpdateScoreUI();
        AnimateScore();
        UpdateProgress();
        CheckLevelProgression();
    }

    public int GetScore() => _score;

    public void SaveHighScore()
    {
        if (_highScore != null)
        {
            _highScore.TrySetHighScore(_score);
        }
    }

    public void ResetScore()
    {
        _score = 0;
        SaveHighScore();
        UpdateScoreUI();
        UpdateProgress();

        scoreText.transform.DOScale(1.3f, 0.2f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
        
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = _score.ToString();
    }

    private void AnimateScore()
    {
        if (scoreText == null) return;

        scoreText.transform.DOScale(scaleAmount, scaleDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() => scoreText.transform.DOScale(1f, scaleDuration));
    }

    private void UpdateProgress()
    {
        if (progressBar == null || _targetScore == 0) return;

        float fillAmount = Mathf.Clamp((float)_score / _targetScore, 0f, 1f);
        progressBar.DOValue(fillAmount, 0.5f);
        ActivateStars(_score);
    }

    private void ActivateStars(int score)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            if (i < _starThresholds.Length && score >= _starThresholds[i])
            {
                if (!stars[i].activeSelf)
                {
                    stars[i].SetActive(true);
                    
                    stars[i].transform.localScale = Vector3.zero;
                    
                    AnimateStar(stars[i]);
                }
            }
            else
            {
                stars[i].SetActive(false);
            }
        }
    }

    public void SetTargetScore(int targetScore)
    {
        _targetScore = targetScore;
        _scoreToNextLevel = _targetScore;
        UpdateProgress();
    }

    private void CheckLevelProgression()
    {
        if (_score >= _targetScore)
        {
            OnLevelCompleted?.Invoke();
        }
    }

    public void HandleEndOfRound()
    {
        if (_hasEnded) return;

        _hasEnded = true;
        SaveHighScore();

        if (timer != null)
        {
            timer.StopTimer();
        }

        var victory = _score >= _targetScore;
        ShowLevelCompletedPanel(victory);
        
    }

    private void ShowLevelCompletedPanel(bool success)
    {
        if (uiCanvas == null) return;

        if (_currentPanel != null)
        {
            Destroy(_currentPanel);
        }

        var panelPrefab = success ? victoryPanelPrefab : defeatPanelPrefab;

        if (panelPrefab == null) return;

        _currentPanel = Instantiate(panelPrefab, uiCanvas);
        _currentPanel.SetActive(true);

        var panelController = _currentPanel.GetComponent<LevelCompletedPanel>();
        if (panelController != null)
        {
            var starsEarned = CalculateEarnedStars();
            panelController.Setup(success, success ? starsEarned : 0);

            panelController.SetGameController(gameController);
            panelController.SetScoreManager(this);
        }
    }

    private int CalculateEarnedStars()
    {
        int count = 0;
        foreach (var threshold in _starThresholds)
        {
            if (_score >= threshold) count++;
        }
        return count;
    }

    private void AdvanceToNextLevel()
    {
        _currentLevel++;
        GameSession._currentLevel = _currentLevel;
        
        _scoreToNextLevel += Mathf.RoundToInt(_scoreToNextLevel * 0.5f);

        if (progressBar != null)
            progressBar.value = 0f;

        UpdateLevelUI();
        AnimateLevelUp();
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
            levelText.text = "Level " + _currentLevel;
    }

    private void AnimateLevelUp()
    {
        levelText.transform.DOScale(1.5f, 0.3f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutBounce);
    }

    private void AnimateStar(GameObject star)
    {
        star.transform.DOScale(1.5f, 0.2f).SetEase(Ease.OutBack)
            .OnComplete(() => { star.transform.DOScale(1f, 0.1f); });

        star.transform.DORotate(new Vector3(0, 0, -360), 0.5f, RotateMode.FastBeyond360)
            .SetEase(Ease.OutQuad);
    }

    public void AdvancedToNextLevelExternally()
    {
        _hasEnded = false;
        AdvanceToNextLevel();
    }
}
