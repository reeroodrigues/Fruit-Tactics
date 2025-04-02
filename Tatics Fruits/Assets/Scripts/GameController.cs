using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject _preRoundPrefab;
    [SerializeField] private Transform _uiContainer;
    [SerializeField] private Sprite _starsSprite;

    private void Start()
    {
        var (points, time) = GetRandomObjective();
        var objectiveDescription = $"Obtenha {points} pontos em {time} segundos.";

        var preRound = Instantiate(_preRoundPrefab, _uiContainer);
        preRound.GetComponent<PreRoundPanelController>().SetupPreRound(1, objectiveDescription, _starsSprite, points, time); // Passando também o tempo
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
}
