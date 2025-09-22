using UnityEngine;

public enum MissionEventType
{
    WinLevel
}

[CreateAssetMenu(fileName = "DailyMissions", menuName = "Game/DailyMissions")]
public class DailyMissionSo : ScriptableObject
{
    public string id; //"win_level_1", etc...
    [TextArea] public string descriptionTemplate; //"Vença o nível {level}"
    public MissionEventType eventType =  MissionEventType.WinLevel;
    public int target = 1; //ex: 1 vitória
    public int rewardGold = 100; //quanto dá de gold
    public int levelParam = 1; //para WinLeve, qual nível? (0= qualquer)
}

