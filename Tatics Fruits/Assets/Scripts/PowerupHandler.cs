using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class PowerupHandler
{
    [Obsolete("Obsolete")]
    public static void ApplyPower(Card sourceCard, Card targetCard, CardManager cardManager)
    {
        if (targetCard.isProtected) return;
        switch (sourceCard.cardTypeSo.powerEffect)
        {
            case PowerEffectType.DoublePoints:
                ApplyDoublePoints(targetCard);
                Cleanup(sourceCard, cardManager);
                break;

            case PowerEffectType.ExplodeAdjacent:
                ExplodeSingle(targetCard, cardManager);
                Cleanup(sourceCard, cardManager);
                break;

            case PowerEffectType.Freeze:
                ApplyFreeze(targetCard);
                Cleanup(sourceCard, cardManager);
                break;
            
            case PowerEffectType.Protection:
                ApplyProtection(targetCard, sourceCard.cardTypeSo.protectionDuration);
                Cleanup(sourceCard, cardManager);
                break;
            
            case PowerEffectType.Joker:
                ApplyJoker(sourceCard, targetCard);
                sourceCard.ResetCardPosition();
                break;
            
            case PowerEffectType.ClearRow:
                ClearEntireRow(targetCard, cardManager);
                Cleanup(sourceCard, cardManager);
                break;
            
            case PowerEffectType.IncreaseNumber:
                ApplyIncreaseNumber(targetCard, sourceCard.cardTypeSo);
                Cleanup(sourceCard, cardManager);
                break;
            
            case PowerEffectType.BonusPoints:
                ApplyBonusPoints(targetCard);
                Cleanup(sourceCard, cardManager);
                break;
            
            case PowerEffectType.Cleanse:
                ApplyCleanse(cardManager);
                Cleanup(sourceCard, cardManager);
                break;
            
            case PowerEffectType.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var parentHolder = targetCard.transform.parent?.GetComponent<CardHolder>();
        if (parentHolder != null)
        {
            parentHolder.CheckForMatchingCards();
        }

        var layoutGroup = cardManager.GetComponentInChildren<HorizontalLayoutGroup>();
        if (layoutGroup != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        }
    }

    private static void ApplyDoublePoints(Card target)
    {
        target.cardNumber *= 2;

        var face = target.GetComponent<CardFace>();
        if (face != null)
            face.UpdateCardInfo();
    }

    private static void ApplyFreeze(Card target)
    {
        target.isFrozen = true;

        var face = target.GetComponent<CardFace>();
        if (face != null)
        {
            face.UpdateCardInfo();
        }
    }
    
    private static void ApplyProtection(Card target, float duration)
    {
        target.isProtected = true;

        var face = target.GetComponent<CardFace>();
        if (face != null)
        {
            face.UpdateCardInfo();
        }

        target.StartCoroutine(RemoveProtectionAfterDelay(target, duration));
    }
    
    private static void ApplyJoker(Card source, Card target)
    {
        if (target.cardTypeSo.isPowerCard || target.isProtected) return;
        
        source.cardTypeSo = target.cardTypeSo;
        source.cardNumber = target.cardNumber;
        
        var face = source.GetComponent<CardFace>();
        if (face != null)
            face.UpdateCardInfo();
    }
    
    private static void ApplyIncreaseNumber(Card target, CardTypeSo cardTypeSo)
    {
        var increase = Random.Range(cardTypeSo.increaseAmountMin, cardTypeSo.increaseAmountMax + 1);
        target.cardNumber += increase;

        var face = target.GetComponent<CardFace>();
        if (face != null)
            face.UpdateCardInfo();
    }
    
    private static void ApplyBonusPoints(Card target)
    {
        target.hasBonusPoints = true;

        var face = target.GetComponent<CardFace>();
        if (face != null)
        {
            face.ShowBonusIcon();
        }
    }
    
    [Obsolete("Obsolete")]
    private static void ApplyCleanse(CardManager manager)
    {
        var cardsToRemove = new List<GameObject>();

        foreach (var cardObj in manager._cards)
        {
            var card = cardObj.GetComponent<Card>();
            if (card != null && card.cardTypeSo.isPowerCard)
            {
                foreach (var face in Object.FindObjectsOfType<CardFace>())
                {
                    if (face._target == card.gameObject)
                    {
                        Object.Destroy(face.gameObject);
                        break;
                    }
                }

                cardsToRemove.Add(cardObj);
                Object.Destroy(card.transform.parent.gameObject);
            }
        }

        foreach (var cardObj in cardsToRemove)
        {
            manager._cards.Remove(cardObj);
        }
    }
    
    [Obsolete("Obsolete")]
    private static void ClearEntireRow(Card target, CardManager cardManager)
    {
        var parent = target.transform.parent?.parent;
        if (parent == null) return;

        foreach (Transform child in parent)
        {
            var card = child.GetComponentInChildren<Card>();
            if (card != null && !card.cardTypeSo.isPowerCard)
            {
                foreach (var face in Object.FindObjectsOfType<CardFace>())
                {
                    if (face._target == card.gameObject)
                    {
                        Object.Destroy(face.gameObject);
                        break;
                    }
                }

                cardManager._cards.Remove(card.gameObject);
                Object.Destroy(card.transform.parent.gameObject);
            }
        }
    }
    
    [Obsolete("Obsolete")]
    private static void ExplodeSingle(Card target, CardManager manager)
    {
        foreach (var face in Object.FindObjectsOfType<CardFace>())
        {
            if (face._target != target.gameObject) continue;
            Object.Destroy(face.gameObject);
            break;
        }

        manager._cards.Remove(target.gameObject);
        Object.Destroy(target.transform.parent.gameObject);
    }

    private static System.Collections.IEnumerator RemoveProtectionAfterDelay(Card target, float duration)
    {
        yield return new WaitForSeconds(duration);
        target.isProtected = false;

        var face = target.GetComponent<CardFace>();
        if (face != null)
        {
            face.UpdateCardInfo();
        }
    }

    [Obsolete("Obsolete")]
    private static void Cleanup(Card sourceCard, CardManager manager)
    {
        foreach (var face in Object.FindObjectsOfType<CardFace>())
        {
            if (face._target != sourceCard.gameObject) continue;
            Object.Destroy(face.gameObject);
            break;
        }

        manager._cards.Remove(sourceCard.gameObject);
        Object.Destroy(sourceCard.transform.parent.gameObject);
    }
}