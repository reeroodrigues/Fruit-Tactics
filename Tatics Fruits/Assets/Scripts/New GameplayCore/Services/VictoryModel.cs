namespace New_GameplayCore.Services
{
    [System.Serializable]
    public struct VictoryModel
    {
        public string levelId;
        public int totalScore;
        public int targetScore;
        public int starsEarned;
        public int bestBefore;
        public bool newRecord;
        public int timeLeftSeconds;
    }
}