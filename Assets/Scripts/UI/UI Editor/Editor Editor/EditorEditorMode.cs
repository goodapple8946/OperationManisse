using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorEditorMode : EditorUI
{
    private Toggle toggle;
    private GameObject content;
    private string modeName;

    public void Awake()
    {
        content = GameObject.Find("UI Canvas/UI Editor/Viewport/Content");
        modeName = GetComponentInChildren<Text>().text;

        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                editorController.EditorMode = StringToEditorMode(modeName);
                UpdateShowing();
                content.GetComponent<EditorContent>().RefreshByEditorMode();
            }
        });

        tipTitle = "Editor Mode";
        tipSubtitle = StringToEditorMode(modeName).ToString();
        tipContent = "  Objects can be edited when the editor mode of their type selected.";
    }

    public override void UpdateShowing()
    {
        toggle.SetIsOnWithoutNotify(editorController.EditorMode == StringToEditorMode(modeName));
    }

    // 按钮上的text转化为编辑模式
    private EditorMode StringToEditorMode(string mode)
    {
        switch (mode[0])
        {
            case 'U':
                return EditorMode.Unit;
            case 'B':
                return EditorMode.Background;
            case 'T':
                return EditorMode.Terrain;
            case 'M':
                return EditorMode.Module;
            default:
                throw new Exception("Unknown editor mode selected.");
        }
    }
}