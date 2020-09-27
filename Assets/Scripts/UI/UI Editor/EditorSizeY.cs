using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorSizeY : MonoBehaviour
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
                GameObject.Find("Editor Controller").GetComponent<EditorController>().SetYNum(y);
            }
        });
    }
}
