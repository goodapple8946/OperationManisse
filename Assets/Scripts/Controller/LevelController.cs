using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    private GameObject loading;

    struct LevelInfo
    {
        public string code;
        public string title;
        public string tip;
        public LevelInfo(string code, string title, string tip)
        {
            this.code = code;
            this.title = title;
            this.tip = tip;
        }
    }

    private LevelInfo[] levelInfos =
    {
        new LevelInfo("0", "TEST", "This is a test level."),
        new LevelInfo("1", "SHELTER", "Build some obstacles to block missiles."),
    };

    void Start()
    {
        loading = GameObject.Find("Canvas/Loading");
        loading.SetActive(false);
    }

	public void LoadLevel(int level)
    {
        LevelInfo info = levelInfos[level];
        loading.transform.GetChild(0).GetComponent<Text>().text = "LEVEL " + info.code;
        loading.transform.GetChild(1).GetComponent<Text>().text = info.title;
        loading.transform.GetChild(2).GetComponent<Text>().text = info.tip;

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
