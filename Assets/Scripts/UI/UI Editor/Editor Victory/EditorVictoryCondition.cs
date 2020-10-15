using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorVictoryCondition : EditorUI
{
    private Dropdown dropdown;

    void Awake()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(value =>
        {
            VictoryCondition victoryCondition = (VictoryCondition)value;
            victoryController.victoryCondition = victoryCondition;
        });

        tipTitle = "Victory Condition";
        tipContent = "  The condition to win during the game.";
    }

    public override void UpdateShowing()
    {
        dropdown.SetValueWithoutNotify((int)victoryController.victoryCondition);
    }
}
