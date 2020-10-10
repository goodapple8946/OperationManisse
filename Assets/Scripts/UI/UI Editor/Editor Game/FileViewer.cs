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
					RedrawSaveScrollView(ResourceController.GamePath);
					break;
				case State.SaveModule:
					RedrawSaveScrollView(ResourceController.ModulePath);
					break;
				case State.LoadGame:
					RedrawScrollView(ResourceController.GamePath);
					break;
				case State.LoadModule:
					RedrawScrollView(ResourceController.ModulePath);
					break;
				case State.DeleteGame:
					RedrawScrollView(ResourceController.GamePath);
					break;
				case State.DeleteModule:
					RedrawScrollView(ResourceController.ModulePath);
					break;
				case State.None:
					// 初始化ViewerState,清空处理的文件名
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

		// 初始化ViewerState
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
				RedrawSaveScrollView(ResourceController.GamePath);
				break;
			case State.SaveModule:
				EditorSaveModule.SaveFile2FS(fileSelected);
				RedrawSaveScrollView(ResourceController.ModulePath);
				break;
			case State.LoadGame:
				EditorLoadGame.LoadGameFromFS(fileGamePath);
				break;
			case State.LoadModule:
				EditorLoadModule.LoadModuleFromFS(fileModulePath);
				break;
			case State.DeleteGame:
				DeleteFileOnFS(fileGamePath);
				RedrawScrollView(ResourceController.GamePath);
				break;
			case State.DeleteModule:
				DeleteFileOnFS(fileModulePath);
				RedrawScrollView(ResourceController.ModulePath);
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

	// 在scrollView的content上重新绘制files
	private static void RedrawSaveScrollView(string path)
	{
		List<GameObject> objs = GetFileObjectsByPath(path);
		objs.Add(Instantiate(resourceController.newFilePrefab));

		RedrawScrollView(objs);
	}

	// 重新绘制某路径下
	private static void RedrawScrollView(string path)
	{
		List<GameObject> objs = GetFileObjectsByPath(path);

		RedrawScrollView(objs);
	}

	// 获取某路径下所有文件fileObj
	private static List<GameObject> GetFileObjectsByPath(string path)
	{
		string[] filenames = ResourceController.GetFilesInDirectory(path);

		List<GameObject> objs = new List<GameObject>();
		foreach (string filename in filenames)
		{
			GameObject fileObj = Instantiate(resourceController.filePrefab);
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
		objs.Add(Instantiate(resourceController.backPrefab));
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
		float height = 50f;
		content.GetComponent<RectTransform>().sizeDelta
			= new Vector2(0, height * objs.Count);
		// 添加gameobject
		objs.ForEach(obj =>
		{
			obj.transform.SetParent(content, false);
		});
	}
}
