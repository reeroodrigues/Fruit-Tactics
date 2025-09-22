using System;
using System.Collections.Generic;

[Serializable]
public class PlayerProfileData
{
    // Perfil
    public string PlayerName = "Jogador";
    public int AvatarIndex = 0;

    // Economia
    public int Gold = 0;

    // Coleção e Deck
    public List<string> OwnedCards = new List<string>();   // ids das cartas possuídas
    public List<string> EquippedDeck = new List<string>(); // ids equipados (ex.: limite 5)
}