using System;
using UnityEngine;

public class AndroidQuitHandler : MonoBehaviour
{
    private static AndroidQuitHandler _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Botar voltar pressionando - Encerrando o jogo");
            Application.Quit();
        }
#endif
    }
}