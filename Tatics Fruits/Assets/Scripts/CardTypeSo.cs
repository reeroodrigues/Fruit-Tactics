using UnityEngine;

public enum PowerUpEffect
{
    time_plus,        // +N segundos imediatos
    time_freeze,      // congela o timer por Xs (jogador ainda joga)
    wildcard,         // próxima carta combina com qualquer fruta (1 uso)
    undo_miss,        // ignora a próxima penalidade por erro (1 uso)
    combo_boost,      // por Xs, multiplicador de acertos (ex.: x2)
    streak_guard,     // próximo erro não quebra a streak (1 uso)
    shuffle_soft,     // embaralha % do tabuleiro
    shuffle_full,     // embaralha tudo
    row_clear,        // remove 1 par e embaralha o resto da mesma linha
    time_on_match,    // por Xs, cada par certo dá +N s
    chain_extend,     // enquanto acertar em sequência, +N s por par; zera no erro
    last_chance,      // ao chegar a 0s, concede +N s (1x por fase)
    gold_rush         // por 1 partida, cada par dá +N gold
}


[CreateAssetMenu(menuName = "Card/CardType")]
public class CardTypeSo : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite cardIcon;
    public int maxCardNumber;
    public int setAmount;
    public int priceGold;
    public string rarity;

    public bool isPowerCard;
    public PowerUpEffect powerEffect;

    [Header("Power Effect Settings")]
    public float protectionDuration = 5f;
    public float freezeTimeDuration = 5f; 
    public int increaseAmountMin = 1;
    public int increaseAmountMax = 5;
    
}