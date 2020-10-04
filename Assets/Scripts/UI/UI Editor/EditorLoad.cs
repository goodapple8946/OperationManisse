using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorLoad : MonoBehaviour
{

	private void Awake()
	{
		Button button = GetComponent<Button>();
		button.onClick.AddListener(LoadGameFromFS);
	}

	/// <summary>
	/// 从文件系统中加载保存的xml文件
	/// </summary>
	public static void LoadGameFromFS()
	{
		//try
		//{
			// 根据文件生成Game
			string path = ResourceController.OpenFilePanel(
				"Choose a map", ResourceController.GamePath, "xml");
			// 玩家选择了文件,则加载游戏
			if (path != "")
			{
				XMLGame game = Serializer.Deserialized<XMLGame>(path);
				Load(game);
			}
		//}
		//// xml文件错误,显示错误弹窗
		//catch (System.Exception e)
		//{
		//	ResourceController.DisplayDialog("", "Map File Error!", "ok");
		//}
	}

	// 加载XMLGame对象
	private static void Load(XMLGame game)
	{
		// 清空当前网格
		editorController.XNum = 0;
		editorController.YNum = 0;
		// 清空背景图片
		editorController.ClearBackground();

		// 加载地图信息
		XMLMap map = game.xmlMap;
		Load(map);

		// 加载背景图片
		List<XMLBackground> xmlBackgrounds = game.xmlBackgrounds;
		xmlBackgrounds.ForEach(Load);

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
	}

	/// <summary>
	/// 读取xmlBackground并克隆,保存到editorController里
	/// </summary>
	private static void Load(XMLBackground xmlBackground)
	{
		// 复制一份物体
		GameObject objPrefab = resourceController.gameObjDictionary[xmlBackground.name];
		GameObject objClone = Instantiate(objPrefab);
		objClone.name = objPrefab.name; // 默认复制名称是GameObject Name (Clone)

		// 设置位置和大小
		objClone.transform.position = xmlBackground.position;
		objClone.transform.localScale = xmlBackground.localScale;

		Background background = objClone.GetComponent<Background>();
		editorController.Put(background);
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
		unit.transform.position = editorController.CoordToPosition(xmlUnit.x, xmlUnit.y);

		// prefab的layer是Default，需要根据所属player信息创建
		unit.gameObject.layer = xmlUnit.layer;
		unit.player = (Player)xmlUnit.player; // TODO: 更新血条？

		// 设置成编辑器创建
		unit.isEditorCreated = true;
		return unit;
	}

	/// <summary>
	/// 读取xmlUnit并克隆,保存到editorController里
	/// </summary>
	public static void Load(XMLUnit xmlUnit)
	{
		Unit unit = XML2Unit(xmlUnit);
		// 设置unit的网格位置,和坐标
		editorController.Put(xmlUnit.x, xmlUnit.y, unit);
	}
}