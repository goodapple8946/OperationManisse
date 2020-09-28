using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorEditorMode : MonoBehaviour
{
    private Dropdown dropdown;
    private GameObject content;

    public void Awake()
    {
        content = GameObject.Find("UI Canvas/UI Editor/Viewport/Content");

        dropdown = GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(value =>
        {
            editorController.EditorMode = (EditorMode)value;
            content.GetComponent<EditorContent>().UpdateActive();
        });
    }

    public void ShowEditorMode(EditorMode mode)
    {
        dropdown.SetValueWithoutNotify((int)mode);
    }
}