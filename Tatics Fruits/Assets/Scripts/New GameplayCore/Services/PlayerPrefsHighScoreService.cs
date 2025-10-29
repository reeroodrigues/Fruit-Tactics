using System;
using UnityEngine;

namespace New_GameplayCore.Services
{
    public class PlayerPrefsHighScoreService : IHighScoreService
    {
        public event Action<string, int>  OnHighScoreChanged;
        
        private static string Key(string levelId) => $"highscore_{levelId}";
        public int GetBest(string levelId) => PlayerPrefs.GetInt(Key(levelId), 0);

        public bool TryReportScore(string levelId, int score)
        {
            var best = GetBest(levelId);
            if (score > best)
            {
                PlayerPrefs.SetInt(Key(levelId), score);
                PlayerPrefs.Save();
                OnHighScoreChanged?.Invoke(levelId, best);
                return true;
            }
            return false;
        }
    }
}