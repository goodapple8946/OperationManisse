using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorNewModule : MonoBehaviour
{
    private Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            editorController.moduleSelected = (GetComponentInParent<EditorModules>().Count + 1).ToString();
        });
    }
}
