using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public enum Player { Neutral, Player, Enemy }
    public enum Layer { Default, TransparentFX, IgnoreRaycast, Water = 4, UI, PlayerBall = 8, PlayerBlock, PlayerMissile, EnemyBall, EnemyBlock, EnemyMissile, Goods, Ground }
    public enum GamePhase { Editor, Preparation, Playing, Victory }

    // 场景阶段
    public static GamePhase gamePhase;

	// 所有物体的根节点
	/// <summary>preparation和playing阶段,场景中所有Unit</summary>
	[HideInInspector] public GameObject unitObjects;
    [HideInInspector] public GameObject missileObjects;
    [HideInInspector] public GameObject hpBarObjects;
    private GameObject unitObjectsSaved;
    private GameObject unitObjectsOrigin;

    private EditorController editorController;
    private VictoryController victoryController;
    private ShopController shopController;

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
        editorController = GameObject.Find("Editor Controller").GetComponent<EditorController>();
        victoryController = GameObject.Find("Victory Controller").GetComponent<VictoryController>();
        shopController = GameObject.Find("UI Canvas/UI Shop").GetComponent<ShopController>();

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

    /**
     * UI按键响应
     */ 

    // Editor阶段，按下UI的Run键
    public void Run()
    {
        FromPhaseEditor();
        ToPhasePreparation();
    }

    // Preparation阶段，按下UI的Reset键
    public void Reset()
    {
        LoadUnitsOrigin();
    }

    // Preparation阶段，按下UI的Start键
    public void StartGame()
    {
        ToPhasePlaying();
    }

    // Playing阶段，按下UI的Stop键
    public void StopGame()
    {
        ToPhasePreparation();
    }

    // 任意阶段，按下UI的Menu键
    public void Menu()
    {
        ToMenu();
    }

    // 任意阶段，按下UI的EditorMode键
    public void EditorMode()
    {
        ToPhaseEditor();
    }

    /**
     * 游戏阶段切换
     */

    // 进入Editor阶段
    void ToPhaseEditor()
    {
        gamePhase = GamePhase.Editor;
        editorController.ShowGrids(true);
        uiEditor.SetActive(true);
        uiGame.SetActive(false);
        uiGame.GetComponent<UIGame>().UpdateActive();
        shopController.UpdateShop();
        LoadUnitsOrigin();
    }

    // 离开Editor阶段
    void FromPhaseEditor()
    {
        editorController.FinishEditor();
        uiEditor.SetActive(false);
        uiGame.SetActive(true);
        SaveUnitsOrigin();
        SaveUnits();
    }

    // 进入Preparation阶段
    void ToPhasePreparation()
    {
        gamePhase = GamePhase.Preparation;
        editorController.ShowGrids(true);
        uiGame.GetComponent<UIGame>().UpdateActive();
        shopController.UpdateShop();
        ClearMissile();
        LoadUnits();
    }

    // 进入Playing阶段
    void ToPhasePlaying()
    {
        gamePhase = GamePhase.Playing;
        editorController.ShowGrids(false);
        uiGame.GetComponent<UIGame>().UpdateActive();
        victoryController.Init();
        SaveUnits();

        // 连接所有Block
        editorController.LinkAllBlocksInGrid();

        // 向所有物体发送GameStart信号
        unitObjects.BroadcastMessage("GameStart");
    }

    // 返回主菜单
    void ToMenu()
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
    void LoadUnits()
    {
        if (unitObjects != null)
        {
            Destroy(unitObjects);
        }
        unitObjects = Instantiate(unitObjectsSaved);
        unitObjects.name = "Unit Objects";
        unitObjects.SetActive(true);
        editorController.UpdateGridWithAllUnits();
    }

    // 载入最初的物体
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
        editorController.UpdateGridWithAllUnits();
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
