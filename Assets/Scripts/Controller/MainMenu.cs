using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Story()
    {
    }

    public void Custom()
    {

    }

    public void Editor()
    {
        Controller.isGame = false;
        SceneManager.LoadScene("Scene Game");
    }

    public void Option()
    {

    }

    public void Credits()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }
}
