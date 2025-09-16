using System;
using DG.Tweening;
using UnityEngine;

public class Wiggle : MonoBehaviour
{
    [SerializeField] private float angle = 18f;
    [SerializeField] private float duration = 0.35f;
    [SerializeField] private float everySeconds = 6f;

    private void OnEnable()
    {
        InvokeRepeating(nameof(DoWiggle), 1f, everySeconds);
    }

    private void OnDisable()
    {
        CancelInvoke();
        transform.DOKill();
    }

    private void DoWiggle()
    {
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOLocalRotate(new Vector3(0, 0, angle), duration / 2f))
            .Append(transform.DOLocalRotate(new Vector3(0, 0, -angle), duration))
            .Append(transform.DOLocalRotate(Vector3.zero, duration / 2f));
    }
}