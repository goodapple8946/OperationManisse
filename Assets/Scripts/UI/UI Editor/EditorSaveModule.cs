using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

using static Controller;

/// <summary>
/// 与EditorSave相比只改变了SaveFile()方法和默认文本内容
/// </summary>
public class EditorSaveModule : MonoBehaviour
{
	InputField inputField;

	public void Awake()
	{
		inputField = GetComponent<InputField>();
		inputField.onValueChanged.AddListener(filename => SaveFile2FS(filename));
		// 初始文本
		inputField.SetTextWithoutNotify("Save As Module");
	}

	/// <summary>
	/// 转换成xml, 保存filename到文件系统,如果重名让编辑者选择
	/// </summary>
	void SaveFile2FS(string filename)
	{
		// 如果不存在文件夹，就创建
		string dirPath = Application.dataPath + "/Modules/";
		if (!Directory.Exists(dirPath))
		{
			Directory.CreateDirectory(dirPath);
		}

		string path = System.IO.Path.Combine(dirPath, filename + ".xml");
		// 文件系统存在重名文件
		if (File.Exists(path))
		{
			bool ok = EditorUtility.DisplayDialog("",
				"Are you sure you want to replace existing file?", "ok", "cancel");
			if (ok)
			{
				SaveFile(path);
			}
		}
		else
		{
			SaveFile(path);
		}
		// 更新文本
		inputField.SetTextWithoutNotify("Save As Module");
	}

	/// <summary>
	/// 转换成xml保存至文件中
	/// </summary>
	private void SaveFile(string path)
	{
		CheckEditorResult(editorController.Grid);
		
		Serializer.SerializeModule(editorController, path);
	}

	// 编辑结果的检测
	[System.Diagnostics.Conditional("DEBUG")]
	private void CheckEditorResult(Unit[,] Grid)
	{
		foreach (Unit unit in Grid)
		{
			System.Diagnostics.Debug.Assert(
				unit != null && unit.gameObject != null, "unit绑定的gameObject已被销毁");
		}
	}
}
