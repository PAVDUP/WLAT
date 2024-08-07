using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour
{
    void Awake()
    {
        
    }

    public void GameStart()
    {
        SceneManager.LoadScene("GameStartTest");
    }
    
    public void GameStartScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void GameOverScene()
    {
        SceneManager.LoadScene("GameOverTest");
    }

}
