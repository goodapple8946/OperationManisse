using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorDeleteGame : MonoBehaviour
{
	Button button;

	private void Awake()
	{
		button = GetComponent<Button>();
		button.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
		{
			FileViewer.ViewerState = FileViewer.State.DeleteGame;
		}));
	}
}