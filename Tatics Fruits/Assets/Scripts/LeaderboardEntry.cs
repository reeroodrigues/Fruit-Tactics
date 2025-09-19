
[System.Serializable]
public class LeaderboardEntry
{
    public int rank;
    public string playerId;
    public string playerName;
    public int score;
    public float timeSeconds;
}

[System.Serializable]
public class LeaderboardPayLoad
{
    public LeaderboardEntry[] daily;
    public LeaderboardEntry[] weekly;
    public LeaderboardEntry[] allTime;
}