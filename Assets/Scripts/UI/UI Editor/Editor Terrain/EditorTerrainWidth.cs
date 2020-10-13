using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorTerrainWidth : EditorUI
{
    private InputField inputField;

    public void Awake()
    {
        inputField = GetComponent<InputField>();
        inputField.onValueChanged.AddListener(value =>
        {
            int width;
            bool legeal = int.TryParse(value, out width);
            if (!legeal || width < 1)
            {
                width = 1;
            }
            editorController.TerrainWidth = width;
        });
    }

    public override void UpdateShowing()
    {
        inputField.text = editorController.TerrainWidth.ToString();
    }

    public void AddWidth(int x)
    {
        int width = editorController.TerrainWidth + x;
        width = Mathf.Max(width, 1);
        width = Mathf.Min(width, 999);
        editorController.TerrainWidth = width;
    }
}
