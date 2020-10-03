using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using static Controller;

/// <summary>
/// 辅助save和load的工具类
/// </summary>
static class Serializer
{
	/// <summary>
	/// 根据参数构造Game并序列化
	/// </summary>
	public static void SerializeGame(
		EditorController editorController, List<Background> backgrounds, 
		List<Unit> units, List<string> goodsVisable, string path)
	{
		XMLMap map = EditorController2XML(editorController);

		
		List<XMLBackground> xmlBackgrounds = new List<XMLBackground>();
		foreach (Background background in backgrounds)
		{
			XMLBackground xmlBackground = Background2XML(background);
			xmlBackgrounds.Add(xmlBackground);
		}

		List<XMLUnit> xmlUnits = new List<XMLUnit>();
		foreach (Unit unit in units)
		{
			XMLUnit xmlUnit = Unit2XML(unit);
			xmlUnits.Add(xmlUnit);
		}

		XMLGame game = new XMLGame(map, xmlBackgrounds, xmlUnits, goodsVisable);
		Serialize(game, path);
	}

	/// <summary>
	/// 根据参数构造Module并序列化
	/// </summary>
	public static void SerializeModule(
		EditorController editorController, string path)
	{

		XMLUnit[][] Grid = ToXMLUnitGrid(editorController.Grid);
		XMLModule module = new XMLModule(editorController.XNum, editorController.YNum, Grid);
		Serialize(module, path);
	}

	/// <summary>
	/// 将xml文件转换成T类型的对象, 可能返回XmlException表示解析错误
	/// </summary>
	public static T Deserialized<T>(string path)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(T));
		StreamReader reader = new StreamReader(path);
		T deserialized = (T)serializer.Deserialize(reader.BaseStream);
		reader.Close();
		return deserialized;
	}

	/// <summary>
	/// 将obj的所有public字段xml化，添加到path文件
	/// </summary>
	private static void Serialize(System.Object obj, string path)
	{
		XmlSerializer serializer = new XmlSerializer(obj.GetType());
		StreamWriter writer = new StreamWriter(path);
		serializer.Serialize(writer.BaseStream, obj);
		writer.Close();
	}

	// 将dimentional grid转换成jagged array
	private static XMLUnit[][] ToXMLUnitGrid(Unit[,] Grid)
	{
		// 初始化
		XMLUnit[][] xmlGrid = new XMLUnit[Grid.GetLength(0)][];
		for (int i = 0; i < Grid.GetLength(1); i++)
		{
			xmlGrid[i] = new XMLUnit[Grid.GetLength(1)];
		}
		// 转换
		for (int i = 0; i < Grid.GetLength(0); i++)
		{
			for (int j = 0; j < Grid.GetLength(1); j++)
			{
				xmlGrid[i][j] = Unit2XML(Grid[i, j]);
			}
		}
		return xmlGrid;
	}

	private static XMLBackground Background2XML(Background background)
	{
		string name = background.gameObject.name;
		Vector3 position = background.transform.position;
		Vector3 localScale = background.transform.localScale;
		return new XMLBackground(name, position, localScale);
	}

	/// <summary>
	/// 将Unit映射成XMLUnit
	/// </summary>
	private static XMLUnit Unit2XML(Unit unit)
	{
		String name = unit.gameObject.name;
		int player = (int)unit.player;
		int x = unit.gridX;
		int y = unit.gridY;
		int direction = unit.direction;
		int layer = unit.gameObject.layer;
		return new XMLUnit(name, player, x, y, direction, layer);
	}

	/// <summary>
	/// 将EditorController映射成XMLMap
	/// </summary>
	private static XMLMap EditorController2XML(EditorController editorController)
	{
		int xNum = editorController.XNum;
		int yNUm = editorController.YNum;
		int money = editorController.PlayerMoneyOrigin;
		float lightIntensity = editorController.LightIntensity;
		return new XMLMap(xNum, yNUm, money, lightIntensity);
	}


}

/// <summary>
/// XMLGame := xmlMap + xmlBackgrounds + xmlUnits + 可用商品名称数组 
/// </summary>
public class XMLGame
{
	public XMLMap xmlMap;
	public List<XMLBackground> xmlBackgrounds;
	public List<XMLUnit> xmlUnits;
	public List<String> goodsVisable;

	// 默认无参构造函数
	public XMLGame() { }

	public XMLGame(XMLMap xmlMap, List<XMLBackground> xmlBackgrounds, List<XMLUnit> xmlUnits, List<String> goodsVisable)
	{
		this.xmlMap = xmlMap;
		this.xmlBackgrounds = xmlBackgrounds;
		this.xmlUnits = xmlUnits;
		this.goodsVisable = goodsVisable;
	}
}

/// <summary>
/// XMLGame := 网格大小 + xmlUnits
/// </summary>
public class XMLModule
{
	public int xNum;
	public int yNum;

	//[XmlIgnore]
	//private Unit[,] grid;

	public XMLUnit[][] Grid;

	// 默认无参构造函数
	public XMLModule() { }

	public XMLModule(int xNum, int yNum, XMLUnit[][] Grid)
	{
		this.xNum = xNum;
		this.yNum = yNum;
		this.Grid = Grid;
	}
}


public class XMLBackground
{
	public string name;
	public Vector3 position;
	public Vector3 localScale;

	// 默认无参构造函数
	public XMLBackground() { }

	public XMLBackground(string name, Vector3 position, Vector3 localScale)
	{
		this.name = name;
		this.position = position;
		this.localScale = localScale;
	}
}

public class XMLUnit
{
	// ResourceController 的 prefab name 
	public String name;
	public int player;
	public int x;
	public int y;
	public int direction;
	public int layer;

	// 默认无参构造函数
	public XMLUnit() { }

	public XMLUnit(string name, int player, int x, int y, int direction, int layer)
	{
		this.name = name;
		this.player = player;
		this.x = x;
		this.y = y;
		this.direction = direction;
		this.layer = layer;
	}
}

public class XMLMap
{
	public int xNum;
	public int yNum;
	public int money;
	public float lightIntensity;

	// 默认无参构造函数
	public XMLMap() { }

	public XMLMap(int xNum, int yNum, int money, float lightIntensity)
	{
		this.xNum = xNum;
		this.yNum = yNum;
		this.money = money;
		this.lightIntensity = lightIntensity;
	}
}
