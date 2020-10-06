using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorModules : MonoBehaviour
{
    private void Start()
    {
        UpdateModules();
    }

    // 更新展示的游戏文件
    public void UpdateModules()
    {
        string[] modules = ResourceController.GetFilesInDirectory(ResourceController.ModulePath);

        // 清除当前显示的文件
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        float height = 70f;
        // 重新创建要显示的文件的Prefab
        Instantiate(resourceController.newModulePrefab, transform);
        foreach (string module in modules)
        {
            GameObject moduleObj = Instantiate(resourceController.modulePrefab, transform);
            string[] str = module.Split('/', '.');
            moduleObj.GetComponentInChildren<Text>().text = str[str.Length - 2];
            height += 70f;
        }
        GetComponent<RectTransform>().sizeDelta = new Vector2(0, height);
    }
}
