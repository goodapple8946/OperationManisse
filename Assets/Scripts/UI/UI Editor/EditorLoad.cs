using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Controller;

public class EditorLoad : MonoBehaviour
{
	// 被点击是调用LoadFile方法
	public void LoadFile()
	{
		// 清空当前网格
		editorController.XNum = 0;
		editorController.YNum = 0;

		// 根据文件生成Game
		string filename = Path.Combine(Application.dataPath, "1.xml");
		XMLGame game = Serializer.Deserialized(filename);

		// 加载地图信息
		XMLMap map = game.xmlMap;
		LoadMap(map);

		// 加载单位信息
		List<XMLUnit> xmlUnits = game.xmlUnits;
		foreach (XMLUnit xmlUnit in xmlUnits)
		{
			LoadUnit(xmlUnit);
		}

		List<string> goodsVisible = game.goodsVisable;
		shopController.SetShopObjectVisibility(goodsVisible);
		// 开始游戏
		// gameController.Run();
	}

	private void LoadMap(XMLMap map)
	{
		editorController.XNum = map.xNum;
		editorController.YNum = map.yNum;
		editorController.PlayerMoneyOrigin = map.money;
		editorController.LightIntensity = map.lightIntensity;
	}

	private void LoadUnit(XMLUnit xmlUnit)
	{
		// 复制一份物体
		GameObject objPrefab = resourceController.unitDictionary[xmlUnit.name];
		GameObject objClone = Instantiate(objPrefab);
        objClone.name = objPrefab.name; // 默认复制名称是GameObject Name (Clone)
		

		// 设置位置,网格信息和旋转,以及所在layer, player
		Unit unit = objClone.GetComponent<Unit>();
		editorController.Put(xmlUnit.x, xmlUnit.y, unit);
		// 计算存档与克隆出的方向之差,设置旋转角度
		int dirDifference = (xmlUnit.direction - unit.direction) + 4;
		unit.Rotate(dirDifference);
		// prefab的layer是Default需要根据player信息创建
		unit.gameObject.layer = xmlUnit.layer; 
		// TODO: 更新血条？
		unit.player = (Player)xmlUnit.player;

		// 设置成编辑器创建
		unit.isEditorCreated = true;
	}
}