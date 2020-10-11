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
                editorController.PlayerOwner = StringToPlayer(playerName);
            }
        });
    }

	public void UpdateShowing()
	{
        toggle.SetIsOnWithoutNotify(editorController.PlayerOwner.ToString() == playerName);
	}

    // 按钮上的text转化为所有者
    private Player StringToPlayer(string player)
    {
        switch (player[0])
        {
            case 'N':
                return Player.Neutral;
            case 'P':
                return Player.Player;
            case 'E':
                return Player.Enemy;
            default:
                throw new Exception("Unknown player selected.");
        }
    }
}