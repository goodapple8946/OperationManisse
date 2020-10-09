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
					RedrawSaveScrollView();
					break;
				case State.SaveModule:
					RedrawSaveScrollView();
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
			}
		}
	}

	// 当前选中的文件
	[HideInInspector] public static string fileSelected = "";

	// 当前选中的模型
	[HideInInspector] public static string moduleSelected = "";

	// TODO:是否有必要？唯一的实例
	[HideInInspector] public static FileViewer instance;

	private void Awake()
	{
		instance = 
			GameObject.Find("UI Editor").GetComponentInChildren<FileViewer>();
		Debug.Assert(instance != null);

		ViewerState = State.None;
	}

	private void Update()
	{
		//if (fileSelected == "" && moduleSelected == "")
		//{
		//	return;
		//}

		//switch (ViewerState)
		//{
		//	case State.SaveGame:
		//		break;
		//	case State.SaveModule:
		//		break;
		//	case State.LoadGame:
		//		break;
		//	case State.LoadModule:
		//		break;
		//	case State.DeleteGame:
		//		break;
		//	case State.DeleteModule:
		//		break;
		//}
		
		switch (ViewerState)
		{
			case State.SaveGame:
				RedrawScrollView(transform, ResourceController.GamePath);
				EditorSaveGame.SaveFile2FS(fileSelected);
				fileSelected = "";

				break;
			case State.SaveModule:
				RedrawScrollView(transform, ResourceController.ModulePath);
				EditorSaveModule.SaveFile2FS(moduleSelected);
				moduleSelected = "";
				break;
			case State.LoadGame:
				RedrawScrollView(transform, ResourceController.GamePath);
				string path = ResourceController.GamePath + FileViewer.fileSelected + ".xml";
				EditorLoadGame.LoadModuleFromFS(path);
				FileViewer.fileSelected = "";
				break;
			case State.LoadModule:
				RedrawScrollView(transform, ResourceController.ModulePath);
				string modulePath = ResourceController.ModulePath + FileViewer.moduleSelected + ".xml";
				EditorLoadModule.LoadModuleFromFS(modulePath);
				moduleSelected = "";
				break;
			case State.DeleteGame:
				RedrawScrollView(transform, ResourceController.GamePath);
				string deletePath = ResourceController.GamePath
					+ fileSelected + ".xml";
				DeleteFileOnFS(deletePath);
				break;
			case State.DeleteModule:
				RedrawScrollView(transform, ResourceController.ModulePath);
				string deleteModulePath = ResourceController.ModulePath + FileViewer.moduleSelected + ".xml";
				DeleteFileOnFS(deleteModulePath);
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

	// 在instance的子物体content上,重新绘制path路径下的文件夹
	private static void RedrawScrollView(string path)
	{
		RedrawScrollView(instance.transform, path);
	}

	// 在transform的子物体content上,重新绘制path路径下的文件夹
	private static void RedrawScrollView(Transform transform, string path)
	{
		Transform viewport = transform.Find("Viewport");
		Transform content = viewport.Find("Content");

		string[] filenames = ResourceController.GetFilesInDirectory(path);
		RedrawScrollView(content, filenames);
	}

	// 在scrollView的content上重新绘制files
	private static void RedrawScrollView(Transform content, string[] filenames)
	{
		// 清除当前显示的文件
		for (int i = 0; i < content.childCount; i++)
		{
			Destroy(content.GetChild(i).gameObject);
		}

		float height = 70f;
		// 重新创建要显示的文件的Prefab
		Instantiate(resourceController.newFilePrefab, content);
		foreach (string filename in filenames)
		{
			GameObject fileObj = Instantiate(resourceController.filePrefab, content);
			string[] str = filename.Split('/', '.');
			fileObj.GetComponentInChildren<Text>().text = str[str.Length - 2];
			height += 70f;
		}
		content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, height);
	}

	// 在scrollView的content上重新绘制files
	private static void RedrawSaveScrollView()
	{
		Transform viewport = instance.transform.Find("Viewport");
		Transform content = viewport.Find("Content");

		// 清除当前显示的文件
		for (int i = 0; i < content.childCount; i++)
		{
			Destroy(content.GetChild(i).gameObject);
		}

		// 重新创建要显示的文件的Prefab
		Instantiate(resourceController.newFilePrefab, content);
		Instantiate(resourceController.newFilePrefab, content);
		Instantiate(resourceController.newFilePrefab, content);
		Instantiate(resourceController.newFilePrefab, content);
	}

}
