using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorAmbience : EditorUI
{
    private Dropdown dropdown;
    
    void Awake()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(value =>
        {
            editorController.AmbienceObject = Instantiate(resourceController.ambienceObjects[value]);
        });
    }

    public override void UpdateShowing()
    {
        // TODO
    }
}
