using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Graphic))]
public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string key;
    [TextArea] public string fallback;

    private TextMeshProUGUI tmp;
    private TextMeshProUGUI ugui;
    private bool subscribed;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        ugui = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        StartCoroutine(EnsureSubscribedThenRefresh());
    }

    private void OnDisable()
    {
        TryUnsubscribe();
    }

    private IEnumerator EnsureSubscribedThenRefresh()
    {
        // espera até o Localizer existir (normalmente instantâneo com o Bootstrap)
        while (Localizer.Instance == null) yield return null;

        TrySubscribe();
        Refresh();
    }

    private void TrySubscribe()
    {
        if (subscribed || Localizer.Instance == null) return;
        Localizer.Instance.OnLanguageChanged += Refresh;
        subscribed = true;
    }

    private void TryUnsubscribe()
    {
        if (!subscribed || Localizer.Instance == null) return;
        Localizer.Instance.OnLanguageChanged -= Refresh;
        subscribed = false;
    }

    public void Refresh()
    {
        if (Localizer.Instance == null) return;
        var txt = Localizer.Instance.Tr(key, fallback);
        if (tmp) tmp.text = txt;
        else if (ugui) ugui.text = txt;
    }
}