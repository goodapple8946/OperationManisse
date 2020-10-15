using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorLoadGame : EditorUI
{
	Button button;

	private void Awake()
	{
		button = GetComponent<Button>();
		button.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
		{
			FileViewer.ViewerState = FileViewer.State.LoadGame;
		}));

		tipTitle = "Load File";
		tipContent = "  Load a saved file. Current objects and configurations will be overriden.";
	}

	public static void LoadGameFromFS(String filename)
	{
		try
		{
			string path = System.IO.Path.Combine(ResourceController.GamePath, filename);
			XMLGame game = Serializer.Deserialized<XMLGame>(path);
			Load(game);
			resourceController.playAudio("Success");
		}
		catch (Exception e)
		{
			Debug.Log(e.Message);
			resourceController.playAudio("Error");
		}
	}

	// 加载XMLGame对象
	private static void Load(XMLGame game)
	{
		// 清空当前网格
		editorController.Clear();

		// 加载地图信息
		XMLMap map = game.xmlMap;
		Load(map);

		// 加载背景图片
		List<XMLBackground> xmlBackgrounds = game.xmlBackgrounds;
		xmlBackgrounds.ForEach(Load);

		// 加载地块
		List<XMLTerrainA> xmlTerrains = game.xmlTerrains;
		xmlTerrains.ForEach(Load);

		// 加载单位信息
		List<XMLUnit> xmlUnits = game.xmlUnits;
		xmlUnits.ForEach(Load);

		// 加载可用商品
		List<string> goodsVisible = game.goodsVisable;
		shopController.SetShopObjectVisibility(goodsVisible);
	}

	/// <summary>
	/// 读取map并修改editorController的设置
	/// </summary>
	private static void Load(XMLMap map)
	{
		editorController.XNum = map.xNum;
		editorController.YNum = map.yNum;
		editorController.PlayerMoneyOrigin = map.money;
		editorController.LightIntensity = map.lightIntensity;
		editorController.CurrAmbience = resourceController.GetAmbience(map.ambienceName);
		editorController.BuildingCoord1 = map.buildingCoord1;
		editorController.BuildingCoord2 = map.buildingCoord2;
		victoryController.victoryCondition = map.victoryCond;
	}

	/// <summary>
	/// 读取xmlBackground并克隆,保存到editorController里
	/// </summary>
	private static void Load(XMLBackground xmlBackground)
	{
		Background background = XML2Background(xmlBackground);
		editorController.Put(background);
	}

	/// <summary>
	/// 读取xmlBackground并克隆,保存到editorController里
	/// </summary>
	private static void Load(XMLTerrainA xmlTerrain)
	{
		TerrainA terrain = XML2Terrain(xmlTerrain);
		editorController.Put(terrain);
	}

	/// <summary>
	/// 读取xmlUnit并克隆,保存到editorController里
	/// </summary>
	public static void Load(XMLUnit xmlUnit)
	{
		Unit unit = XML2Unit(xmlUnit);
		// 设置unit的网格位置,和坐标
		editorController.Put(new Coord(xmlUnit.x, xmlUnit.y), unit, (Player)xmlUnit.player);
	}

	private static Background XML2Background(XMLBackground xmlBackground)
	{
		// 复制一份物体
		GameObject objPrefab = resourceController.gameObjDictionary[xmlBackground.name];
		GameObject objClone = Instantiate(objPrefab);
		objClone.name = objPrefab.name; // 默认复制名称是GameObject Name (Clone)

		// 设置位置和大小
		objClone.transform.position = xmlBackground.position;
		objClone.transform.localScale = xmlBackground.localScale;

		Background background = objClone.GetComponent<Background>();
		return background;
	}

	private static TerrainA XML2Terrain(XMLTerrainA xmlTerrain)
	{
		// 复制一份物体
		GameObject objPrefab = resourceController.gameObjDictionary[xmlTerrain.name];
		GameObject objClone = Instantiate(objPrefab);
		objClone.name = objPrefab.name; // 默认复制名称是GameObject Name (Clone)

		TerrainA terrain = objClone.GetComponent<TerrainA>();
		// 设置位置和大小
		terrain.Width = xmlTerrain.width;
		terrain.Height = xmlTerrain.height;
		objClone.transform.position = xmlTerrain.position;
		return terrain;
	}

	/// <summary>
	/// 根据xmlUnit创建一份unit拷贝, 并根据网格中的坐标设置世界坐标
	/// </summary>
	public static Unit XML2Unit(XMLUnit xmlUnit)
	{
		// 复制一份物体
		GameObject objPrefab = resourceController.gameObjDictionary[xmlUnit.name];
		GameObject objClone = Instantiate(objPrefab);
		objClone.name = objPrefab.name; // 默认复制名称是GameObject Name (Clone)
		Unit unit = objClone.GetComponent<Unit>();

		// 计算存档与克隆出的方向之差,设置旋转角度
		int dirDifference = (xmlUnit.direction - unit.direction) + 4;
		unit.Rotate(dirDifference);
		// 计算网格中的位置
		unit.transform.position = editorController.MainGrid.Coord2WorldPos(new Coord(xmlUnit.x, xmlUnit.y));

		return unit;
	}
}