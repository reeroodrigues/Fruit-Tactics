using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueLabel : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField, Range(0, 2)] private int decimals = 0;

    private void OnEnable()
    {
        if (!slider || !valueText)
            return;
            
        slider.onValueChanged.AddListener(OnChanged);
        OnChanged(slider.value);
    }

    private void OnDisable()
    {
        if (slider)
            slider.onValueChanged.RemoveListener(OnChanged);
    }

    private void OnChanged(float value)
    {
        var pct = Mathf.Clamp01(value) * 100f;
        valueText.text = pct.ToString("F" + decimals) + "%";
    }
}