using System;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    public enum MissionType
    {
        ScorePoints,
        PlayXMatches,
        UseSpecialCard
    }

    [Serializable]
    public class Mission
    {
        public string _missionName;
        public string _description;
        public MissionType _type;
        public int _goal;
        public int _currentProgress;
        public bool _isCompleted;
        public int _rewardAmount;
        public string _rewardType;

        public Action<Mission> OnMissionCompleted;

        public Mission(string name, string description, MissionType missionType, int goal, int reward,
            string rewardType)
        {
            _missionName = name;
            _description = description;
            _type = missionType;
            _goal = goal;
            _rewardAmount = reward;
            _rewardType = rewardType;
            _currentProgress = 0;
            _isCompleted = false;
        }

        public void UpdateProgress(int amount)
        {
            if (_isCompleted ) return;

            _currentProgress += amount;

            if (_currentProgress >= _goal)
            {
                _isCompleted = true;
                OnMissionCompleted?.Invoke(this);
            }
        }
    }
    
    
}