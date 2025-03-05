using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Image _timerImage;
    public TextMeshProUGUI _timerText;
    public float _totalTime;
    public float _remainingTime;

    private bool _isPulsing = false;

    private void Start()
    {
        _remainingTime = _totalTime;
        UpdateTimerText();
    }

    private void Update()
    {
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
        else if (_isPulsing)
        {
            StopPulsingEffect();
        }
    }
    
    public void AddTime(float timeToAdd)
    {
        _remainingTime += timeToAdd;
        _remainingTime = Mathf.Clamp(_remainingTime, 0, _totalTime);
        AnimateTimerText();
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
        if (_timerText != null)
        {
            _timerText.transform.DOScale(1.3f, 0.2f).SetLoops(2, LoopType.Yoyo);
            UpdateTimerText();
        }
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
}