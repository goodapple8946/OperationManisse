using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorShowHP : MonoBehaviour
{
    private Dropdown dropdown;

    public void Awake()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(value =>
        {
            bool showHP = value == 1;
            GameObject.Find("Editor Controller").GetComponent<EditorController>().isShowingHP = showHP;
        });
    }
}
