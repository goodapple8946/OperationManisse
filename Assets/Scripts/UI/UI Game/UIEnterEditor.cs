using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class UIEnterEditor : MonoBehaviour
{
	private Button button;

	public void Awake()
	{
		if (Controller.isGame)
		{
			gameObject.SetActive(false);
		}
        else
		{
			button = GetComponent<Button>();
			button.onClick.AddListener(() =>
			{
				gameController.GamePhase = GamePhase.Editor;
			});
		}
	}
}
