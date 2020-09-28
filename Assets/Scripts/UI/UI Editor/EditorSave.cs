using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static Controller;

public class EditorSave : MonoBehaviour
{
	public void SaveFile()
	{
		string filename = Path.Combine(Application.dataPath, "1.xml");

		List<Unit> units = editorController.Grid.OfType<Unit>().ToList();
		Debug.Log(units.Count);
		Serializer.Serialize(editorController, units, filename);
	}
}