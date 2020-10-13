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
            editorController.Ambience = StringToAmbience(value);
        });
    }

    public override void UpdateShowing()
    {
        dropdown.SetValueWithoutNotify((int)victoryController.victoryCondition);
    }

    private Ambience StringToAmbience(string str)
    {
        switch (str)
        {
            case "Sunday":
                return Ambience.Sunday;
            case "Rainday":
                return Ambience.Rainday;
            default:
                throw new Exception(string.Format("Unknown ambience: {0}.", str));
        }
    }

}
