using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorSizeX : MonoBehaviour
{
    InputField inputField;

	public void Awake()
    {
        inputField = GetComponent<InputField>();
        inputField.onEndEdit.AddListener(value =>
        {
            if (value != "" && value != "-")
            {
                int x = int.Parse(value);
				GameObject.Find("Editor Controller").GetComponent<EditorController>().XNum = x;
			}
        });
    }

	public void ShowX(int x)
	{
		inputField.text = x + "";
	}
}
