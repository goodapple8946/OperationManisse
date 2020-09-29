#define DEBUG

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using static Controller;


public class EditorSave : MonoBehaviour
{

	// 编辑结果的检测
	[Conditional("Debug")]
	private void CheckEditorResult(HashSet<Background> Backgrounds, Unit[,] Grid, List<string> goodsVisible)
	{
		UnityEngine.Debug.Assert(editorController.Backgrounds != null, "Backgrounds 数组为空");
		foreach (Unit unit in Grid)
		{
			UnityEngine.Debug.Assert(unit.gameObject != null, "unit绑定的gameObject已被销毁");
		}
		foreach(string goodname in goodsVisible)
		{
			bool containName = resourceController.gameObjDictionary.ContainsKey(goodname);
			UnityEngine.Debug.Assert(containName, goodname + "无法在resourceController找到");
		}
	}


	public void SaveFile()
	{
		CheckEditorResult(
			editorController.Backgrounds, 
			editorController.Grid,
			shopController.GetShopObjectVisibility());

		string filename = Path.Combine(Application.dataPath, "1.xml");
		List<Background> backgrounds = editorController.Backgrounds.ToList<Background>();
		List<Unit> units = editorController.Grid.OfType<Unit>().ToList();
		List<string> goodsVisible = shopController.GetShopObjectVisibility();
		Serializer.Serialize(editorController, backgrounds, units, goodsVisible, filename);
	}
}