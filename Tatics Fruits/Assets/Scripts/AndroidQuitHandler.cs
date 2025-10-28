using UnityEngine;
using UnityEngine.SceneManagement;

public class AndroidQuitHandler : MonoBehaviour
{
    private static AndroidQuitHandler _instance;
    public string mainMenuSceneName = "MainMenu";

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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name == mainMenuSceneName)
            {
                QuitGame();
            }
            else
            {
                SceneManager.LoadScene(mainMenuSceneName);
                
                Time.timeScale = 1f; 
            }
        }
    }
    
    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}