using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorHold : MonoBehaviour
{
    private Dropdown dropdown;

    public void Awake()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(value =>
        {
            bool hold = value == 1;
            GameObject.Find("Editor Controller").GetComponent<EditorController>().isClickHold = hold;
        });
    }
}