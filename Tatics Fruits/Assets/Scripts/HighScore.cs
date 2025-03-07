using UnityEngine;
using System.IO;

namespace DefaultNamespace
{
    public class HighScore : MonoBehaviour
    {
        private string FilePath => Application.persistentDataPath + "/highscore.json";

        public int GetHighScore()
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                return JsonUtility.FromJson<ScoreData>(json).score;
            }
            return 0;
        }

        public void TrySetHighScore(int newScore)
        {
            int currentHighScore = GetHighScore();
            if (newScore > currentHighScore)
            {
                string json = JsonUtility.ToJson(new ScoreData { score = newScore });
                File.WriteAllText(FilePath, json);
                Debug.Log($"Novo High Score Salvo: {newScore}");
            }
        }

        [System.Serializable]
        private class ScoreData
        {
            public int score;
        }
    }
}