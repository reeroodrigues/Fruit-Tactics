using UnityEngine;

namespace DefaultNamespace
{
    public class ScoreManager : MonoBehaviour
    {
        private int _score;

        public void AddScore(int value)
        {
            _score += value;
        }

        public int GetScore()
        {
            return _score;
        }
    }
}