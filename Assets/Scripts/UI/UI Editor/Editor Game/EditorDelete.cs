using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorDelete : MonoBehaviour
{
	Button button;

	private void Awake()
	{
		button = GetComponent<Button>();
		button.onClick.AddListener(() =>
		{
			string path = ResourceController.GamePath + editorController.fileSelected + ".xml";
			try
			{
				File.Delete(path);
				resourceController.playAudio("Success");
			}
			catch (Exception e)
			{
				Debug.Log(e.Message);
				resourceController.playAudio("Error");
			}
			editorController.UpdateFiles();
		});
	}
}