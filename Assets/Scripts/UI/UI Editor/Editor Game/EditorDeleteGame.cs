using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorDeleteGame : EditorUI
{
	private Button button;

	private void Awake()
	{
		button = GetComponent<Button>();
		button.onClick.AddListener(() =>
		{
			FileViewer.ViewerState = FileViewer.State.DeleteGame;
		});
	}
}