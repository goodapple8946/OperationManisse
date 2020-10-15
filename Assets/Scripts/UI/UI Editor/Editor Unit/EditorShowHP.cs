using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorShowHP : EditorUI
{
    private Toggle toggle;

    public void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(value =>
        {
            editorController.IsShowingHP = value;
        });

        tipTitle = "Unit Show HP";
        tipContent = "  HP bars of all units are always displayed.";
    }

    public override void UpdateShowing()
    {
        toggle.transform.GetChild(1).GetComponent<Text>().text = editorController.IsShowingHP ? "On" : "Off";
    }
}
