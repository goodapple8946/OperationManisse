using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
	// 所有单位的prefab
	public GameObject[] editorObjects;
	public GameObject[] blockObjects;
	public GameObject[] ballObjects;
	// 根据上面的生成的dictionary
	public Dictionary<string, GameObject> unitDictionary;

    // Block连接时的粒子预设
    public GameObject particleLinkPrefab;
    
    // Block连接时的音效
    public AudioClip[] audiosLink;

    // 删除时的音效
    public AudioClip[] audiosDelete;

    // 向网格中放置时的音效
    public AudioClip[] audiosPut;

    // 静音图片
    public Sprite mute;

	// 解除静音图片
	public Sprite unmute;

    // 生命值Prefab
    public GameObject hpBarPrefab;

	protected void Start()
	{
		// 初始化unitDictionary
		unitDictionary = new Dictionary<string, GameObject>();
		foreach(GameObject obj in editorObjects)
		{
			unitDictionary.Add(obj.name, obj);
		}
		foreach (GameObject obj in blockObjects)
		{
			unitDictionary.Add(obj.name, obj);
		}
		foreach (GameObject obj in ballObjects)
		{
			unitDictionary.Add(obj.name, obj);
		}
	}
}
