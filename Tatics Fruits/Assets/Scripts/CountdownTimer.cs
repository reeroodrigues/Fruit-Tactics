using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI _countdownText;
    public float _timeBetweenNumbers = 1f;

    async void Start()
    {
        await StartCountDown();
    }

    private async Task StartCountDown()
    {
        var counter = 3;
        while (counter > 0)
        {
            _countdownText.text = counter.ToString();
            
            _countdownText.transform.localScale = Vector3.zero;
            _countdownText.transform.DOScale(1f, 0.8f).SetEase(Ease.OutBack);
            
            await Task.Delay(System.TimeSpan.FromSeconds(_timeBetweenNumbers));
            counter--;
        }

        _countdownText.text = "GO!";
        
        _countdownText.transform.localScale = Vector3.zero;
        _countdownText.transform.DOScale(1f, 0.8f).SetEase(Ease.OutBack);
        await Task.Delay(System.TimeSpan.FromSeconds(1f));
        gameObject.SetActive(false);
    }
}
