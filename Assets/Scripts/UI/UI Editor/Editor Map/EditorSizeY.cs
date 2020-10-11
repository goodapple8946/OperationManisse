using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorSizeY : EditorUI
{
    InputField inputField;

    public void Awake()
    {
        inputField = GetComponent<InputField>();
        inputField.onEndEdit.AddListener(value =>
        {
            if (value != "" && value != "-")
            {
                int y = int.Parse(value);
				editorController.YNum = y;
            }
        });
    }

	public override void UpdateShowing()
	{
		inputField.text = editorController.YNum + "";
	}
}
