using System;
using UnityEngine;

public class AndroidBackClose : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (panelRoot != null)
                panelRoot.SetActive(false);
        }
    }
}