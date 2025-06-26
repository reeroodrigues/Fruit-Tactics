using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

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