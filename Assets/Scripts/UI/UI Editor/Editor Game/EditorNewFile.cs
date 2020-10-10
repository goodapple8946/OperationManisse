using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorNewFile : MonoBehaviour
{
    private InputField inputField;
    private void Awake()
    {
        inputField = GetComponent<InputField>();
        inputField.onEndEdit.AddListener(name =>
        {
			// 添加后缀
            FileViewer.DealFile(name + ".xml");
        });
    }
}
