using UnityEngine;

[CreateAssetMenu(menuName = "Game/Power Up", fileName = "PowerUp_")]
public class PowerUpCardSO : ScriptableObject
{
    [Header("Identidade")]
    public PowerUpEffect effect;            // define o ID lógico
    public string displayName;              // ex.: "Relógio+"
    public Sprite icon;
    public string rarity = "Common";        // "Common", "Uncommon", "Rare", "Epic"
    public int priceGold = 100;

    // Use isto como ID estável para salvar no JSON (OwnedCards)
    public string Id => effect.ToString();

    [Header("Parâmetros Comuns")]
    [Tooltip("Duração em segundos para efeitos temporais (ex.: time_freeze, combo_boost, time_on_match).")]
    [Min(0f)] public float duration = 0f;

    [Tooltip("Quantidade principal (ex.: +segundos imediatos no time_plus; +segundos do last_chance).")]
    public int amount = 0;

    [Tooltip("Multiplicador (ex.: combo x2).")]
    [Min(1)] public int multiplier = 1;

    [Tooltip("Percentual para embaralhar (0..1) no shuffle_soft (ex.: 0.25).")]
    [Range(0f, 1f)] public float shufflePercent = 0.25f;

    [Tooltip("Segundos extras por par certo quando ativo (time_on_match, chain_extend).")]
    public int extraSecondsPerMatch = 0;

    [Tooltip("Bônus de gold por par (gold_rush).")]
    public int bonusGoldPerPair = 0;

    [Tooltip("Usos para efeitos 'de um uso' (wildcard, undo_miss, streak_guard).")]
    [Min(0)] public int uses = 1;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Sugestões rápidas de defaults por efeito
        switch (effect)
        {
            case PowerUpEffect.time_plus:
                if (amount <= 0) amount = 10; // +10s
                break;
            case PowerUpEffect.time_freeze:
                if (duration <= 0) duration = 3f;
                break;
            case PowerUpEffect.wildcard:
                if (uses <= 0) uses = 1;
                break;
            case PowerUpEffect.undo_miss:
                if (uses <= 0) uses = 1;
                break;
            case PowerUpEffect.combo_boost:
                if (duration <= 0) duration = 10f;
                if (multiplier < 2) multiplier = 2;
                break;
            case PowerUpEffect.streak_guard:
                if (uses <= 0) uses = 1;
                break;
            case PowerUpEffect.shuffle_soft:
                if (shufflePercent <= 0f) shufflePercent = 0.25f;
                break;
            case PowerUpEffect.shuffle_full:
                // sem params
                break;
            case PowerUpEffect.row_clear:
                // sem params (remove 1 par, embaralha linha)
                break;
            case PowerUpEffect.time_on_match:
                if (duration <= 0) duration = 15f;
                if (extraSecondsPerMatch <= 0) extraSecondsPerMatch = 2;
                break;
            case PowerUpEffect.chain_extend:
                if (extraSecondsPerMatch <= 0) extraSecondsPerMatch = 1;
                break;
            case PowerUpEffect.last_chance:
                if (amount <= 0) amount = 5; // +5s quando 0s
                break;
            case PowerUpEffect.gold_rush:
                if (bonusGoldPerPair <= 0) bonusGoldPerPair = 1;
                break;
        }
    }
#endif
}
