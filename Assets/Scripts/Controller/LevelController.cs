using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour
{
    public GameObject loading;

    private AsyncOperation async;

    private float distanceTitleTarget = 32f;
    private Vector3 positionTitleTarget = Vector3.zero;

    private bool isLoading = false;
    private float timeLoading = 0f;
    private float timeLoadingMax = 2f;

    public int level = -1;

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

    void Start()
    {
        if (level >= 0)
        {
            SetLevelInfo(level);
        }
    }

    void Update()
    {
        if (isLoading)
        {
            if (positionTitleTarget == Vector3.zero)
            {
                positionTitleTarget = loading.transform.GetChild(0).position + new Vector3(0, distanceTitleTarget, 0);
            }
            loading.transform.GetChild(0).position = loading.transform.GetChild(0).position + (positionTitleTarget - loading.transform.GetChild(0).position) * 1f * Time.deltaTime;
            loading.transform.GetChild(1).GetComponent<Text>().color = Color.Lerp(Color.black, Color.white, timeLoading / timeLoadingMax);

            timeLoading += Time.deltaTime;
        }
    }

    private LevelInfo[] levelInfos =
    {
        new LevelInfo("0", "TEST", "This is a test level."),
        new LevelInfo("1", "SHELTER", "Build some obstacles to block missiles."),
        new LevelInfo("2", "HIGHER", "Where is the weakness."),
        new LevelInfo("3", "IMPACT", "No weapons. Knock it Down."),
        new LevelInfo("4", "DASH", "Full Ahead."),
        new LevelInfo("5", "RISE", "Bring the light into the sky."),
        new LevelInfo("6", "BESIEGE", "We have been surrounded."),
    };

	public void LoadLevel(int level)
    {
        SetLevelInfo(level);

        // 读取
        loading.SetActive(true);
        StartCoroutine(Loading(level));
    }

    IEnumerator Loading(int level)
    {
        isLoading = true;
        async = SceneManager.LoadSceneAsync("Level " + level);
        async.allowSceneActivation = false;

        yield return new WaitForSeconds(timeLoadingMax + 1f);

        async.allowSceneActivation = true;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SetLevelInfo(int level)
    {
        // 信息
        LevelInfo info = levelInfos[level];
        loading.transform.GetChild(0).GetComponent<Text>().text = "LEVEL " + info.code;
        loading.transform.GetChild(1).GetComponent<Text>().text = info.title;
        loading.transform.GetChild(2).GetComponent<Text>().text = info.tip;
    }
}
