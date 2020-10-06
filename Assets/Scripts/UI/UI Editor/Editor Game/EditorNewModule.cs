using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorNewModule : MonoBehaviour
{
    private InputField inputField;
    private void Awake()
    {
        inputField = GetComponent<InputField>();
        inputField.onEndEdit.AddListener(str =>
        {
            editorController.moduleSelected = str;
        });
    }
}
