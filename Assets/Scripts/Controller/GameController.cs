using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static Controller;

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
						LeavePhaseEditor();
						EnterPhasePreparation();
					}
					break;

				case GamePhase.Preparation:
					if (newPhase == GamePhase.Editor)
					{
						EnterPhaseEditor();
					}
					else if (newPhase == GamePhase.Preparation)
					{
						Clone(unitObjs, unitObjsEditor, true);
						// 删除Player创建的
						Clone(unitObjsEditorAndPlayer, unitObjsEditor, false);

						editorController.RecreateMainGrid(GetUnits());
						editorController.InitPlayerMoney();
					}
					else if (newPhase == GamePhase.Playing)
					{
						EnterPhasePlaying();
					}
					break;

				case GamePhase.Playing:
					if (newPhase == GamePhase.Editor)
					{
						LeavePhasePlaying();
						EnterPhaseEditor();
					}
					else if (newPhase == GamePhase.Preparation)
					{
						LeavePhasePlaying();
						
						EnterPhasePreparation();
					}
					else if (newPhase == GamePhase.Victory)
					{
						EnterVictory();
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
	private GameObject unitObjsEditorAndPlayer;
	/// <summary> 编辑者放置的物体 </summary>
	private GameObject unitObjsEditor;

	[HideInInspector] public GameObject missileObjects;
    [HideInInspector] public GameObject hpBarObjects;
    [HideInInspector] public GameObject backgroundObjects;

	// 编辑器部分的UI
    private GameObject uiEditor;
	// 游戏部分的UI
    private GameObject uiGame;
    // 商店部分的UI
    private GameObject uiShop;

    /**
     * 生命周期函数
     */
    public void Awake()
    {
        uiEditor = GameObject.Find("UI Canvas/UI Editor");
        uiGame = GameObject.Find("UI Canvas/UI Game");
        uiShop = GameObject.Find("UI Canvas/UI Shop");
		
		// 创建Obj
		unitObjs = new GameObject("Unit Objects");
		unitObjsEditor = new GameObject("Unit Objects Editor");
		unitObjsEditorAndPlayer = new GameObject("Unit Objects EditorAndPlayer");
		missileObjects = new GameObject("Missile Objects");
		hpBarObjects = new GameObject("HP Bar Objects");
		backgroundObjects = new GameObject("Background Objects");
	}

	public void Start()
    {
        uiGame.SetActive(false);
    }

    void Update()
    {
        DebugGame();
    }

    void FixedUpdate()
    {

    }

    // 进入Editor阶段
    void EnterPhaseEditor()
    {
        uiEditor.SetActive(true);
        uiGame.SetActive(false);
        uiShop.SetActive(true);

        editorController.EnterPhaseEditor();
		// shopController EnterPhaseEditor
		shopController.UpdateShop(GamePhase.Editor);

		// 返回到Editor阶段，把保存的unitObjsEditor赋值给unitObjs
		Debug.Assert(unitObjsEditor.activeSelf == false);

		Clone(unitObjs, unitObjsEditor, true);
		// 删除Player创建的
		Clone(unitObjsEditorAndPlayer, unitObjsEditor, false);

		editorController.InitPlayerMoney();
		editorController.RecreateMainGrid(GetUnits());
	}

    // 离开Editor阶段
    void LeavePhaseEditor()
    {
        editorController.LeavePhaseEditor();
	
        uiEditor.SetActive(false);
        uiGame.SetActive(true);

		// 把舞台上的保存到Editor
		Clone(unitObjsEditor, unitObjs, false);
	}

    // 进入Preparation阶段
    void EnterPhasePreparation()
    {
        uiGame.GetComponent<UIGame>().UpdateActive(GamePhase.Preparation);
        uiShop.SetActive(true);

		// shopController EnterPhasePreparation
		shopController.UpdateShop(GamePhase.Preparation);
		
		victoryController.Init();
	
		Clone(unitObjs, unitObjsEditorAndPlayer, true);
		editorController.RecreateMainGrid(GetUnits());
	}

	// 离开Playing阶段
	void LeavePhasePlaying()
	{
		ClearMissile();
	}

	// 进入Playing阶段
	void EnterPhasePlaying()
	{
		uiGame.GetComponent<UIGame>().UpdateActive(GamePhase.Playing);
        uiShop.SetActive(false);

        editorController.MainGrid.SetShow(false);
        editorController.MainGrid.LinkBlocks();

		// 把当前舞台上的Editor和Player创建的东西保存
		Clone(unitObjsEditorAndPlayer, unitObjs, false);
        unitObjs.BroadcastMessage("GameStart");
    }

    // 进入Victory阶段
    void EnterVictory()
    {
        uiGame.GetComponent<UIGame>().UpdateActive(GamePhase.Victory);

        AudioSource.PlayClipAtPoint(resourceController.audioVictory, Camera.main.transform.position);
    }

    // 返回主菜单
    void EnterMenu()
    {
        SceneManager.LoadScene("Level Panel");
    }

	/// <summary>
	/// 把src赋值给Dest,并设置active
	/// </summary>
	static void Clone(GameObject dest, GameObject src, bool isActive)
	{
		string destName = dest.name;
		if (dest != null)
		{
			Destroy(dest);
		}
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
	

	//---------------- 工具函数 ---------------//

	/// <summary>
	/// 获取场景中的Unit 
	/// </summary>
	public Unit[] GetUnits()
    {
        return unitObjs.GetComponentsInChildren<Unit>();
    }

    /// <summary>
    /// 获取场景中的Unit，通过Player
    /// </summary>
    public Unit[] GetUnits(Player player)
    {
        ArrayList arr = new ArrayList();
        Unit[] units = unitObjs.GetComponentsInChildren<Unit>();
        foreach(Unit unit in units)
        {
            if (unit.player == player)
            {
                arr.Add(unit);
            }
        }
        return (Unit[])arr.ToArray(typeof(Unit));
    }

    /// <summary>
    /// 获取场景中的Unit，通过Tag
    /// </summary>
    public Unit[] GetUnits(string tag)
    {
        ArrayList arr = new ArrayList();
        Unit[] units = unitObjs.GetComponentsInChildren<Unit>();
        foreach (Unit unit in units)
        {
            if (unit.tag == tag)
            {
                arr.Add(unit);
            }
        }
        return (Unit[])arr.ToArray(typeof(Unit));
    }

    /// <summary>
    /// 获取场景中的Unit，通过Player和Tag
    /// </summary>
    public Unit[] GetUnits(Player player, string tag)
    {
        ArrayList arr = new ArrayList();
        Unit[] units = unitObjs.GetComponentsInChildren<Unit>();
        foreach (Unit unit in units)
        {
            if (unit.player == player && unit.tag == tag)
            {
                arr.Add(unit);
            }
        }
        return (Unit[])arr.ToArray(typeof(Unit));
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
