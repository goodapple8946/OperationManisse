using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
	public void LoadLevel1()
	{
		SceneManager.LoadScene("Test");
	}

    public void Quit()
    {
        Application.Quit();
    }
}
