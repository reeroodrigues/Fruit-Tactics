using System;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject preRoundPrefab;
    [SerializeField] private Transform uiContainer;
    [SerializeField] private Sprite starsSprite;

    private GameObject _preRoundInstance;

    [Obsolete("Obsolete")]
    private void Start()
    {
        var phase = GameSession._phaseNumber > 0 ? GameSession._phaseNumber : 1;
        var points = GameSession._targetScore > 0 ? GameSession._targetScore : 100;
        var time = GameSession._totalTime > 0 ? GameSession._totalTime : 60;
        var objectiveDescription = !string.IsNullOrEmpty(GameSession._objectiveDescription)
            ? GameSession._objectiveDescription
            : $"Score {points} points in {time} seconds.";

        GameSession._phaseNumber = phase;
        GameSession._targetScore = points;
        GameSession._totalTime = time;
        GameSession._objectiveDescription = objectiveDescription;

        ShowPreRoundPanel(phase);
    }


    private (int points, int time) GetRandomObjective()
    {
        var objectives = new (int points, int time)[]
        {
            (100, 60),
            (50, 30),
            (150, 90),
            (200, 120)
        };

        return objectives[UnityEngine.Random.Range(0, objectives.Length)];
    }

    [Obsolete("Obsolete")]
    public void StartNewPhase()
    {
        GameSession._phaseNumber++;
        ShowPreRoundPanel(GameSession._phaseNumber);
        var (points, time) = GetRandomObjective();
        GameSession._phaseNumber = 2;
        GameSession._targetScore = points;
        GameSession._totalTime = time;
        GameSession._objectiveDescription = $"Score {points} points in {time} seconds.";
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    [Obsolete("Obsolete")]
    private void ShowPreRoundPanel(int phaseNumber)
    {
        var points = GameSession._targetScore > 0 ? GameSession._targetScore : 100;
        var time = GameSession._totalTime > 0 ? GameSession._totalTime : 60;
        var objectiveDescription = !string.IsNullOrEmpty(GameSession._objectiveDescription)
            ? GameSession._objectiveDescription
            : $"Score {points} points in {time} seconds.";

        if (_preRoundInstance == null)
        {
            _preRoundInstance = Instantiate(preRoundPrefab, uiContainer);
        }
        else
        {
            _preRoundInstance.SetActive(true);
        }

        _preRoundInstance.GetComponent<PreRoundPanelController>().SetupPreRound(
            phaseNumber,
            objectiveDescription,
            starsSprite,
            points,
            time
        );
    }

}