using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private RectTransform settingsPanel;
    [SerializeField] private Button settingsButton;
    [SerializeField] private GameObject dimmer;

    [Header("Anim")] 
    [SerializeField, Min(0.05f)] private float duration = 0.25f;
    [SerializeField] private Ease easeIn = Ease.InOutCubic;
    [SerializeField] private Ease easeOut = Ease.InCubic;

    private bool _isOpen;
    private float _panelHeight;
    private Tweener _tween;

    private void Awake()
    {
        _panelHeight = settingsPanel.rect.height;

        var pos = settingsPanel.anchoredPosition;
        pos.y = _panelHeight;
        settingsPanel.anchoredPosition = pos;

        if (dimmer != null)
            dimmer.SetActive(false);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(Toggle);

        if (dimmer != null)
        {
            var dimmerBtn = dimmer.GetComponent<Button>();
            if (dimmerBtn != null)
                dimmerBtn.onClick.AddListener(Close);
        }
    }

    public void Toggle()
    {
        if (_isOpen)
            Close();
        else
            Open();
    }

    public void Open()
    {
        if (_tween != null && _tween.IsActive())
            _tween.Kill();

        if (dimmer != null)
            dimmer.SetActive(true);

        _isOpen = true;
        _tween = settingsPanel.DOAnchorPosY(0f, duration).SetEase(easeIn).SetUpdate(true);
    }

    public void Close()
    {
        if (_tween != null && _tween.IsActive())
            _tween.Kill();

        _isOpen = false;
        _tween = settingsPanel.DOAnchorPosY(_panelHeight, duration).SetEase(easeOut).SetUpdate(true).OnComplete(() =>
        {
            if (dimmer != null)
                dimmer.SetActive(false);
        });
    }
}