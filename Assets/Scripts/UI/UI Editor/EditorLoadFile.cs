using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameController;

public class EditorLoadFile : MonoBehaviour
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

	public void LoadFile()
	{
		// 初始化网格
		editorController.XNum = 0;
		editorController.YNum = 0;

		// 根据文件生成Game
		string filename = "C://Users//asus//Desktop//1.xml";
		XMLGame game = Serializer.Deserialized(filename);

		// 加载地图信息
		XMLMap map = game.xmlMap;
		LoadMap(map);

		// 加载单位信息
		List<XMLUnit> xmlUnits = game.xmlUnits;
		foreach(XMLUnit xmlUnit in xmlUnits)
		{
			LoadUnit(xmlUnit);
		}
		// 开始游戏
		// gameController.Run();
	}

	public void LoadMap(XMLMap map)
	{
		editorController.XNum = map.xNum;
		editorController.YNum = map.yNum;
		editorController.PlayerMoneyOrigin = map.money;
		editorController.LightIntensity = map.lightIntensity;
	}

	public void LoadUnit(XMLUnit xmlUnit)
	{
		// 复制一份物体
		GameObject objPrefab = resourceController.unitDictionary[xmlUnit.name];
		GameObject objClone = Instantiate(objPrefab);
		// 设置位置,player和方向信息
		Unit unit = objClone.GetComponent<Unit>();
		editorController.Put(xmlUnit.x, xmlUnit.y, unit);
		unit.Direction = xmlUnit.direction;
		// TODO: 更新血条？
		unit.player = (Player)xmlUnit.player;
		// 设置成编辑器创建
		unit.isEditorCreated = true;
	}
}
