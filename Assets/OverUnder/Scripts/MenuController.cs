using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    public void GoToGame()
    {
        SceneManager.LoadScene("OverUnderGame");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void GoToHelp()
    {
        SceneManager.LoadScene("HowToPlay");
    }
}
