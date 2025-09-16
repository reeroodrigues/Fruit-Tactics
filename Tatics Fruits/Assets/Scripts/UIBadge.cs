using UnityEngine;

public class UIBadge : MonoBehaviour
{
    [SerializeField] private GameObject badge;

    public void Show(bool on)
    {
        if (badge)
            badge.SetActive(on);
    }
}