using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DailyMissionsController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private PlayerProfileController profile;
    [SerializeField] private List<DailyMissionSo> missionPool;
    [SerializeField, Range(1, 5)] private int missionsPerDay = 3;
    [SerializeField] private bool useLocalTime = true;

    // Badge do menu
    public event Action<bool> OnAttentionChanged; // true = tem algo para coletar

    // UI de login diário (grade 7/7)
    public event Action OnDailyLoginChanged;

    private string TodayKey =>
        (useLocalTime ? DateTime.Now : DateTime.UtcNow).ToString("yyyyMMdd");

    // ------- TIPOS: Login diário -------
    public struct DailyLoginDayInfo
    {
        public int index;      // 0..6 (Dia 1..7)
        public int reward;     // gold do dia
        public bool claimed;   // já coletado no ciclo?
        public bool claimable; // pode coletar hoje?
    }

    private void Awake()
    {
        // Garante estruturas
        EnsureLoginInitialized();
        MigrateLegacyLoginIfNeeded();

        // Gera missões do dia (3)
        EnsureDayGenerated();

        FireAttention();
    }

    // =========================
    // INIT / MIGRAÇÃO (LOGIN)
    // =========================
    private void EnsureLoginInitialized()
    {
        if (profile.Data == null) return;
        if (profile.Data.Daily == null)
            profile.Data.Daily = new DailySystemData();

        var l = profile.Data.Daily.Login;
        if (l == null)
        {
            profile.Data.Daily.Login = new DailyLoginData();
            l = profile.Data.Daily.Login;
        }

        if (l.rewards == null || l.rewards.Count != 7)
            l.rewards = new List<int> { 20, 30, 40, 60, 80, 100, 200 };

        if (l.claimed == null || l.claimed.Count != 7)
            l.claimed = new List<bool> { false, false, false, false, false, false, false };

        l.cycleIndex = Mathf.Clamp(l.cycleIndex, 0, 6);
    }

    private void MigrateLegacyLoginIfNeeded()
    {
        // Copia dados antigos (se existirem) para o novo formato
        var daily = profile.Data.Daily;
        if (daily == null) return;

        var l = daily.Login;
        if (l == null) { EnsureLoginInitialized(); l = daily.Login; }

        // Se havia um lastLoginRewardDayKey antigo e ainda não temos lastClaimDayKey, migra
        if (!string.IsNullOrEmpty(daily.lastLoginRewardDayKey) &&
            string.IsNullOrEmpty(l.lastClaimDayKey))
        {
            l.lastClaimDayKey = daily.lastLoginRewardDayKey;
        }

        // Se havia um loginRewardGold único antigo e quiser reaproveitar como Dia 1
        if (daily.loginRewardGold > 0 && l.rewards != null && l.rewards.Count == 7)
        {
            // só ajusta se ainda está no default
            if (l.rewards[0] == 20) l.rewards[0] = daily.loginRewardGold;
        }

        profile.SaveProfile();
    }

    // ==================
    // RESET DIÁRIO (3)
    // ==================
    public void EnsureDayGenerated()
    {
        var daily = profile.Data.Daily;
        if (daily == null)
        {
            profile.Data.Daily = new DailySystemData();
            daily = profile.Data.Daily;
        }

        if (daily.dayKey == TodayKey && daily.missions != null && daily.missions.Count == missionsPerDay)
            return;

        daily.dayKey = TodayKey;
        daily.missions = new List<DailyMissionState>();

        // escolha aleatória da pool
        var pool = missionPool.Where(m => m != null).OrderBy(_ => Random.value).ToList();
        for (int i = 0; i < Mathf.Min(missionsPerDay, pool.Count); i++)
        {
            var def = pool[i];
            var desc = def.descriptionTemplate;
            if (def.eventType == MissionEventType.WinLevel && def.levelParam > 0)
                desc = desc.Replace("{level}", def.levelParam.ToString());

            daily.missions.Add(new DailyMissionState
            {
                missionId = def.id,
                description = desc,
                progress = 0,
                target = Mathf.Max(1, def.target),
                rewardGold = Mathf.Max(0, def.rewardGold),
                completed = false,
                claimed = false
            });
        }

        profile.SaveProfile();
    }

    // ==========================
    // LOGIN DIÁRIO (ciclo 7/7)
    // ==========================
    public List<DailyLoginDayInfo> GetLoginDays()
    {
        EnsureLoginInitialized();
        var l = profile.Data.Daily.Login;

        var list = new List<DailyLoginDayInfo>(7);
        for (int i = 0; i < 7; i++)
        {
            bool claimable = (i == l.cycleIndex) && l.lastClaimDayKey != TodayKey && !l.claimed[i];
            list.Add(new DailyLoginDayInfo
            {
                index = i,
                reward = l.rewards[i],
                claimed = l.claimed[i],
                claimable = claimable
            });
        }
        return list;
    }

    public bool TryClaimDailyLoginDay(int index)
    {
        EnsureLoginInitialized();
        var l = profile.Data.Daily.Login;

        if (index != l.cycleIndex) return false;         // não é o dia da vez
        if (l.claimed[index]) return false;              // já coletado
        if (l.lastClaimDayKey == TodayKey) return false; // já coletou hoje

        // concede recompensa
        profile.AddGoldAndSave(l.rewards[index]);

        // marca estado
        l.claimed[index] = true;
        l.lastClaimDayKey = TodayKey;

        // avança ciclo
        l.cycleIndex++;
        if (l.cycleIndex >= 7)
        {
            l.cycleIndex = 0;
            for (int i = 0; i < 7; i++) l.claimed[i] = false; // reseta para próximo ciclo
        }

        profile.SaveProfile();
        OnDailyLoginChanged?.Invoke();
        FireAttention();
        return true;
    }

    // Compatibilidade com UI antiga (opcional)
    public bool IsDailyLoginAvailable()
    {
        EnsureLoginInitialized();
        var l = profile.Data.Daily.Login;
        return l.lastClaimDayKey != TodayKey && !l.claimed[l.cycleIndex];
    }

    public bool TryClaimDailyLogin()
    {
        EnsureLoginInitialized();
        var l = profile.Data.Daily.Login;
        return TryClaimDailyLoginDay(l.cycleIndex);
    }

    // =========================
    // MISSÕES: Progresso/Claim
    // =========================
    DailyMissionSo FindDef(string missionId) =>
        missionPool.FirstOrDefault(m => m && m.id == missionId);

    public IReadOnlyList<DailyMissionState> GetMissions() => profile.Data.Daily.missions;

    public bool TryClaimMission(string missionId)
    {
        var st = profile.Data.Daily.missions.FirstOrDefault(m => m.missionId == missionId);
        if (st == null || !st.completed || st.claimed)
            return false;

        profile.AddGoldAndSave(st.rewardGold);
        st.claimed = true;
        profile.SaveProfile();
        FireAttention();
        return true;
    }

    // Reportadores (chamados do gameplay)
    public void ReportWinLevel(int level)
    {
        var list = profile.Data.Daily.missions;
        if (list == null) return;

        foreach (var st in list)
        {
            var def = FindDef(st.missionId);
            if (def == null || def.eventType != MissionEventType.WinLevel)
                continue;

            // se levelParam=0 vale qualquer nível; se >0 tem que bater
            if (def.levelParam > 0 && def.levelParam != level) continue;
            if (st.completed) continue;

            st.progress = Mathf.Min(st.target, st.progress + 1);
            if (st.progress >= st.target) st.completed = true;
        }

        profile.SaveProfile();
        FireAttention();
        // TODO: evento OnMissionCompleted para UI in-game (toast)
    }

    // =========================
    // Badge no botão do menu
    // =========================
    public bool HasAnyClaimAvailable()
    {
        bool anyMission = profile.Data.Daily.missions.Any(m => m.completed && !m.claimed);
        bool login = IsDailyLoginAvailable();
        return anyMission || login;
    }

    private void FireAttention()
    {
        OnAttentionChanged?.Invoke(HasAnyClaimAvailable());
    }
}
