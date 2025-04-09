using System;
using System.Runtime.InteropServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _howToPlayButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Transform _titleTransform;
    [SerializeField] private SettingsMenu _settingsMenu;
    [SerializeField] private GameObject _tutorialPanel;

    private void Start()
    {
        _titleTransform.localScale = Vector3.zero;
        _titleTransform.DOScale(1f, 0.8f).SetEase(Ease.OutBounce);

        SetupButtonAnimation(_playButton);
        SetupButtonAnimation(_howToPlayButton);
        SetupButtonAnimation(_optionsButton);

        _playButton.onClick.AddListener(StartGame);
        _howToPlayButton.onClick.AddListener(() => _tutorialPanel.SetActive(true));
        _optionsButton.onClick.AddListener(OpenOptions);
    }

    private void SetupButtonAnimation(Button button)
    {
        button.transform.localScale = Vector3.one * 0.9f;
        
        var trigger = button.gameObject.AddComponent<EventTrigger>();

        var pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener((eventData) => button.transform.DOScale(1f, 0.2f));
        trigger.triggers.Add(pointerEnter);

        var pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener((eventData) => button.transform.DOScale(0.9f, 0.2f));
        trigger.triggers.Add(pointerExit);
    }

    private void StartGame() => SceneManager.LoadScene("Gameplay Scene");
    private void OpenShop() => Debug.Log("Abrindo Loja...");
    private void OpenOptions()
    {
            _settingsMenu.gameObject.SetActive(true);

            _settingsMenu.transform.localScale = Vector3.zero;
            _settingsMenu.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
    }

}
