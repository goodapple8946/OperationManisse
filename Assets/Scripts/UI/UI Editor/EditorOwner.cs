using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorOwner : MonoBehaviour
{
    private Toggle toggle;
    private string playerName;

    public void Awake()
    {
        playerName = GetComponentInChildren<Text>().text;
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                editorController.PlayerOwner = (Player)Enum.Parse(typeof(Player), playerName);
            }
        });
    }

	public void UpdateShowing()
	{
        toggle.SetIsOnWithoutNotify(editorController.PlayerOwner.ToString() == playerName);
	}
}