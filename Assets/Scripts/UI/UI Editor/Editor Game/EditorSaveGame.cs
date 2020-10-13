using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorSaveGame : EditorUI
{
	Button button;

	public void Awake()
	{
		button = GetComponent<Button>();
		button.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
		{
			FileViewer.ViewerState = FileViewer.State.SaveGame;
		}));
	}

	/// <summary>
	/// 转换成xml,保存filename到文件系统,如果重名让编辑者选择
	/// </summary>
	public static void SaveGame2FS(string filename)
	{
		try
		{
			string path = System.IO.Path.Combine(ResourceController.GamePath, filename);
			SaveGame(path);
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
	private static void SaveGame(string path)
	{
		CheckEditorResult(editorController.Backgrounds,
			editorController.MainGrid.GetUnits(), 
			shopController.GetShopObjectVisibility());

		// 获取场景中所有的物品信息
		List<Background> backgrounds = editorController.Backgrounds.ToList<Background>();
		List<TerrainA> terrains = editorController.Terrains.ToList<TerrainA>();
		List<Unit> units = editorController.MainGrid.GetUnits();
		List<string> goodsVisible = shopController.GetShopObjectVisibility();

		XMLGame game = ObtainXMLGame(editorController, victoryController, 
			backgrounds, terrains, units, goodsVisible);
		Serializer.SerializeGame(game, path);
	}

	/// <summary>
	/// 根据参数构造XMLGame并序列化
	/// </summary>
	private static XMLGame ObtainXMLGame(
		EditorController editorController, VictoryController victoryController,
		List<Background> backgrounds, List<TerrainA> terrains,
		List<Unit> units, List<string> goodsVisable)
	{
		// 获取场景中物品的XML映射
		XMLMap map = Serializer.Map2XML(editorController, victoryController);

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

		List<XMLTerrainA> xmlTerrains = new List<XMLTerrainA>();
		foreach (TerrainA terrain in terrains)
		{
			XMLTerrainA xmlTerrain = Serializer.Terrain2XML(terrain);
			xmlTerrains.Add(xmlTerrain);
		}

		XMLGame game = new XMLGame(map, xmlBackgrounds, xmlTerrains, xmlUnits, goodsVisable);
		return game;
	}


	// 编辑结果的检测
	[System.Diagnostics.Conditional("DEBUG")]
	private static void CheckEditorResult(HashSet<Background> Backgrounds, List<Unit> units, List<string> goodsVisible)
	{
		System.Diagnostics.Debug.Assert(
			editorController.Backgrounds != null, "Backgrounds 数组为空");

		foreach (Unit unit in units)
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