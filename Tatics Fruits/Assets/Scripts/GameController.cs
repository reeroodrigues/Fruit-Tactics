using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject _preRoundPrefab;
    [SerializeField] private Transform _uiContainer;
    [SerializeField] private Sprite _starsSprite;

    private void Start()
    {
        var preRound = Instantiate(_preRoundPrefab, _uiContainer);
        preRound.GetComponent<PreRoundPanelController>().SetupPreRound(1, "Faça 100 pontos em até 60 segundos para ganhar 3 estrelas", _starsSprite);
    }
}
