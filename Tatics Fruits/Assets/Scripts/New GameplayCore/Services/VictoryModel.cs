namespace New_GameplayCore.Services
{
    [System.Serializable]
    public struct VictoryModel
    {
        public string LevelId;
        public int TotalScore;
        public int TargetScore;
        public int StarsEarned;
        public int BestBefore;
        public bool NewRecord;
        public int TimeLeftSeconds;
    }
}