using System;
using System.Collections.Generic;

#region Missões diárias (igual ao seu)
[Serializable]
public class DailyMissionState
{
    public string missionId;
    public string description;   // snapshot do texto do dia
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
    // chave do último dia coletado (yyyyMMdd) para garantir 1x por dia
    public string lastClaimDayKey = "";

    // índice do ciclo atual (0..6) -> Dia 1..Dia 7
    public int cycleIndex = 0;

    // estados coletados no ciclo corrente (zera ao completar o Dia 7)
    public List<bool> claimed = new List<bool> { false,false,false,false,false,false,false };

    // recompensas por dia (7 entradas; a 7ª costuma ser especial)
    public List<int> rewards = new List<int> { 20, 30, 40, 60, 80, 100, 200 };
}
#endregion

[Serializable]
public class DailySystemData
{
    // chave do dia em que as 3 missões foram geradas (ex.: "20250922")
    public string dayKey;

    // missões do dia (sempre 3)
    public List<DailyMissionState> missions = new List<DailyMissionState>();

    // ===== NOVO: trilha de login 7/7 =====
    public DailyLoginData Login = new DailyLoginData();

    // ===== LEGADO (mantido temporariamente p/ migração) =====
    [Obsolete("Use Daily.Login em vez de lastLoginRewardDayKey.")]
    public string lastLoginRewardDayKey; // dia da última coleta (antigo)

    [Obsolete("Use Daily.Login.rewards em vez de loginRewardGold.")]
    public int loginRewardGold = 50;     // valor único (antigo)
}

[Serializable]
public class PlayerProfileData
{
    // Perfil / economia / coleção
    public string PlayerName = "Jogador";
    public int AvatarIndex = 0;
    public int Gold = 0;
    public List<string> OwnedCards = new List<string>();
    public List<string> EquippedDeck = new List<string>();

    // Diário (missões + login 7/7)
    public DailySystemData Daily = new DailySystemData();
}
