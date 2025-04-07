using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Image _timerImage;
    public TextMeshProUGUI _timerText;
    public GameObject _addedTimeTextPrefab;
    public Transform _addedTimeTextParent;
    public float _totalTime;
    public float _remainingTime;
    public event Action OnRoundEnd;

    private bool _isPulsing = false;
    private bool _isRunning = false;

    public void SetTotalTime(float totalTime)
    {
        _totalTime = totalTime;
        _remainingTime = _totalTime;
        UpdateTimerText();
        
        {
            Debug.Log("Aguardando clique do botão Start...");
        };
    }

    public void StartTimer()
    {
        _isRunning = true;
        Debug.Log("Timer iniciado!");
    }

    private void Update()
    {
        if (!_isRunning) return;

        if (_remainingTime > 0)
        {
            _remainingTime -= Time.deltaTime;
            _timerImage.fillAmount = _remainingTime / _totalTime;
            UpdateTimerText();

            if (_remainingTime <= 10 && !_isPulsing)
            {
                StartPulsingEffect();
            }
        }
        else
        {
            StopTimer();
            EndRound();
        }
    }

    public void StopTimer()
    {
        _isRunning = false;
        StopPulsingEffect();
    }

    public void AddTime(float timeToAdd)
    {
        _remainingTime += timeToAdd;
        _remainingTime = Mathf.Clamp(_remainingTime, 0, _totalTime);
        AnimateTimerText();
        ShowAddedTimeEffect(timeToAdd);
    }

    private void ShowAddedTimeEffect(float timeToAdd)
    {
        if (_addedTimeTextPrefab == null || _addedTimeTextParent == null) return;
    
        var addedTimeText = Instantiate(_addedTimeTextPrefab, _addedTimeTextParent);
        var textComponent = addedTimeText.GetComponent<TextMeshProUGUI>();

        if (textComponent != null)
        {
            var isReduction = timeToAdd < 0;
            textComponent.text = isReduction ? $"{timeToAdd:F1}s" : $"+{timeToAdd:F1}s";
            textComponent.color = isReduction ? Color.red : Color.green;
        }
    
        addedTimeText.transform.DOLocalMoveY(50f, 1f).SetRelative().SetEase(Ease.OutQuad);
        textComponent.DOFade(0, 1f).OnComplete(() => Destroy(addedTimeText));
    }

    private void UpdateTimerText()
    {
        if (_timerText != null)
        {
            var seconds = Mathf.CeilToInt(_remainingTime);
            _timerText.text = seconds.ToString();
        }
    }

    private void AnimateTimerText()
    {
        _timerText?.transform.DOScale(1.3f, 0.2f).SetLoops(2, LoopType.Yoyo);
    }

    private void StartPulsingEffect()
    {
        if (_timerText == null) return;

        _isPulsing = true;
        _timerText.color = Color.red;
        _timerText.transform.DOScale(1.3f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
    }

    private void StopPulsingEffect()
    {
        if (_timerText == null) return;

        _isPulsing = false;
        _timerText.color = Color.white;
        _timerText.transform.localScale = Vector3.one;
        _timerText.transform.DOKill();
    }

    public float GetTimeRemaining()
    {
        return _remainingTime;
    }
    
    private void EndRound()
    {
        StopTimer();
        OnRoundEnd?.Invoke();
    }
}
