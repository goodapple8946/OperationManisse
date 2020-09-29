using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;
using static Controller;

/// <summary>
/// 辅助save和load的工具类
/// </summary>
public static class Serializer
{
	/// <summary>
	/// 根据参数构造Game并序列化
	/// </summary>
	public static void Serialize(
		EditorController editorController, List<Unit> units, 
		List<string> goodsVisable, string path)
	{
		XMLMap map = EditorController2XMLMap(editorController);

		List<XMLUnit> xmlUnits = new List<XMLUnit>();
		foreach (Unit unit in units)
		{
			XMLUnit xmlUnit = Unit2XML(unit);
			xmlUnits.Add(xmlUnit);
		}

		XMLGame game = new XMLGame(map, xmlUnits, goodsVisable);
		Serialize(game, path);
	}

	/// <summary>
	/// 将xml文件转换成Game
	/// </summary>
	public static XMLGame Deserialized(string path)
	{
		return Deserialized<XMLGame>(path);
	}

	/// <summary>
	/// 将xml文件转换成T类型的对象
	/// </summary>
	private static T Deserialized<T>(string path)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(T));
		StreamReader reader = new StreamReader(path);
		T deserialized = (T)serializer.Deserialize(reader.BaseStream);
		reader.Close();
		return deserialized;
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
	private static XMLMap EditorController2XMLMap(EditorController editorController)
	{
		int xNum = editorController.XNum;
		int yNUm = editorController.YNum;
		int money = editorController.PlayerMoneyOrigin;
		float lightIntensity = editorController.LightIntensity;
		return new XMLMap(xNum, yNUm, money, lightIntensity);
	}

	/// <summary>
	/// 将item的所有public字段添加到path文件的尾部
	/// </summary>
	private static void Serialize(System.Object item, string path)
	{
		XmlSerializer serializer = new XmlSerializer(item.GetType());
		StreamWriter writer = new StreamWriter(path);
		serializer.Serialize(writer.BaseStream, item);
		writer.Close();
	}
}

public class XMLGame
{
	public XMLMap xmlMap;
	public List<XMLUnit> xmlUnits;
	public List<String> goodsVisable;

	// 默认无参构造函数
	public XMLGame() { }

	public XMLGame(XMLMap xmlMap, List<XMLUnit> xmlUnits, List<String> goodsVisable)
	{
		this.xmlMap = xmlMap;
		this.xmlUnits = xmlUnits;
		this.goodsVisable = goodsVisable;
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
