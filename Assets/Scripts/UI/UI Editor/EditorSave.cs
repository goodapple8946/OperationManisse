#define DEBUG

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using static Controller;


public class EditorSave : MonoBehaviour
{
	InputField inputField;

	public void Awake()
	{
		inputField = GetComponent<InputField>();
		inputField.onValueChanged.AddListener(filename => SaveFile2FS(filename));
		// 初始文本
		inputField.SetTextWithoutNotify("Save");
	}

	/// <summary>
	/// 转换成xml,保存filename到文件系统,如果重名让编辑者选择
	/// </summary>
	void SaveFile2FS(string filename)
	{
		string path = System.IO.Path.Combine(Application.dataPath, filename + ".xml");
		// 文件系统存在重名文件
		if (File.Exists(path))
		{
			bool ok = EditorUtility.DisplayDialog("", 
				"Are you sure you want to replace existing file?", "ok", "cancel");
			if (ok)
			{
				SaveFile(path);
			}
		}
		else
		{
			SaveFile(path);
		}
		// 更新输入区文本
		inputField.SetTextWithoutNotify("Save");
	}

	/// <summary>
	/// 转换成xml,保存至文件中
	/// </summary>
	private static void SaveFile(string path)
	{
		CheckEditorResult(editorController.Backgrounds,
			editorController.Grid, shopController.GetShopObjectVisibility());

		List<Background> backgrounds = editorController.Backgrounds.ToList<Background>();
		List<Unit> units = editorController.Grid.OfType<Unit>().ToList();
		List<string> goodsVisible = shopController.GetShopObjectVisibility();
		Serializer.SerializeGame(editorController, backgrounds, units, goodsVisible, path);
	}

	// 编辑结果的检测
	[System.Diagnostics.Conditional("DEBUG")]
	private static void CheckEditorResult(HashSet<Background> Backgrounds, Unit[,] Grid, List<string> goodsVisible)
	{
		System.Diagnostics.Debug.Assert(
			editorController.Backgrounds != null, "Backgrounds 数组为空");

		foreach (Unit unit in Grid)
		{
			System.Diagnostics.Debug.Assert(
				unit != null && unit.gameObject != null, "unit绑定的gameObject已被销毁");
		}

		foreach (string goodname in goodsVisible)
		{
			bool containName = resourceController.gameObjDictionary.ContainsKey(goodname);
			System.Diagnostics.Debug.Assert(
				containName, goodname + "无法在resourceController找到");
		}
	}
}