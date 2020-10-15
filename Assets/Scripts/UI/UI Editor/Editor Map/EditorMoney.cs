using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorMoney : EditorUI
{
    private InputField inputField;

    public void Awake()
    {
        inputField = GetComponent<InputField>();
        inputField.onValueChanged.AddListener(value => 
        {
            if (value != "" && value != "-")
            {
                int money = int.Parse(value);
                editorController.PlayerMoneyOrigin = money;
            }
        });

        tipTitle = "Map Building Grid";
        tipContent = "  The amount of money given to the player when the game begins.";
    }

	public override void UpdateShowing()
	{
		inputField.text = editorController.PlayerMoneyOrigin.ToString();
	}
}
