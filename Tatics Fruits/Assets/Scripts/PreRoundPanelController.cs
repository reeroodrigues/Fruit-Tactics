using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreRoundPanelController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _objectiveText;
    [SerializeField] private Image _starsImage;
    [SerializeField] private Button _backToMenuButton;
    [SerializeField] private Button _startPhaseButton;
    [SerializeField] private Timer _timer;
    
    private void Start()
    {
        _backToMenuButton.onClick.AddListener(BackToMenu);
        _startPhaseButton.onClick.AddListener(StartPhase);
    }

    public void SetupPreRound(int phaseNumber, string objectiveDescription, Sprite starsSprite, int targetScore, int totalTime)
    {
        _objectiveText.text = objectiveDescription;
        _starsImage.sprite = starsSprite;

        if (_timer == null)
        {
            _timer = FindObjectOfType<Timer>();
        }

        if (_timer != null)
        {
            _timer.SetTotalTime(totalTime);
        }

        var scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            scoreManager.SetTargetScore(targetScore);
        }
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void StartPhase()
    {
        gameObject.SetActive(false);
    
        if (_timer == null)
        {
            _timer = FindObjectOfType<Timer>();
        }

        if (_timer != null)
        {
            Debug.Log("Iniciando Timer...");
            _timer.StartTimer();
        }
        else
        {
            Debug.LogError("Timer não encontrado na cena!");
        }
    }
}