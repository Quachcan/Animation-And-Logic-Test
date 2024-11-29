using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void LoadScene(string sceneName)
    {
        //StartCoroutine(LoadSceneAsync(sceneName));
    }

    public void ReloadCurrentScene()
    {
        //string currentSceneName = SceneManager.GetAtiveScene().name;
        //LoadScene(currentSceneName);
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
