using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorNewFile : MonoBehaviour
{
    private Button button;
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            editorController.FileSelected = (GetComponentInParent<EditorFiles>().Count + 1).ToString();
        });
    }
}
