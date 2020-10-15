using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static Controller;


/// <summary>
/// 与EditorSave相比只改变了SaveFile()方法
/// </summary>
public class EditorSaveModule : EditorUI
{
	Button button;

	public void Awake()
	{
		button = GetComponent<Button>();
		button.onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
		{
			FileViewer.ViewerState = FileViewer.State.SaveModule;
		}));

		tipTitle = "Save Module";
		tipContent = "  Save all units as a module.";
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
		CheckEditorResult(editorController.MainGrid);
		
		XMLModule module = ObtainModule(editorController);
		Serializer.SerializeModule(module, path);
	}

	// 除去空行，对单位坐标变换后保存
	public static XMLModule ObtainModule(EditorController editorController)
	{
		Coord lbCoord = GetLeftBottom();
		Coord rtCoord = GetRightTop();
		List<XMLUnit> xmlUnits = new List<XMLUnit>();
		for (int i = lbCoord.x; i < rtCoord.x; i++)
		{
			for (int j = lbCoord.y; j < rtCoord.y; j++)
			{
				// 添加所有Grid的非空元素
				Unit unit = editorController.MainGrid.Get(new Coord(i, j));
				if (unit != null)
				{
					XMLUnit xmlUnit = Serializer.Unit2XML(unit);
					// 转换成新的local坐标
					xmlUnit.x -= lbCoord.x;
					xmlUnit.y -= lbCoord.y;
					xmlUnits.Add(xmlUnit);
				}
			}
		}

		XMLModule module = new XMLModule(
			rtCoord.x - lbCoord.x, rtCoord.y - lbCoord.y, xmlUnits);
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
		for (x = editorController.XNum; (x > lbCoord.x) && (ColumnEmpty(x - 1)); x--) { }
		for (y = editorController.YNum; (y > lbCoord.y) && (RowEmpty(y - 1)); y--) { }
		return new Coord(x, y);
	}

	/// <summary>
	/// Y所在行为空
	/// </summary>
	private static bool RowEmpty(int Y)
	{
		for (int i = 0; i < editorController.XNum; i++)
		{
			Coord coord = new Coord(i, Y);
			Unit unit = editorController.MainGrid.Get(coord);
			if (unit != null)
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// X所在列为空
	/// </summary>
	private static bool ColumnEmpty(int X)
	{
		for (int j = 0; j < editorController.YNum; j++)
		{
			Coord coord = new Coord(X, j);
			Unit unit = editorController.MainGrid.Get(coord);
			if (unit != null)
			{
				return false;
			}
		}
		return true;
	}

	// 编辑结果的检测
	[System.Diagnostics.Conditional("DEBUG")]
	private static void CheckEditorResult(Grid grid)
	{
		foreach (Unit unit in grid.GetUnits())
		{
			if (unit != null)
			{
				Debug.Assert(unit.gameObject != null);
			}
		}
	}
}
