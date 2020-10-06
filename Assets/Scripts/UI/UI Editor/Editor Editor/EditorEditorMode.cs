using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorEditorMode : MonoBehaviour
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
                editorController.EditorMode = (EditorMode)Enum.Parse(typeof(EditorMode), modeName);
                UpdateShowing();
                content.GetComponent<EditorContent>().UpdateByEditorMode();
            }
        });
    }

    public void UpdateShowing()
    {
        toggle.SetIsOnWithoutNotify(editorController.EditorMode.ToString() == modeName);
    }
}