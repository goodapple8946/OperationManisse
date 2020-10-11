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
			// 原始状态
			switch (gamePhase)
			{
				case GamePhase.Editor:
					if(newPhase == GamePhase.Preparation)
					{
						LeavePhaseEditor();
						EnterPhasePreparation();
					}
					else if (newPhase == GamePhase.Preparation)
					{
						LeavePhaseEditor();
						EnterPhasePlaying();
					}
					break;

				case GamePhase.Preparation:
					if (newPhase == GamePhase.Editor)
					{
						EnterPhaseEditor();
					}
					else if (newPhase == GamePhase.Preparation)
					{
						LoadUnitsOrigin();
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

	// 所有物体的根节点
	/// <summary>preparation和playing阶段,场景中所有Unit</summary>
	[HideInInspector] public GameObject unitObjects;
    [HideInInspector] public GameObject missileObjects;
    [HideInInspector] public GameObject hpBarObjects;
    [HideInInspector] public GameObject backgroundObjects;
    private GameObject unitObjectsSaved;
    private GameObject unitObjectsOrigin;

	// 编辑器部分的UI
    private GameObject uiEditor;
	// 游戏部分的UI
    private GameObject uiGame;

    /**
     * 生命周期函数
     */
    public void Awake()
    {
        uiEditor = GameObject.Find("UI Canvas/UI Editor");
        uiGame = GameObject.Find("UI Canvas/UI Game");

        Init();
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
		// 右侧的Editor面板
        uiEditor.SetActive(true);
		// 上方的Button
        uiGame.SetActive(false);
		editorController.EnterPhaseEditor();
		shopController.UpdateShop();
        LoadUnitsOrigin();
    }

    // 离开Editor阶段
    void LeavePhaseEditor()
    {
        uiEditor.SetActive(false);
        uiGame.SetActive(true);
        SaveUnitsOrigin();
        SaveUnits();
    }

    // 进入Preparation阶段
    void EnterPhasePreparation()
    {
        editorController.MainGrid.SetShow(true);
        uiGame.GetComponent<UIGame>().UpdateActive();
        shopController.UpdateShop();
        LoadUnits();
        victoryController.Init();
    }

	// 进入Playing阶段
	void LeavePhasePlaying()
	{
		ClearMissile();
	}

	// 进入Playing阶段
	void EnterPhasePlaying()
	{
		editorController.MainGrid.SetShow(false);
		uiGame.GetComponent<UIGame>().UpdateActive();
		SaveUnits();

		// 连接所有Block
		editorController.MainGrid.LinkBlocks();

		// 向所有物体发送GameStart信号
		unitObjects.BroadcastMessage("GameStart");
	}

    // 进入Victory阶段
    void EnterVictory()
    {
        uiGame.GetComponent<UIGame>().UpdateActive();

        AudioSource.PlayClipAtPoint(resourceController.audioVictory, Camera.main.transform.position);
    }

    // 返回主菜单
    void EnterMenu()
    {
        SceneManager.LoadScene("Level Panel");
    }

    /**
     * 载入与保存
     */

    // 初始化
    public void Init()
    {
        if (unitObjects)
        {
            Destroy(unitObjects);
        }
        if (missileObjects)
        {
            Destroy(missileObjects);
        }
        if (hpBarObjects)
        {
            Destroy(hpBarObjects);
        }
        unitObjects = new GameObject("Unit Objects");
        missileObjects = new GameObject("Missile Objects");
        hpBarObjects = new GameObject("HP Bar Objects");
        backgroundObjects = new GameObject("Background Objects");
    }

    // 保存物体
    void SaveUnits()
    {
        if (unitObjectsSaved != null)
        {
            Destroy(unitObjectsSaved);
        }
        unitObjectsSaved = Instantiate(unitObjects);
        unitObjectsSaved.name = "Unit Objects Saved";
        unitObjectsSaved.SetActive(false);
    }

    // 保存最初的物体
    void SaveUnitsOrigin()
    {
        if (unitObjectsOrigin != null)
        {
            Destroy(unitObjectsOrigin);
        }
        unitObjectsOrigin = Instantiate(unitObjects);
        unitObjectsOrigin.name = "Unit Objects Origin";
        unitObjectsOrigin.SetActive(false);
    }

	// 载入物体
	// 在playing阶段中止时，读取preparation阶段的物体
	void LoadUnits()
	{
		if (unitObjects != null)
		{
			Destroy(unitObjects);
		}
		unitObjects = Instantiate(unitObjectsSaved);
		unitObjects.name = "Unit Objects";
		unitObjects.SetActive(true);

		editorController.MainGrid = new Grid(
			editorController.XNum, editorController.YNum,
			EditorController.MAINGRID_POS, GetUnits());

		cameraController.SetView(
			editorController.MainGrid.OriginPos,
			editorController.MainGrid.GetRightTopPos());
	}

	// 载入最初的物体
	// 在playing阶段中止时，读取editor阶段的物体
	void LoadUnitsOrigin()
    {
        if (unitObjects != null)
        {
            Destroy(unitObjects);
        }
        if (unitObjectsSaved != null)
        {
            Destroy(unitObjectsSaved);
        }
        unitObjects = Instantiate(unitObjectsOrigin);
        unitObjects.name = "Unit Objects";
        unitObjects.SetActive(true);
        SaveUnits();
        editorController.InitPlayerMoney();

		editorController.MainGrid= new Grid(
			editorController.XNum, editorController.YNum,
			EditorController.MAINGRID_POS, GetUnits());

		cameraController.SetView(
			editorController.MainGrid.OriginPos,
			editorController.MainGrid.GetRightTopPos());
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
        return unitObjects.GetComponentsInChildren<Unit>();
    }

    /// <summary>
    /// 获取场景中的Unit，通过Player
    /// </summary>
    public Unit[] GetUnits(Player player)
    {
        ArrayList arr = new ArrayList();
        Unit[] units = unitObjects.GetComponentsInChildren<Unit>();
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
        Unit[] units = unitObjects.GetComponentsInChildren<Unit>();
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
        Unit[] units = unitObjects.GetComponentsInChildren<Unit>();
        foreach (Unit unit in units)
        {
            if (unit.player == player && unit.tag == tag)
            {
                arr.Add(unit);
            }
        }
        return (Unit[])arr.ToArray(typeof(Unit));
    }

    /**
     * Debug
     */

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
