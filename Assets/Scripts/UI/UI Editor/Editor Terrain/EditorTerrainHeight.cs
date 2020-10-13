using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorTerrainHeight : EditorUI
{
    private InputField inputField;

    public void Awake()
    {
        inputField = GetComponent<InputField>();
        inputField.onValueChanged.AddListener(value =>
        {
            int height;
            bool legeal = int.TryParse(value, out height);
            if (!legeal || height < 1)
            {
                height = 1;
            }
            editorController.TerrainHeight = height;
        });
    }

    public override void UpdateShowing()
    {
        inputField.text = editorController.TerrainHeight.ToString();
    }
    public void AddHeight(int y)
    {
        int height = editorController.TerrainHeight + y;
        height = Mathf.Max(height, 1);
        height = Mathf.Min(height, 999);
        editorController.TerrainHeight = height;
    }
}
