using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorFiles : MonoBehaviour
{
    private void Start()
    {
        UpdateFiles();
    }

    // 更新展示的游戏文件
    public void UpdateFiles()
    {
        string[] files = ResourceController.GetFilesInDirectory(ResourceController.GamePath);

        // 清除当前显示的文件
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        float height = 70f;
        // 重新创建要显示的文件的Prefab
        Instantiate(resourceController.newFilePrefab, transform);
        foreach (string file in files)
        {
            GameObject fileObj = Instantiate(resourceController.filePrefab, transform);
            string[] str = file.Split('/', '.');
            fileObj.GetComponentInChildren<Text>().text = str[str.Length - 2];
            height += 70f;
        }
        GetComponent<RectTransform>().sizeDelta = new Vector2(0, height);
    }
}
