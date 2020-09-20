using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    private GameObject loading;

    void Awake()
    {
        loading = GameObject.Find("Canvas/Loading");
    }

    void Start()
    {
        if (loading.activeSelf)
        {
            loading.SetActive(false);
        }
    }

	public void LoadLevel(int level)
    {
        loading.SetActive(true);
        StartCoroutine(Loading(level));
	}

    IEnumerator Loading(int level)
    {
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("Level " + level);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
