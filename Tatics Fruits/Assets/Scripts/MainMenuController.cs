using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _profileButton;
    [SerializeField] private Transform _titleTransform;
    [SerializeField] private SettingsMenuController _settingsMenu;
    [SerializeField] private PlayerProfileController _playerProfileController;

    private void Start()
    {
        _optionsButton.onClick.AddListener(_settingsMenu.OpenSettings);
        _titleTransform.localScale = Vector3.zero;
        _titleTransform.DOScale(1f, 0.8f).SetEase(Ease.OutBounce);

        SetupButtonAnimation(_playButton);
        SetupButtonAnimation(_shopButton);
        SetupButtonAnimation(_optionsButton);
        SetupButtonAnimation(_profileButton);

        _playButton.onClick.AddListener(StartGame);
        _shopButton.onClick.AddListener(OpenShop);
        _optionsButton.onClick.AddListener(OpenOptions);
        _profileButton.onClick.AddListener(OpenProfile);
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
    private void OpenOptions() => Debug.Log("Abrindo Opções...");
    private void OpenProfile() => _playerProfileController.OpenProfile();
}
