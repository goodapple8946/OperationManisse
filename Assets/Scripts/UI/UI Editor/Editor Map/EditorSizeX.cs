using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorSizeX : EditorUI
{
    InputField inputField;

    public void Awake()
    {
        inputField = GetComponent<InputField>();
        inputField.onEndEdit.AddListener(value =>
        {
            int x;
            bool legeal = int.TryParse(value, out x);
            if (!legeal || x < 1)
            {
                x = 1;
            }
            editorController.XNum = x;
        });
    }

    public override void UpdateShowing()
	{
		inputField.text = editorController.XNum + "";
	}
}
