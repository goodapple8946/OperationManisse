using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    private GameObject titleObj;
    private GameObject subtitleObj;
    private GameObject contentObj;

    private float width = 400f;
    private float height = 0;

    public static Tooltip tooltip;

    /// <summary>
    /// 隐藏提示条。
    /// </summary>
    public void Hide()
    {
        Show(null, null, null);
    }

    /// <summary>
    /// 显示提示条，并且设置内容。如果需要跳过某条目，则传入null。
    /// </summary>
    public void Show(string title, string content)
    {
        Show(title, null, content);
    }

    /// <summary>
    /// 显示提示条，并且设置内容。如果需要跳过某条目，则传入null。
    /// </summary>
    public void Show(string title, string subtitle, string content)
    {
        InitTextObj(titleObj, title);
        InitTextObj(subtitleObj, subtitle);
        InitTextObj(contentObj, content);

        height = 0;
        height += GetTextObjHeight(titleObj);
        height += GetTextObjHeight(subtitleObj);
        height += GetTextObjHeight(contentObj);
    }

    private void Awake()
    {
        titleObj = transform.Find("Title").gameObject;
        subtitleObj = transform.Find("Subtitle").gameObject;
        contentObj = transform.Find("Content").gameObject;

        tooltip = gameObject.GetComponent<Tooltip>();
    }

    private void Start()
    {
        Show(null, null, null);
    }

    private void Update()
    {
        ChangePosition();
    }

    // 跟随鼠标
    private void ChangePosition()
    {
        Vector2 pos = Input.mousePosition;

        if (pos.y - height < 0)
        {
            pos += new Vector2(0, height);
        }

        if (pos.x + width > Screen.width)
        {
            pos += new Vector2(Screen.width - pos.x - width, 0);
        }

        GetComponent<RectTransform>().position = pos;
    }

    // 根据字符串初始化文本内容
    private void InitTextObj(GameObject obj, string str)
    {
        if (str == null)
        {
            obj.SetActive(false);
        }
        else
        {
            obj.SetActive(true);
            obj.GetComponentInChildren<Text>().text = str;
        }
    }

    private float GetTextObjHeight(GameObject obj)
    {
        if (obj.activeSelf)
        {
            return obj.GetComponent<RectTransform>().sizeDelta.y;
        }
        else
        {
            return 0;
        }
    }
}