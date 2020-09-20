using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public GameObject loading;

    private AsyncOperation async;

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
        new LevelInfo("1", "BUILD A SHELTER", "Build some obstacles to block missiles."),
        new LevelInfo("2", "THE HIGHER THE STRONGER", "Try to find where the enemy's defenses are weak."),
    };

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
        async = SceneManager.LoadSceneAsync("Level " + level);
        async.allowSceneActivation = false;

        yield return new WaitForSeconds(3f);

        async.allowSceneActivation = true;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
