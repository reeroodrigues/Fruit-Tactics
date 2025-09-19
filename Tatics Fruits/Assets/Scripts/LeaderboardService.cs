using System.IO;
using System.Linq;
using UnityEngine;

public class LeaderboardService
{
    private const string FileName = "leaderboard.json";
    
    private static string PathFor() => Path.Combine(Application.streamingAssetsPath, FileName);

    public static LeaderboardPayLoad LoadLocal()
    {
        string path = PathFor();
#if UNITY_ANDROID && !UNITY_EDITOR
        var www = new WWW(path);
        while (!www.isDone){}
        if (!string.IsNullOrEmpty(www.error))
        {
        Debug.Log(www.error);
        return new LeaderboardPayLoad();
        }
        var json = www.text;
#else
        if (!File.Exists(path))
        {
            Debug.LogWarning("Missing leaderboard:" + path);
            return new LeaderboardPayLoad();
        }
        var json = File.ReadAllText(path);
#endif
        var payload = JsonUtility.FromJson<LeaderboardPayLoad>(json);

        void SortAndRank(ref LeaderboardEntry[] arr)
        {
            if (arr == null)
            {
                arr = new LeaderboardEntry[0];
                return;
            }
            var sorted = arr.OrderByDescending(e=> e.score).ThenBy(e => e.timeSeconds).ToList();
            for (int i = 0; i < sorted.Count; i++)
            {
                sorted[i].rank = i+ 1;
            }
            arr = sorted.ToArray();
        }

        SortAndRank(ref payload.daily);
        SortAndRank(ref payload.weekly);
        SortAndRank(ref payload.allTime);
        return payload;
    }
}