using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorDeleteModule : MonoBehaviour
{
	Button button;

	private void Awake()
	{
		button = GetComponent<Button>();
		button.onClick.AddListener(() =>
		{
			try
			{
				string path = ResourceController.GamePath + editorController.moduleSelected + ".xml";
				File.Delete(path);
				editorController.UpdateFiles();
				resourceController.playAudio("Success");
			}
			catch (Exception e)
			{
				Debug.Log(e.Message);
				resourceController.playAudio("Error");
			}
		});
	}
}