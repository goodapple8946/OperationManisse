using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorModules : MonoBehaviour
{
    public int Count { get; set; }

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

        // 重新创建要显示的文件的Prefab
        foreach (string module in modules)
        {
            GameObject moduleObj = Instantiate(resourceController.modulePrefab, transform);
            string[] str = module.Split('/', '.');
            moduleObj.GetComponentInChildren<Text>().text = str[str.Length - 2];
        }
        Instantiate(resourceController.newModulePrefab, transform);

        float moduleHeight = 70f;
        int moduleNumOfRow = 5;
        float widthRow = 350f;

        // 调整显示区域高度
        float height = (modules.Length / moduleNumOfRow + 1) * moduleHeight;
        GetComponent<RectTransform>().sizeDelta = new Vector2(widthRow, height);

        // 更新父物体（Module）高度
        transform.parent.GetComponentInParent<RectTransform>().sizeDelta = new Vector2(0, height + 150);

        // 更新Content高度
        GetComponentInParent<EditorContent>().UpdateHeight();

        // 文件计数（不包括新建文件）
        Count = modules.Length;
    }
}
