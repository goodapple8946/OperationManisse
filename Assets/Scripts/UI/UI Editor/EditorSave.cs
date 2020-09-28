using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class EditorSave : MonoBehaviour
{
	private EditorController editorController;
	private ResourceController resourceController;

	public void Awake()
	{
		editorController = GameObject.Find("Editor Controller").GetComponent<EditorController>();
		resourceController = GameObject.Find("Resource Controller").GetComponent<ResourceController>();
	}

	public void SaveFile()
	{
		string filename = Path.Combine(Application.dataPath, "1.xml");

		List<Unit> units = editorController.Grid.OfType<Unit>().ToList();
		Debug.Log(units.Count);

		List<string> goodsVisable = 
		Serializer.Serialize(editorController, units, goodsVisable, filename);
	}
}