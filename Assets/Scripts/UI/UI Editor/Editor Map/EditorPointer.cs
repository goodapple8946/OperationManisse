using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static Controller;

public class EditorPointer : MonoBehaviour
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
