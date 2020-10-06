using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorFiles : MonoBehaviour
{
    public int Count { get; set; }

    private void Start()
    {
        UpdateFiles();
    }

    // 更新展示的游戏文件
    public void UpdateFiles()
    {
        string[] files = ResourceController.GetFilesInDirectory("Game");

        // 清除当前显示的文件
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        // 重新创建要显示的文件的Prefab
        foreach(string file in files)
        {
            GameObject fileObj = Instantiate(resourceController.filePrefab, transform);
            string[] str = file.Split('/', '.');
            fileObj.GetComponentInChildren<Text>().text = str[str.Length - 2];
        }
        Instantiate(resourceController.newFilePrefab, transform);

        float fileHeight = 70f;
        int fileNumOfRow = 5;
        float widthRow = 350f;

        // 调整显示区域高度
        float height = (files.Length / fileNumOfRow + 1) * fileHeight;
        GetComponent<RectTransform>().sizeDelta = new Vector2(widthRow, height);

        // 更新父物体（File）高度
        transform.parent.GetComponentInParent<RectTransform>().sizeDelta = new Vector2(0, height + 150);

        // 更新Content高度
        GetComponentInParent<EditorContent>().UpdateHeight();

        // 文件计数（不包括新建文件）
        Count = files.Length;
    }
}
