using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static Controller;
using System;
using System.Linq;

public class GameController : MonoBehaviour
{
	// 场景阶段
	private GamePhase gamePhase = GamePhase.Editor;
	// 切换状态
    public GamePhase GamePhase
	{
		get => gamePhase;
		set
		{
			// 新设置的状态
			GamePhase newPhase = value;
            Debug.Log(string.Format("GamePhase: {0} -> {1}.", gamePhase, newPhase));
			// 原始状态
			switch (gamePhase)
			{
				case GamePhase.Editor:
					if(newPhase == GamePhase.Preparation)
					{
						uiEditor.SetActive(false);
						uiGame.SetActive(true);
						uiGame.GetComponent<UIGame>().UpdateActive(GamePhase.Preparation);
						uiShop.SetActive(true);

						editorController.MainGrid.SetShow(false);
						editorController.LeavePhaseEditor();
						shopController.UpdateShop(GamePhase.Preparation);
						victoryController.Init();

						Clone(ref unitObjsEditor, unitObjs, false); // 把舞台上的保存到Editor
					}
					break;

				case GamePhase.Preparation:
					if (newPhase == GamePhase.Editor)
					{
						uiEditor.SetActive(true);
						uiGame.SetActive(false);

						editorController.MainGrid.SetShow(true);
						editorController.EnterPhaseEditor();
						shopController.UpdateShop(GamePhase.Editor);

						Clone(ref unitObjsEditorAndPlayer, unitObjsEditor, false); // 删除Player创建的
						Clone(ref unitObjs, unitObjsEditor, true);
						UpdateUnits();

						editorController.RecreateMainGrid(GetUnitsArr());
					}
					else if (newPhase == GamePhase.Preparation)
					{
						editorController.InitPlayerMoney();

						Clone(ref unitObjsEditorAndPlayer, unitObjsEditor, false); // 删除Player创建的
						Clone(ref unitObjs, unitObjsEditor, true);

						editorController.RecreateMainGrid(GetUnitsArr());
						editorController.MainGrid.SetShow(false);
					}
					else if (newPhase == GamePhase.Playing)
					{
						uiGame.GetComponent<UIGame>().UpdateActive(GamePhase.Playing);
						uiShop.SetActive(false);

						editorController.BuildingGrid.SetShow(false);
						editorController.MainGrid.LinkBlocks();

						// 把当前舞台上的Editor和Player创建的东西保存
						Clone(ref unitObjsEditorAndPlayer, unitObjs, false);

						unitObjs.BroadcastMessage("GameStart");
					}
					break;

				case GamePhase.Playing:
					if (newPhase == GamePhase.Editor)
					{
						ClearMissile();
						
						uiEditor.SetActive(true);
						uiGame.SetActive(false);
						uiShop.SetActive(true);

						editorController.MainGrid.SetShow(true);
						editorController.BuildingGrid.SetShow(true);
						editorController.EnterPhaseEditor();
						shopController.UpdateShop(GamePhase.Editor);

						Clone(ref unitObjsEditorAndPlayer, unitObjsEditor, false); // 删除Player创建的
						Clone(ref unitObjs, unitObjsEditor, true);

						editorController.RecreateMainGrid(unitsSet);
					}
					else if (newPhase == GamePhase.Preparation)
					{
						ClearMissile();

						uiGame.GetComponent<UIGame>().UpdateActive(GamePhase.Preparation);
						uiShop.SetActive(true);

						editorController.BuildingGrid.SetShow(true);
						shopController.UpdateShop(GamePhase.Preparation);
						victoryController.Init();

						Clone(ref unitObjs, unitObjsEditorAndPlayer, true);

						editorController.RecreateMainGrid(unitsSet);
						editorController.MainGrid.SetShow(false);
					}
					else if (newPhase == GamePhase.Victory)
					{
						uiGame.GetComponent<UIGame>().UpdateActive(GamePhase.Victory);

						// TODO: 胜利后弹出胜利窗口

						AudioSource.PlayClipAtPoint(resourceController.audioVictory, Camera.main.transform.position);
					}
					break;

				case GamePhase.Victory:
					
					break;
			}
			// 更新状态
			gamePhase = newPhase;
		}
	}

	/// <summary>preparation和playing阶段,场景中所有Unit</summary>
	// 所有物体的根节点
	/// <summary> 舞台上的物体 </summary>
	[HideInInspector] public GameObject unitObjs;
	/// <summary> 玩家放置的物体 </summary>
	[HideInInspector] public GameObject unitObjsEditorAndPlayer;
	/// <summary> 编辑者放置的物体 </summary>
	[HideInInspector] public GameObject unitObjsEditor;

	[HideInInspector] public GameObject missileObjects;
    [HideInInspector] public GameObject hpBarObjects;
    [HideInInspector] public GameObject backgroundObjects;
	[HideInInspector] public GameObject terrainObjects;

	// 编辑器部分的UI
	private GameObject uiEditor;
	// 游戏部分的UI
    private GameObject uiGame;
    // 商店部分的UI
    private GameObject uiShop;

	// 所有可见的单位
	public HashSet<Unit> unitsSet;

	/**
     * 生命周期函数
     */
	public void Awake()
    {
		unitsSet = new HashSet<Unit>();

		uiEditor = GameObject.Find("UI Canvas/UI Editor");
        uiGame   = GameObject.Find("UI Canvas/UI Game");
        uiShop   = GameObject.Find("UI Canvas/UI Shop");
		
		unitObjs                = new GameObject("Unit Objects");
		unitObjsEditor          = new GameObject("Unit Objects Editor");
		unitObjsEditorAndPlayer = new GameObject("Unit Objects EditorAndPlayer");
		missileObjects          = new GameObject("Missile Objects");
		hpBarObjects            = new GameObject("HP Bar Objects");
		backgroundObjects       = new GameObject("Background Objects");
		terrainObjects          = new GameObject("Terrain Objects");
	}

	public void Start()
	{
		uiGame.SetActive(false);
		editorController.StartInit();

		if (Controller.isGame)
        {
			GamePhase = GamePhase.Preparation;
        }
    }

    void Update()
    {
        DebugGame();
    }

    // 返回主菜单
    void EnterMenu()
    {
        SceneManager.LoadScene("Scene Menu");
    }

	/// <summary>
	/// 把src赋值给Dest,并设置active
	/// </summary>
	static void Clone(ref GameObject dest, GameObject src, bool isActive)
	{
		string destName = dest.name;
		Destroy(dest);
		dest = Instantiate(src);
		dest.SetActive(isActive);
		// 保存名字
		dest.name = destName;
	}

	// 清除投掷物
	void ClearMissile()
    {
        if (missileObjects != null)
        {
            Destroy(missileObjects);
        }
        missileObjects = new GameObject("Missile Objects");
    }

	// 更新可见单位集合
	void UpdateUnits()
    {
		unitsSet.Clear();
		unitsSet.Union(unitObjs.GetComponentsInChildren<Unit>());
	}

	public Unit[] GetUnitsArr()
    {
		Unit[] units = new Unit[unitsSet.Count];
		unitsSet.CopyTo(units);
		return units;
	}

	public List<Unit> GetUnitsList()
    {
		return unitsSet.ToList();
    }

	// Debug
	void DebugGame()
    {
        // 左Alt + 鼠标左键取鼠标位置
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftAlt))
        {
            Debug.Log(MouseController.MouseWorldPosition());
        }
    }
}
