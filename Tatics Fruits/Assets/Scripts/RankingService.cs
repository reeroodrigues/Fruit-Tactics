using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class RunRecord
{
    public int level;
    public int score;
    public float timeSeconds;   // tempo gasto na run
    public bool completed;      // completou o objetivo do level?
    public string dateIso;      // ISO8601 para debug/ordenação
}

[Serializable]
public class RankingData
{
    public int maxLevel;
    public int maxScore;
    public float bestTimeSeconds = -1f; // -1 = inexistente
    public List<RunRecord> topRuns = new List<RunRecord>();
}

public static class RankingService
{
    private static readonly string FilePath =
        Path.Combine(Application.persistentDataPath, "ranking.json");

    private static RankingData _cache;

    public static RankingData Load()
    {
        if (_cache != null) return _cache;
        if (!File.Exists(FilePath))
        {
            _cache = new RankingData();
            Save();
            return _cache;
        }

        try
        {
            var json = File.ReadAllText(FilePath);
            _cache = JsonUtility.FromJson<RankingData>(json) ?? new RankingData();
        }
        catch
        {
            _cache = new RankingData();
        }
        return _cache;
    }

    private static void Save()
    {
        try
        {
            var dir = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            var json = JsonUtility.ToJson(_cache, true);
            File.WriteAllText(FilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[RankingService] Save error: {e}");
        }
    }

    /// <summary>
    /// Registre o fim de uma partida.
    /// levelReached: último level alcançado (ou o level da run se for por-level).
    /// score: pontuação da run.
    /// timeSeconds: tempo gasto no level/run.
    /// completed: true se o objetivo do level foi concluído.
    /// </summary>
    public static void SubmitRun(int levelReached, int score, float timeSeconds, bool completed)
    {
        var data = Load();

        // Atualiza agregados
        if (levelReached > data.maxLevel) data.maxLevel = levelReached;
        if (score > data.maxScore) data.maxScore = score;

        if (completed)
        {
            if (data.bestTimeSeconds < 0f || timeSeconds < data.bestTimeSeconds)
                data.bestTimeSeconds = Mathf.Max(0f, timeSeconds);
        }

        // Guarda a run para o ranking (top N)
        var rec = new RunRecord
        {
            level = Mathf.Max(1, levelReached),
            score = Mathf.Max(0, score),
            timeSeconds = Mathf.Max(0f, timeSeconds),
            completed = completed,
            dateIso = DateTime.UtcNow.ToString("o")
        };
        data.topRuns.Add(rec);

        // Ordenação: Level desc, Score desc, Tempo asc
        data.topRuns.Sort((a, b) =>
        {
            int byLevel = b.level.CompareTo(a.level);
            if (byLevel != 0) return byLevel;
            int byScore = b.score.CompareTo(a.score);
            if (byScore != 0) return byScore;
            return a.timeSeconds.CompareTo(b.timeSeconds);
        });

        // Limita tamanho (ajuste se quiser)
        const int MAX = 50;
        if (data.topRuns.Count > MAX)
            data.topRuns.RemoveRange(MAX, data.topRuns.Count - MAX);

        Save();
    }

    public static void ResetAll()
    {
        _cache = new RankingData();
        Save();
    }

    // Helpers
    public static string FormatTime(float seconds)
    {
        if (seconds < 0f) return "--:--";
        int m = Mathf.FloorToInt(seconds / 60f);
        int s = Mathf.FloorToInt(seconds % 60f);
        return $"{m:00}:{s:00}";
    }
}
