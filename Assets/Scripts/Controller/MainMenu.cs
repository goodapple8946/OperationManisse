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
        SceneManager.LoadScene("Scene Editor");
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
