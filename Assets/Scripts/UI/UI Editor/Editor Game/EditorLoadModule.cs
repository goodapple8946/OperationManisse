using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

// 类型定义
using Coord = System.Tuple<int, int>;

public class EditorLoadModule : MonoBehaviour
{
	private static List<GameObject> lastClones = new List<GameObject>();
	private Button button;

	private void Awake()
	{
		button = GetComponent<Button>();
		button.onClick.AddListener(() =>
		{
			if (editorController.moduleSelected != "")
			{
				string path = ResourceController.ModulePath + editorController.moduleSelected + ".xml";
				LoadModuleFromFS(path);
				editorController.moduleSelected = "";
			}
		});
	}

	/// <summary>
	/// 从文件系统中加载保存的xml文件
	/// </summary>
	public static void LoadModuleFromFS(string path)
	{
		try
		{
			// 玩家选择了文件,则加载游戏
			if (path != "")
			{
				XMLModule module = Serializer.Deserialized<XMLModule>(path);
				// EditorController进入Module模式，并且设置Module
				editorController.MouseModule = module;
			}
		}
		// xml文件错误,显示错误弹窗
		catch
		{
			ResourceController.DisplayDialog("", "Module File Error!", "ok");
		}
	}

	// 清除上一次绘制的物体
	public static void ClearLastDisplay()
	{
		foreach (GameObject clone in lastClones)
		{
			Destroy(clone);
		}
		lastClones.Clear();
	}

	/// <summary>
	// 检测是否能把中心放入,true:可以
	/// </summary>
	public static bool CanPlaceModuleCenter(XMLModule module, Coord worldCoord)
	{
		Coord center = module.GetCenter();
		Coord worldStart = EditorController.Minus(worldCoord, center);

		return CanPlace(module, worldStart);
	}
	
	/// <summary>
	///	将module的中心点放置在绘制(worldX, worldY), 如果coord不合法报错
	/// </summary>
	public static void DisplayModuleCenter(XMLModule module, Coord worldCoord)
	{
		Debug.Assert(CanPlaceModuleCenter(module, worldCoord), "Error worldCoord");

		Coord center = module.GetCenter();
		Coord worldStart = EditorController.Minus(worldCoord, center);
		
		DisplayModule(module, worldStart);
	}

	/// <summary>
	/// 加载module对象使其左下角在世界坐标(x,y)
	///	将模组中每一个物品放到世界网格对应格中
	/// 如果coord不合法报错
	/// </summary>
	public static void LoadModuleCenter(XMLModule module, Coord worldCoord)
	{
		Debug.Assert(CanPlaceModuleCenter(module, worldCoord), "Error worldCoord");

		Coord center = module.GetCenter();
		Coord worldStart = EditorController.Minus(worldCoord, center);

		Load(module, worldStart);
	}

	// 将module的左下角放置在绘制(worldStartX, worldStartY)
	private static void DisplayModule(XMLModule module, Coord worldCoord)
	{
		// 加载单位信息
		for (int moduleX = 0; moduleX < module.xNum; moduleX++)
		{
			for (int moduleY = 0; moduleY < module.yNum; moduleY++)
			{
				XMLUnit xmlUnit = module.Grid[moduleX, moduleY];
				if (xmlUnit != null)
				{
					int worldX = worldCoord.Item1 + moduleX;
					int worldY = worldCoord.Item2 + moduleY;
					// 修改xmlUnit的坐标
					xmlUnit.x = worldX;
					xmlUnit.y = worldY;

					Unit unit = EditorLoad.XML2Unit(xmlUnit);
					GameObject clone = CorpseFactory.CreateTransparentGraphicClone(unit.gameObject);

					lastClones.Add(clone);
					Destroy(unit.gameObject);
				}
			}
		}
	}

	// 加载module对象使其左下角在世界坐标(x,y)
	// 将模组中每一个物品放到世界网格对应格中
	private static void Load(XMLModule module, Coord worldCoord)
	{
		// 加载单位信息
		for (int moduleX = 0; moduleX < module.xNum; moduleX++)
		{
			for (int moduleY = 0; moduleY < module.yNum; moduleY++)
			{
				// 如果xmlUnit存在
				XMLUnit xmlUnit = module.Grid[moduleX, moduleY];
				if (xmlUnit != null)
				{
					int worldX = worldCoord.Item1 + moduleX;
					int worldY = worldCoord.Item2 + moduleY;
					// 修改xmlUnit的坐标到Editor坐标网并加载
					xmlUnit.x = worldX;
					xmlUnit.y = worldY;
					EditorLoad.Load(xmlUnit);
				}
			}
		}
	}

	// 检测是否能放入,true:可以
	private static bool CanPlace(XMLModule module, Coord worldCoord)
	{
		// module内的相对坐标区域
		for (int moduleX = 0; moduleX < module.xNum; moduleX++)
		{
			for (int moduleY = 0; moduleY < module.yNum; moduleY++)
			{
				int worldX = worldCoord.Item1 + moduleX;
				int worldY = worldCoord.Item2 + moduleY;

				// 模组中的某格存在物品 && 世界坐标对应格超出或存在物品
				bool mappingGridInvalid = (!editorController.IsLegalCoord(worldX, worldY)
						|| editorController.Grid[worldX, worldY] != null);
				if (module.Grid[moduleX, moduleY] != null && mappingGridInvalid)
				{
					return false;
				}
			}
		}
		return true;
	}
}
