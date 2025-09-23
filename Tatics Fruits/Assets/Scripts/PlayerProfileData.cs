using System;
using System.Collections.Generic;

#region Missões diárias (igual ao seu)
[Serializable]
public class DailyMissionState
{
    public string missionId;
    public string description;
    public int progress;
    public int target;
    public int rewardGold;
    public bool completed;
    public bool claimed;
}
#endregion

#region Login diário 7/7 (novo)
[Serializable]
public class DailyLoginData
{
    public string lastClaimDayKey = "";
    
    public int cycleIndex = 0;
    
    public List<bool> claimed = new List<bool> { false,false,false,false,false,false,false };
    
    public List<int> rewards = new List<int> { 20, 30, 40, 60, 80, 100, 200 };
}
#endregion

[Serializable]
public class DailySystemData
{
    public string dayKey;
    
    public List<DailyMissionState> missions = new List<DailyMissionState>();
    
    public DailyLoginData Login = new DailyLoginData();
    
    [Obsolete("Use Daily.Login em vez de lastLoginRewardDayKey.")]
    public string lastLoginRewardDayKey;

    [Obsolete("Use Daily.Login.rewards em vez de loginRewardGold.")]
    public int loginRewardGold = 50;
}

[Serializable]
public class PlayerProfileData
{
    public string PlayerName = "Jogador";
    public int AvatarIndex = 0;
    public int Gold = 0;
    public List<string> OwnedCards = new List<string>();
    public List<string> EquippedDeck = new List<string>();
    
    public DailySystemData Daily = new DailySystemData();
}
