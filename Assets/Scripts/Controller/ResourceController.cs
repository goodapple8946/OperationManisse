using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ResourceController : MonoBehaviour
{
	public static string ModulePath { get; private set; }
	public static string GamePath { get; private set; }

	// 所有单位的Prefab
	public GameObject[] editorObjects;
	public GameObject[] blockObjects;
	public GameObject[] ballObjects;

    // 背景的Prefab
    public GameObject[] backgroundObjects;

	// 根据上面的生成的dictionary
	public Dictionary<string, GameObject> gameObjDictionary;

    // Block连接时的粒子预设
    public GameObject particleLinkPrefab;
    
    // Block连接时的音效
    public AudioClip[] audiosLink;

    // 删除时的音效
    public AudioClip[] audiosDelete;

    // 向网格中放置时的音效
    public AudioClip[] audiosPut;

    // 胜利时的音效
    public AudioClip audioVictory;

	// 发生错误时的音效
	public AudioClip audioError;

    // 静音图片
    public Sprite mute;

	// 解除静音图片
	public Sprite unmute;

    // 生命值Prefab
    public GameObject hpBarPrefab;

	// 新建文件（File）Prefab
	public GameObject newFilePrefab;

	// 文件（File）Prefab
	public GameObject filePrefab;

	// 新建模块（Module）Prefab
	public GameObject newModulePrefab;

	// 模块（Module）Prefab
	public GameObject modulePrefab;

	protected void Awake()
	{
		// 初始化unitDictionary
		gameObjDictionary = new Dictionary<string, GameObject>();
		Array.ForEach(editorObjects, obj => gameObjDictionary.Add(obj.name, obj));
		Array.ForEach(blockObjects, obj => gameObjDictionary.Add(obj.name, obj));
		Array.ForEach(ballObjects, obj => gameObjDictionary.Add(obj.name, obj));
		Array.ForEach(backgroundObjects, obj => gameObjDictionary.Add(obj.name, obj));

		ModulePath = Application.persistentDataPath + "/Modules/";
		CheckAndCreatePath(ModulePath);
		GamePath = Application.persistentDataPath + "/Games/";
		CheckAndCreatePath(GamePath);
	}
	
	/// <summary>
	/// 如果不存在文件路径，就创建 
	/// </summary>
	private void CheckAndCreatePath(string path)
	{
		// 初始化路径
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
	}

	// -------------- 文件弹窗 -------------//

	/// <summary>
	/// 返回选择的文件路径，没有选择返回""
	/// </summary>
	public static string OpenFilePanel(string title, string directory, string extension)
	{
		// SetDefaultFilter设置展示文件后缀名例如.xml
		//FileBrowser.SetDefaultFilter("."+ extension);

		// 获取onSuccess的参数filePath, onSuccess作为delegate用参数返回值
		// FileBrowser.ShowLoadDialog(null, null, false, false, directory, title, "Open");

		//return FileBrowser.Result[0];

		return UnityEditor.EditorUtility.OpenFilePanel(title, directory, extension); 
	}

	/// <summary>
	/// 展示一个只有ok的dialog
	/// </summary>
	public static bool DisplayDialog(string title, string message, string ok)
	{
		return UnityEditor.EditorUtility.DisplayDialog(title, message, ok);
	}

	/// <summary>
	/// 展示一个有ok和cancel的dialog,true:点击ok
	/// </summary>
	public static bool DisplayDialog(string title, string message, string ok, string cancel)
	{
		
		return UnityEditor.EditorUtility.DisplayDialog(title, message, ok, cancel);
	}

	/// <summary>
	/// 获取某路径下所有XML文件
	/// 返回名称数组
	/// </summary>
	public static string[] GetFilesInDirectory(string path)
    {
		string[] files = Directory.GetFiles(path);
		List<string> arr = new List<string>();
		foreach (string file in files) 
        {
			if (file.Substring(file.Length - 4) == ".xml")
            {
				arr.Add(file);
            }
        }
		return arr.ToArray();
	}
}
