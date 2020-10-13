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
	public static void SerializeGame(XMLGame game, string path)
	{
		Serialize(game, path);
	}

	/// <summary>
	/// 根据参数构造Module并序列化
	/// </summary>
	public static void SerializeModule(XMLModule module, string path)
	{
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
	
	public static XMLBackground Background2XML(Background background)
	{
		Debug.Assert(background.gameObject != null);
		string name = background.gameObject.name;
		Vector3 position = background.transform.position;
		Vector3 localScale = background.transform.localScale;
		return new XMLBackground(name, position, localScale);
	}

	public static XMLTerrainA Terrain2XML(TerrainA terrain)
	{
		Debug.Assert(terrain.gameObject != null);
		return new XMLTerrainA(terrain.gameObject.name,
			terrain.Width, terrain.Height, terrain.transform.position);
	}

	/// <summary>
	/// 将Unit映射成XMLUnit, unit不能为Null
	/// </summary>
	public static XMLUnit Unit2XML(Unit unit)
	{
		Debug.Assert(unit != null);
		String name = unit.gameObject.name;
		int player = (int)unit.player;
		int x = unit.coord.x;
		int y = unit.coord.y;
		int direction = unit.direction;
		Layer layer = (Layer)unit.gameObject.layer;
		return new XMLUnit(name, player, x, y, direction, layer);
	}

	/// <summary>
	/// 将Map信息映射成XMLMap
	/// </summary>
	public static XMLMap Map2XML(
		EditorController editorController, VictoryController victoryController)
	{
		Debug.Assert(editorController != null);
		int xNum = editorController.XNum;
		int yNUm = editorController.YNum;
		int money = editorController.PlayerMoneyOrigin;
		float lightIntensity = editorController.LightIntensity;
		Coord buildingCoord1 = editorController.BuildingCoord1;
		Coord buildingCoord2 = editorController.BuildingCoord2;
		VictoryCondition victoryCond = victoryController.victoryCondition;
		return new XMLMap(xNum, yNUm, money, lightIntensity,
			buildingCoord1, buildingCoord2, victoryCond);
	}


}

/// <summary>
/// XMLGame := xmlMap + xmlBackgrounds + xmlUnits + 可用商品名称数组 
/// </summary>
public class XMLGame
{
	public XMLMap xmlMap;
	public List<XMLBackground> xmlBackgrounds;
	public List<XMLTerrainA> xmlTerrains;
	public List<XMLUnit> xmlUnits;
	public List<String> goodsVisable;

	// 默认无参构造函数
	public XMLGame() { }

	public XMLGame(XMLMap xmlMap, List<XMLBackground> xmlBackgrounds,
		List<XMLTerrainA> xmlTerrains, List<XMLUnit> xmlUnits,
		List<String> goodsVisable)
	{
		this.xmlMap = xmlMap;
		this.xmlBackgrounds = xmlBackgrounds;
		this.xmlTerrains = xmlTerrains;
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

	public List<XMLUnit> xmlUnits;

	// 不需要序列化
	private XMLUnit[,] grid;
	[XmlIgnore]
	public XMLUnit[,] Grid
	{
		get
		{
			// 从xml加载后, 没有用xmlUnits初始化Grid则初始化
			if (grid == null)
			{
				InitGrid();
			}
			return grid;
		}
	}

	// 默认无参构造函数
	public XMLModule() { }

	public XMLModule(int xNum, int yNum, List<XMLUnit> xmlUnits)
	{
		this.xNum = xNum;
		this.yNum = yNum;
		this.xmlUnits = xmlUnits;
		InitGrid();
	}

	public Coord GetCenter()
    {
		return new Coord(xNum / 2, yNum / 2);
    }

	// 用List初始化Grid
	private void InitGrid()
	{
		grid = new XMLUnit[xNum, yNum];
		foreach (XMLUnit unit in xmlUnits)
		{
			Grid[unit.x, unit.y] = unit;
		}
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

public class XMLTerrainA
{
	public string name;
	public int width;
	public int height;
	public Vector3 position;

	// 默认无参构造函数
	public XMLTerrainA() { }

	public XMLTerrainA(string name, int width, int height, Vector3 position)
	{
		this.name = name;
		this.width = width;
		this.height = height;
		this.position = position;
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
	public Layer layer; 

	// 默认无参构造函数
	public XMLUnit() { }

	public XMLUnit(string name, int player, int x, int y, int direction, Layer layer)
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
	public Coord buildingCoord1;
	public Coord buildingCoord2;
	public VictoryCondition victoryCond;

	// 默认无参构造函数
	public XMLMap() { }

	public XMLMap(int xNum, int yNum, int money, float lightIntensity,
			Coord buildingCoord1, Coord buildingCoord2, VictoryCondition victoryCond)
	{
		this.xNum = xNum;
		this.yNum = yNum;
		this.money = money;
		this.lightIntensity = lightIntensity;
		this.buildingCoord1 = buildingCoord1;
		this.buildingCoord2 = buildingCoord2;
		this.victoryCond = victoryCond;
	}
}
