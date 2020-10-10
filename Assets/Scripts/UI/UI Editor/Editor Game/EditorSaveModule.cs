using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

// 类型定义
using Coord = System.Tuple<int, int>;

/// <summary>
/// 与EditorSave相比只改变了SaveFile()方法
/// </summary>
public class EditorSaveModule : MonoBehaviour
{
	Button button;

	public void Awake()
	{
		button = GetComponent<Button>();
		button.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
		{
			FileViewer.ViewerState = FileViewer.State.SaveModule;
		}));
	}

	/// <summary>
	/// 转换成xml, 保存filename到文件系统,如果重名让编辑者选择
	/// </summary>
	public static void SaveModule2FS(string filename)
	{
        try
        {
            string path = System.IO.Path.Combine(ResourceController.ModulePath, filename);
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
	/// 转换成xml保存至文件中
	/// </summary>
	private static void SaveFile(string path)
	{
		CheckEditorResult(editorController.Grid);
		
		XMLModule module = ObtainModule(editorController);
		Serializer.SerializeModule(module, path);
	}

	// 编辑结果的检测
	[System.Diagnostics.Conditional("DEBUG")]
	private static void CheckEditorResult(Unit[,] Grid)
	{
		foreach (Unit unit in Grid)
		{
			if(unit != null)
			{
				System.Diagnostics.Debug.Assert(
					unit.gameObject != null, "unit绑定的gameObject已被销毁");
			}
		}
	}

	// 除去空行，对单位坐标变换后保存
	public static XMLModule ObtainModule(EditorController editorController)
	{
		Coord lbCoord = GetLeftBottom();
		Coord rtCoord = GetRightTop();
		List<XMLUnit> xmlUnits = new List<XMLUnit>();
		for (int i = lbCoord.Item1; i < rtCoord.Item1; i++)
		{
			for (int j = lbCoord.Item2; j < rtCoord.Item2; j++)
			{
				// 添加所有Grid的非空元素
				Unit unit = editorController.Grid[i, j];
				if (unit != null)
				{
					XMLUnit xmlUnit = Serializer.Unit2XML(unit);
					// 转换成新的local坐标
					xmlUnit.x -= lbCoord.Item1;
					xmlUnit.y -= lbCoord.Item2;
					xmlUnits.Add(xmlUnit);
				}
			}
		}

		XMLModule module = new XMLModule(
			rtCoord.Item1 - lbCoord.Item1, rtCoord.Item2 - lbCoord.Item2, xmlUnits);
		return module;
	}

	//--------- 对EditController的操作 ---------//
	// 闭区间
	// 返回[0,0] -> [xnum, ynum]之间
	private static Coord GetLeftBottom()
	{
		int x;
		int y;
		for (x = 0; (x < editorController.XNum) && (ColumnEmpty(x)); x++) { }
		for (y = 0; (y < editorController.YNum) && (RowEmpty(y)); y++) { }
		return new Coord(x, y);
	}

	// 网格右上角的格子，不在网格上
	// 开区间[left,bottom]到[top,right]为左闭右开区间
	// 返回[left,bottom] -> [xnum, ynum]之间
	private static Coord GetRightTop()
	{
		Coord lbCoord = GetLeftBottom();
		int x;
		int y;
		for (x = editorController.XNum; (x > lbCoord.Item1) && (ColumnEmpty(x - 1)); x--) { }
		for (y = editorController.YNum; (y > lbCoord.Item2) && (RowEmpty(y - 1)); y--) { }
		return new Coord(x, y);
	}

	private static bool RowEmpty(int fixedY)
	{
		for (int i = 0; i < editorController.XNum; i++)
		{
			Unit unit = editorController.Grid[i, fixedY];
			if (unit != null)
			{
				return false;
			}
		}
		return true;
		
	}

	private static bool ColumnEmpty(int fixedX)
	{
		for (int j = 0; j < editorController.YNum; j++)
		{
			Unit unit = editorController.Grid[fixedX, j];
			if (unit != null)
			{
				return false;
			}
		}
		return true;
	}

	// namespace::wzf
	// 除去空行，对单位坐标变换后保存
	//public static XMLModule SerializeModule(EditorController editorController)
	//{
	//	int left = editorController.GetGridCoordNonEmpty("Left");
	//	int right = editorController.GetGridCoordNonEmpty("Right");
	//	int bottom = editorController.GetGridCoordNonEmpty("Bottom");
	//	int top = editorController.GetGridCoordNonEmpty("Top");
	//	int width = Mathf.Max(right - left + 1, 0);
	//	int height = Mathf.Max(top - bottom + 1, 0);

	//	List<XMLUnit> xmlUnits = new List<XMLUnit>();
	//	foreach (Unit unit in editorController.Grid)
	//	{
	//		if (unit != null)
	//		{
	//			XMLUnit xmlUnit = Serializer.Unit2XML(unit);
	//			// 转换成新的local坐标
	//			xmlUnit.x -= left;
	//			xmlUnit.y -= bottom;
	//			xmlUnits.Add(xmlUnit);
	//		}
	//	}
	//	XMLModule module = new XMLModule(width, height, xmlUnits);
	//	return module;
	//}
}
