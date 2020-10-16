using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Controller;

public class EditorContent : MonoBehaviour
{
    private GameObject editorUnit;
    private GameObject editorBackground;
    private GameObject editorTerrain;

    void Awake()
    {
        editorUnit = transform.Find("Editor Unit").gameObject;
        editorBackground = transform.Find("Editor Background").gameObject;
        editorTerrain = transform.Find("Editor Terrain").gameObject;
    }

    // 更新Content内所有显示的数据
    public void UpdateUIShowing<T>()
    {
        //BroadcastMessage("UpdateShowing");
        T[] editorUIs = GetComponentsInChildren<T>();
        foreach (T editorUI in editorUIs)
        {
            (editorUI as EditorUI).UpdateShowing();
        }
    }

    // 根据Editor Mode，对Content内的各功能部分显示或隐藏
    public void RefreshByEditorMode()
    {
        editorUnit.SetActive(editorController.EditorMode == EditorMode.Unit);
        editorBackground.SetActive(editorController.EditorMode == EditorMode.Background);
        editorTerrain.SetActive(editorController.EditorMode == EditorMode.Terrain);

        UpdateHeight();
    }

    // 更新Content高度
    public void UpdateHeight()
    {
        float height = 800f;

        switch (editorController.EditorMode)
        {
            case EditorMode.Unit:
                height += 200f;
                break;
            case EditorMode.Background:
                height += 100f;
                break;
            case EditorMode.Terrain:
                height += 150f;
                break;
        }
        GetComponent<RectTransform>().sizeDelta = new Vector2(0, height);
    }

    // 更新Content高度
    //public void UpdateHeight()
    //{
    //    float height = 0;
    //    for (int i = 0; i < transform.childCount; i++)
    //    {
    //        if (transform.GetChild(i).gameObject.activeSelf)
    //        {
    //            height += transform.GetChild(i).GetComponent<RectTransform>().sizeDelta.y;
    //        }
    //    }
    //    GetComponent<RectTransform>().sizeDelta = new Vector2(0, height);
    //}
}       
