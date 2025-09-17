using System.Collections.Generic;
using UnityEngine;

namespace Tutorial
{
    public class TutorialCardSpawner : MonoBehaviour
    {
        [Header("Dependencies")]
        public CardManager cardManager;

        [Header("Cards for the tutorial")]
        public List<CardTypeSo> tutorialCards;

        private void Start()
        {
            if (cardManager == null)
            {
                Debug.LogWarning("CardManager not found");
                return;
            }
            
            cardManager._isTutorialMode = true;
            
            cardManager._cards.Clear();

            cardManager.AddSpecificCard(tutorialCards);
        }
    }
}
