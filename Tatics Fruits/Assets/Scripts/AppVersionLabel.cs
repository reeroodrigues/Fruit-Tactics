using TMPro;
using UnityEngine;

public class AppVersionLabel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private string prefix = "v";

    private void Awake()
    {
        if (label)
            label.text = $"{prefix}{Application.version}";
    }
}