using System;
using New_GameplayCore;

public class TimeManager : ITimeManager
{
    private int _timeLeft;
    private float _acc;

    public int TimeLeftSeconds => _timeLeft;
    public event Action<int> OnTimeChanged;

    public TimeManager(int startSeconds) => _timeLeft = startSeconds;

    public void Add(int seconds)
    {
        _timeLeft += seconds;
        OnTimeChanged?.Invoke(_timeLeft);
    }

    public bool CanPay(int seconds) => _timeLeft >= seconds;

    public bool TryPay(int seconds)
    {
        if (!CanPay(seconds)) return false;
        _timeLeft -= seconds;
        if (_timeLeft < 0) _timeLeft = 0;
        OnTimeChanged?.Invoke(_timeLeft);
        return true;
    }

    public void Tick(float deltaTime)
    {
        if (_timeLeft <= 0) return;

        _acc += deltaTime;
        while (_acc >= 1f && _timeLeft > 0)
        {
            _timeLeft -= 1;
            _acc -= 1f;
            OnTimeChanged?.Invoke(_timeLeft);
        }
    }
}