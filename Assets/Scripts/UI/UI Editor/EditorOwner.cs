using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameController;

public class EditorOwner : MonoBehaviour
{
    private Dropdown dropdown;

    public void Awake()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(value =>
        {
            Player player = (Player)value;
            GameObject.Find("Editor Controller").GetComponent<EditorController>().PlayerOwner = player;
        });
    }

	public void ShowOwner(Player player)
	{
		dropdown.SetValueWithoutNotify((int)player);
	}
}