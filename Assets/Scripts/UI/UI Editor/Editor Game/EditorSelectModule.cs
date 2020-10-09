using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorSelectModule : MonoBehaviour
{
    private Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            string fileName = GetComponentInChildren<Text>().text;
            FileViewer.moduleSelected = fileName;
        });
    }
}
