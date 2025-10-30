namespace New_GameplayCore.Services
{
    [System.Serializable]
    public struct VictoryModel
    {
        public string levelId;
        public int totalScore;
        public int starsEarned;
        public int bestBefore;
        public int targetScore;
        public int timeLeftSeconds;
        public int levelIndex;
        public bool newRecord;
        public bool canGoNext;
    }
}