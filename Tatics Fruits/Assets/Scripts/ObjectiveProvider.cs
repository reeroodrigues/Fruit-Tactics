using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "ObjectiveProvider", menuName = "CardGame/ObjectiveProvider")]
    public class ObjectiveProvider : ScriptableObject
    {
        [System.Serializable]
        public struct Objective
        {
            public int _points;
            public int _time;
        }
        
        [SerializeField] private Objective[] _objectives;

        public Objective GetRandomObjectives()
        {
            return _objectives[Random.Range(0, _objectives.Length)];
        }
    }
}