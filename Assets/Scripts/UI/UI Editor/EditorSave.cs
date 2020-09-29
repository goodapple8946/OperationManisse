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
		if(editorController.Backgrounds == null)
		{
			Debug.Log(123);
		}
		List<Background> backgrounds = editorController.Backgrounds.OfType<Background>().ToList();


		List<Unit> units = editorController.Grid.OfType<Unit>().ToList();
		foreach(Unit unit in units)
		{
			if(unit.gameObject == null)
			{
				Debug.Log("EditorController.Grid中的Unit绑定的gameObject被销毁!");
			}
		}

		List<string> goodsVisible = shopController.GetShopObjectVisibility();
		Serializer.Serialize(editorController, backgrounds, units, goodsVisible, filename);
	}
}