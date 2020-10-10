using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class FileViewer : MonoBehaviour
{
	public enum State { None, SaveGame, LoadGame, SaveModule, LoadModule, DeleteGame, DeleteModule }

	private static State state;

	// 新建文件（File）Prefab
	[SerializeField] private GameObject newFilePrefab;

	// 文件（File）Prefab
	[SerializeField] private GameObject filePrefab;

	// Back按钮 Prefab
	[SerializeField] private GameObject backPrefab;

	// 顶部文字 Prefab
	[SerializeField] private GameObject scrollViewText;

	[HideInInspector] public static State ViewerState
	{
		private get => state;
		// 更改面板的active状态
		set
		{
			state = value;
			// 设置active
			instance.gameObject.SetActive(state == State.None ? false : true);

			// 设置内容
			switch (state)
			{
				case State.SaveGame:
					RedrawScrollView(ResourceController.GamePath, "Save");
					break;
				case State.SaveModule:
					RedrawScrollView(ResourceController.ModulePath, "Save");
					break;
				case State.LoadGame:
					RedrawScrollView(ResourceController.GamePath, "Load");
					break;
				case State.LoadModule:
					RedrawScrollView(ResourceController.ModulePath, "Load");
					break;
				case State.DeleteGame:
					RedrawScrollView(ResourceController.GamePath, "Delete");
					break;
				case State.DeleteModule:
					RedrawScrollView(ResourceController.ModulePath, "Delete");
					break;
				case State.None:
					break;
			}
		}
	}

	// 唯一的实例
	[HideInInspector] public static FileViewer instance;

	private void Awake()
	{
		instance = 
			GameObject.Find("UI Editor").GetComponentInChildren<FileViewer>();
		Debug.Assert(instance != null);

		// 初始化ViewerState
		ViewerState = State.None;
	}

	/// <summary>
	/// 根据当前状态处理传入的文件
	/// filename 包含了文件全名后缀
	/// </summary>
	public static void DealFile(string filename)
	{
		Debug.Assert(filename != null && filename != "");
		
		switch (ViewerState)
		{
			case State.SaveGame:
				EditorSaveGame.SaveGame2FS(filename);
				RedrawScrollView(ResourceController.GamePath, "Save");
				break;
			case State.SaveModule:
				EditorSaveModule.SaveModule2FS(filename);
				RedrawScrollView(ResourceController.ModulePath, "Save");
				break;
			case State.LoadGame:
				EditorLoadGame.LoadGameFromFS(filename);
				break;
			case State.LoadModule:
				EditorLoadModule.LoadModuleFromFS(filename);
				break;
			case State.DeleteGame:
				DeleteFileOnFS(ResourceController.GamePath + filename);
				RedrawScrollView(ResourceController.GamePath, "Delete");
				break;
			case State.DeleteModule:
				DeleteFileOnFS(ResourceController.ModulePath + filename);
				RedrawScrollView(ResourceController.ModulePath, "Delete");
				break;
		}
	}


	private static void DeleteFileOnFS(string path)
	{
		try
		{
			File.Delete(path);
			resourceController.playAudio("Success");
		}
		catch (Exception e)
		{
			Debug.Log(e.Message);
			resourceController.playAudio("Error");
		}
	}

	// 重新绘制某路径下的文件,在顶部加上title
	private static void RedrawScrollView(string path, string title)
	{
		List<GameObject> objs = new List<GameObject>();
		// 添加文字标题
		GameObject titleObj = Instantiate(instance.scrollViewText);
		titleObj.GetComponent<Text>().text = title;
		objs.Add(titleObj);
		// 添加NewFile
		if (title == "Save")
		{
			objs.Add(Instantiate(instance.newFilePrefab));
		}
		// 添加文件obj
		objs.AddRange(GetFileObjectsByPath(path));
		// 添加Back Button
		objs.Add(Instantiate(instance.backPrefab));
		RedrawScrollView(objs);
	}

	// 获取某路径下所有文件fileObj
	private static List<GameObject> GetFileObjectsByPath(string path)
	{
		string[] filenames = ResourceController.GetFilesInDirectory(path);

		List<GameObject> objs = new List<GameObject>();
		foreach (string filename in filenames)
		{
			GameObject fileObj = Instantiate(instance.filePrefab);
			string[] str = filename.Split('/', '.');
			fileObj.GetComponentInChildren<Text>().text = str[str.Length - 2];
			objs.Add(fileObj);
		}
		return objs;
	}

	// 在instance的子物体content上,重新绘制
	private static void RedrawScrollView(List<GameObject> objs)
	{
		Transform viewport = instance.transform.Find("Viewport");
		Transform content = viewport.Find("Content");
		RedrawScrollView(content, objs);
	}

	// 在scrollView的content上重新绘制
	private static void RedrawScrollView(Transform content, List<GameObject> objs)
	{
		// 清除当前显示的文件
		for (int i = 0; i < content.childCount; i++)
		{
			Destroy(content.GetChild(i).gameObject);
		}

		// 设置新的高度
		float height = 0f;
		// 添加gameobject
		objs.ForEach(obj =>
		{
			obj.transform.SetParent(content, false);
			height += obj.GetComponent<RectTransform>().sizeDelta.y;
		});
		content.GetComponent<RectTransform>().sizeDelta
			= new Vector2(0, height);
	}
}
