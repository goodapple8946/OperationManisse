using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EditorSaveFile : MonoBehaviour
{
	private GameController gameController;
	private EditorController editorController;
	private ResourceController resourceController;

	public void Awake()
	{
		gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
		editorController = GameObject.Find("Editor Controller").GetComponent<EditorController>();
		resourceController = GameObject.Find("Resource Controller").GetComponent<ResourceController>();
	}

	public void SaveFile()
	{
		string filename = "C://Users//asus//Desktop//1.xml";

		List<Unit> units = editorController.Grid.OfType<Unit>().ToList();
		Debug.Log(units.Count);
		Serializer.Serialize(editorController, units, filename);
	}
}
