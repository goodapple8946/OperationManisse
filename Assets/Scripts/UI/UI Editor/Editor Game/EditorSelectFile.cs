using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Controller;

/// <summary>
/// 文件被选中后，将text名称发给editorController
/// </summary>
public class EditorSelectFile : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            string fileName = GetComponentInChildren<Text>().text;
			FileViewer.fileSelected = fileName;
        });
    }
}
