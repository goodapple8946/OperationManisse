using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorOwner : MonoBehaviour
{
    private Dropdown dropdown;

    public void Awake()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(value =>
        {
            Player player = (Player)value;
            editorController.PlayerOwner = player;
        });
    }

	public void ShowOwner(Player player)
	{
		dropdown.SetValueWithoutNotify((int)player);
	}
}