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
					// 清空处理的文件名
					fileSelected = "";
					break;
			}
		}
	}

	// 当前选中的文件无后缀名称
	[HideInInspector] public static string fileSelected;

	// 唯一的实例
	[HideInInspector] public static FileViewer instance;

	private void Awake()
	{
		instance = 
			GameObject.Find("UI Editor").GetComponentInChildren<FileViewer>();
		Debug.Assert(instance != null);

		fileSelected = "";
		ViewerState = State.None;
	}

	private void Update()
	{	
		// 没有要处理的文件
		if (fileSelected == "")
		{
			return;
		}

		string fileGamePath = ResourceController.GamePath + fileSelected + ".xml";
		string fileModulePath = ResourceController.ModulePath + fileSelected + ".xml";

		switch (ViewerState)
		{
			case State.SaveGame:
				EditorSaveGame.SaveFile2FS(fileSelected);
				RedrawScrollView(ResourceController.GamePath, "Save");
				break;
			case State.SaveModule:
				EditorSaveModule.SaveFile2FS(fileSelected);
				RedrawScrollView(ResourceController.ModulePath, "Save");
				break;
			case State.LoadGame:
				EditorLoadGame.LoadGameFromFS(fileGamePath);
				break;
			case State.LoadModule:
				EditorLoadModule.LoadModuleFromFS(fileModulePath);
				break;
			case State.DeleteGame:
				DeleteFileOnFS(fileGamePath);
				RedrawScrollView(ResourceController.GamePath, "Delete");
				break;
			case State.DeleteModule:
				DeleteFileOnFS(fileModulePath);
				RedrawScrollView(ResourceController.ModulePath, "Delete");
				break;
		}
		// 清空当前处理的文件
		fileSelected = "";
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

	// 重新绘制某路径下
	private static void RedrawScrollView(string path, string title)
	{
		List<GameObject> objs = new List<GameObject>();

		// 添加文字标题
		GameObject titleObj = Instantiate(instance.scrollViewText);
		titleObj.GetComponent<Text>().text = title;
		objs.Add(titleObj);

		if (title == "Save")
		{
			objs.Add(Instantiate(instance.newFilePrefab));
		}

		objs.AddRange(GetFileObjectsByPath(path));

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
		objs.Add(Instantiate(instance.backPrefab));
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
