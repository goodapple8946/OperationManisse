using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorHold : EditorUI
{
    private Toggle toggle;

    public void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(value =>
        {
            editorController.IsClickHold = value;
            UpdateShowing();
        });

        tipTitle = "Unit Fast Click";
        tipContent = "  Objects can be placed or deleted continuousely by holding the mouse buttons.";
    }

    public override void UpdateShowing()
    {
        toggle.transform.GetChild(1).GetComponent<Text>().text = editorController.IsClickHold ? "On" : "Off";
    }
}