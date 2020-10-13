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
            int y;
            bool legeal = int.TryParse(value, out y);
            if (!legeal || y < 1)
            {
                y = 1;
            }
            editorController.YNum = y;
        });
    }

	public override void UpdateShowing()
	{
		inputField.text = editorController.YNum + "";
	}
}
