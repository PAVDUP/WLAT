using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour
{
    RhythmGame rhythm;
    public bool isStart;

    void Awake()
    {
        rhythm = GetComponent<RhythmGame>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("MainScene");
        }       
    }

    public void GameOverScene()
    {
        SceneManager.LoadScene("GameOverTest");

        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("MainScene");
        }
    }

}
