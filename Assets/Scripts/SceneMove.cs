using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMove : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("SceneCenterPointer2");
    }
    
    public void EndGame()
    {
        Application.Quit();
    }

    public void TitleBack()
    {
        SceneManager.LoadScene("TitleCenterpointer2");
    }
}
