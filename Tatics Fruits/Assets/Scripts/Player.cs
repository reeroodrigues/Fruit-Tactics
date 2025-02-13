using UnityEngine;

namespace DefaultNamespace
{
    public class Player : MonoBehaviour
    {
        public int _score;

        public void AddScore(int score)
        {
            _score += score;
            Debug.Log($"Pontos adicionados: {_score}. Total: {score}");
        }
    }
}