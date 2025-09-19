using System.IO;
using UnityEngine;

public static class JsonDataService
{
    private static string PathFor(string fileName)
        => System.IO.Path.Combine(Application.persistentDataPath, fileName);

    public static void Save<T>(string fileName, T data)
    {
        var json = JsonUtility.ToJson(data, prettyPrint:true);
        File.WriteAllText(PathFor(fileName), json);
#if UNITY_EDITOR
        Debug.Log($"[Save] {PathFor(fileName)}\n{json}");
#endif
    }

    public static bool TryLoad<T>(string fileName, out T data) where T : new()
    {
        var path = PathFor(fileName);
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            data = JsonUtility.FromJson<T>(json);
            return true;
        }
        data = new T();
        return false;
    }
}