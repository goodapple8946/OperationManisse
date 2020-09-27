using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorHold : MonoBehaviour
{
    private Toggle toggle;
    private EditorController editorController;

    public void Awake()
    {
        editorController = GameObject.Find("Editor Controller").GetComponent<EditorController>();

        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(value =>
        {
            editorController.isClickHold = value;
        });
    }

    public void Update()
    {
        toggle.transform.GetChild(1).GetComponent<Text>().text = editorController.isClickHold ? "On" : "Off";
    }
}