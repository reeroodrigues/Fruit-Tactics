using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Image _timerImage;
    public float _totalTime;
    public float _remainingTime;

    private void Start()
    {
        _remainingTime = _totalTime;
    }

    private void Update()
    {
        if (_remainingTime > 0)
        {
            _remainingTime -= Time.deltaTime;
            _timerImage.fillAmount = _remainingTime / _totalTime;
        }
        else
        {
        }
    }
    
    public void AddTime(float timeToAdd)
    {
        _remainingTime += timeToAdd;
    }
}