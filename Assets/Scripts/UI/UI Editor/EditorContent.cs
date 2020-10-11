using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Controller;

public class EditorContent : MonoBehaviour
{
    private GameObject editorUnit;
    private GameObject editorBackground;

    void Awake()
    {
        editorUnit = transform.Find("Editor Unit").gameObject;
        editorBackground = transform.Find("Editor Background").gameObject;
    }

    // 更新Content内所有显示的数据
    public void UpdateUIShowing<T>()
    {
        //BroadcastMessage("UpdateShowing");
        T editorUI = GetComponentInChildren<T>();
        if (editorUI != null)
        {
            (editorUI as EditorUI).UpdateShowing();
        }
    }

    // 更新所有Content内所有显示的数据
    public void UpdateUIShowingAll()
    {
        //BroadcastMessage("UpdateShowing");
        EditorUI[] editorUIs = GetComponentsInChildren<EditorUI>();
        foreach (EditorUI editorUI in editorUIs)
        {
            editorUI.UpdateShowing();
        }
    }

    // 根据Editor Mode，对Content内的各功能部分显示或隐藏
    public void UpdateByEditorMode()
    {
        editorUnit.SetActive(editorController.EditorMode == EditorMode.Unit);
        editorBackground.SetActive(editorController.EditorMode == EditorMode.Background);

        UpdateHeight();
    }

    // 更新Content高度
    public void UpdateHeight()
    {
        float height = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
            {
                height += transform.GetChild(i).GetComponent<RectTransform>().sizeDelta.y;
            }
        }
        GetComponent<RectTransform>().sizeDelta = new Vector2(0, height);
    }
}       
