using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorShowHP : MonoBehaviour
{
    private Toggle toggle;

    public void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(value =>
        {
            editorController.isShowingHP = value;
        });
    }

    public void Update()
    {
        toggle.transform.GetChild(1).GetComponent<Text>().text = editorController.isShowingHP ? "On" : "Off";
    }
}
