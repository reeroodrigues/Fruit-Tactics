using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

[DefaultExecutionOrder(-100)]
public class Localizer : MonoBehaviour
{
    public static Localizer Instance { get; private set; }
    private readonly Dictionary<string, string> _table = new();

    public string CurrentLanguage { get; private set; } = "pt-BR";
    
    public delegate void LanguageChanged();
    public event LanguageChanged OnLanguageChanged;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance == null && FindObjectOfType<Localizer>() == null)
        {
            var go = new GameObject("Localizer");
            go.AddComponent<Localizer>();
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);

        var cfg = SettingsRepository.Get();
        SetLanguage(cfg.language, save: false);
    }

    public void SetLanguage(string language, bool save = true)
    {
        CurrentLanguage = language;
        LoadTable(language);
        if (save)
        {
            var s = SettingsRepository.Get();
            s.language = language;
            SettingsRepository.Save(s);
        }
        
        OnLanguageChanged?.Invoke();
    }

    private void LoadTable(string language)
    {
        _table.Clear();
        
        string path = Path.Combine(Application.streamingAssetsPath, "i18n", $"{language}.json");
        
#if UNITY_ANDROID && !UNITY_EDITOR
        var www = new UnityEngine.WWW(path);
        while (!www.isDone) { }
        if(!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(www.error);
            return;
        }
        var json = www.text;
#else
        if (!File.Exists(path))
        {
            Debug.LogError($"Missing locale + {path}");
            return;
        }
        var json = File.ReadAllText(path); 
#endif
        var wrapper = JsonUtility.FromJson<LocalizationWrapper>(Wrap(json));
        if (wrapper != null && wrapper.entries != null)
        {
            foreach (var e in wrapper.entries)
            {
                _table[e.key] = e.value;
            }
        }
    }
    
    [System.Serializable] private class LocalizationEntry { public string key; public string value; }
    [System.Serializable] private class LocalizationWrapper { public LocalizationEntry[] entries; }

    private string Wrap(string raw)
    {
        // raw é { "a":"b", "c":"d" }, transformamos para { "entries":[{"key":"a","value":"b"}, ...] }
        var dict = MiniJson.Deserialize(raw) as Dictionary<string, object>;
        var list = new List<LocalizationEntry>();
        foreach (var kv in dict) list.Add(new LocalizationEntry { key = kv.Key, value = kv.Value.ToString() });
        return JsonUtility.ToJson(new LocalizationWrapper { entries = list.ToArray() }, false);
    }

    public string Tr(string key, string fallback = "")
        => _table.TryGetValue(key, out var v) ? v : (string.IsNullOrEmpty(fallback) ? key : fallback);
}