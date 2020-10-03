using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

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

    // 静音图片
    public Sprite mute;

	// 解除静音图片
	public Sprite unmute;

    // 生命值Prefab
    public GameObject hpBarPrefab;

	protected void Start()
	{
		// 初始化unitDictionary
		gameObjDictionary = new Dictionary<string, GameObject>();
		Array.ForEach(editorObjects, obj => gameObjDictionary.Add(obj.name, obj));
		Array.ForEach(blockObjects, obj => gameObjDictionary.Add(obj.name, obj));
		Array.ForEach(ballObjects, obj => gameObjDictionary.Add(obj.name, obj));
		Array.ForEach(backgroundObjects, obj => gameObjDictionary.Add(obj.name, obj));

		// 初始化路径
		ModulePath = Application.dataPath + "/Modules/";
		GamePath = Application.dataPath + "/Games/";
		// 如果不存在文件夹，就创建
		if (!Directory.Exists(ModulePath))
		{
			Directory.CreateDirectory(ModulePath);
		}
		// 如果不存在文件夹，就创建
		if (!Directory.Exists(GamePath))
		{
			Directory.CreateDirectory(GamePath);
		}
	}
}
