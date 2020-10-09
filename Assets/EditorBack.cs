using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorBack : MonoBehaviour
{

	Button button;

	private void Awake()
	{
		button = GetComponent<Button>();
		button.onClick.AddListener(() =>
		{
			FileViewer.ViewerState = FileViewer.State.None;
		});
	}

}
