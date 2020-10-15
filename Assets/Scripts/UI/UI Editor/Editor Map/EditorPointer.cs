using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorPointer : EditorUI
{
	private Toggle toggle;
	private Text content;

	private static readonly string PNAME1 = "Point 1";
	private static readonly string PNAME2 = "Point 2";

	public static EditorPointer point1;
	public static EditorPointer point2;

	public void Awake()
	{
		content = transform.GetComponentInChildren<Text>();
		toggle = transform.GetComponent<Toggle>();
		Debug.Assert(toggle != null);
		// 设置两个物体
		if(content.text.Equals(PNAME1))
		{
			point1 = this;
		}
		else if(content.text.Equals(PNAME2))
		{
			point2 = this;
		}
		else
		{
			Debug.Assert(false);
		}

		tipTitle = "Map Building Grid";
		tipContent = "  The player can only place his units in the building grid when playing the game.";
	}

	public bool IsOn()
	{
		return toggle.isOn;
	}

	public bool SetOn(bool isOn)
	{
		return toggle.isOn = isOn;
	}

	public void UpdateShowing(Coord coord)
	{
		content.text = coord.ToString();
	}
}
