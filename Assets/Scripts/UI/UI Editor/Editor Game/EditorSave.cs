using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorSave : MonoBehaviour
{
	Button button;

	public void Awake()
	{
		button = GetComponent<Button>();
		button.onClick.AddListener(() =>
		{
			if (editorController.fileSelected != "")
			{
				SaveFile2FS(editorController.fileSelected);
				editorController.UpdateFiles();
			}
		});
	}

	/// <summary>
	/// 转换成xml,保存filename到文件系统,如果重名让编辑者选择
	/// </summary>
	void SaveFile2FS(string filename)
	{
        try
		{
			string path = System.IO.Path.Combine(ResourceController.GamePath, filename + ".xml");
			SaveFile(path);
			resourceController.playAudio("Success");
		}
		catch (Exception e)
		{
			Debug.Log(e.Message);
			resourceController.playAudio("Error");
		}
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
		XMLGame game = GetXMLGame(editorController, backgrounds, units, goodsVisible);
		Serializer.SerializeGame(game, path);
	}

	/// <summary>
	/// 根据参数构造Game并序列化
	/// </summary>
	private static XMLGame GetXMLGame(
		EditorController editorController, List<Background> backgrounds,
		List<Unit> units, List<string> goodsVisable)
	{
		XMLMap map = Serializer.EditorController2XML(editorController);

		List<XMLBackground> xmlBackgrounds = new List<XMLBackground>();
		foreach (Background background in backgrounds)
		{
			XMLBackground xmlBackground = Serializer.Background2XML(background);
			xmlBackgrounds.Add(xmlBackground);
		}

		List<XMLUnit> xmlUnits = new List<XMLUnit>();
		foreach (Unit unit in units)
		{
			XMLUnit xmlUnit = Serializer.Unit2XML(unit);
			xmlUnits.Add(xmlUnit);
		}

		XMLGame game = new XMLGame(map, xmlBackgrounds, xmlUnits, goodsVisable);
		return game;
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