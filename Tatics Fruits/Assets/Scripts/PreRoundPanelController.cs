using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreRoundPanelController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _phaseNameText;
    [SerializeField] private TextMeshProUGUI _objectiveText;
    [SerializeField] private Image _starsImage;
    [SerializeField] private Button _backToMenuButton;
    [SerializeField] private Button _startPhaseButton;
    [SerializeField] private Button _skipPhaseButton;

    public void SetupPreRound(int phaseNumber, string objectiveDescription, Sprite starsSprite)
    {
        _phaseNameText.text = $"Fase {phaseNumber}";
        _objectiveText.text = objectiveDescription;
        _starsImage.sprite = starsSprite;
    }

    private void Start()
    {
        _backToMenuButton.onClick.AddListener(BackToMenu);
        _startPhaseButton.onClick.AddListener(StartPhase);
        _skipPhaseButton.onClick.AddListener(SkipPhase);
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void StartPhase()
    {
        gameObject.SetActive(false);
    }

    private void SkipPhase()
    {
        Debug.Log("Fase Pulada!");
    }
}
