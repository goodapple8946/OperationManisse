using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;
using static Controller;

public class EditorController : MonoBehaviour
{
	// 当前编辑模式
	[HideInInspector] private EditorMode editorMode;

	// 玩家放置物体的网格
	private Unit[,] grid;
	[HideInInspector] public Unit[,] Grid { get => grid; }

	// 编辑者放置的背景
	[HideInInspector] private HashSet<Background> backgrounds = new HashSet<Background>();
	[HideInInspector] public HashSet<Background> Backgrounds { get => backgrounds; }

	// Editor面板：网格尺寸x
	[HideInInspector] private int xNum = 8;

	// Editor面板：网格尺寸y
	[HideInInspector] private int yNum = 8;

	// Editor面板：编辑者当前使用的UnitOwner
	[HideInInspector] private Player playerOwner = Player.Neutral;

	// Editor面板：编辑者当前设置的玩家初始钱数
	[HideInInspector] private int playerMoneyOrigin = 0;

	// Editor面板：编辑者当前设置的全局光照
	[HideInInspector] private float lightIntensity = 1.0f;

    // Editor面饭：背景尺寸
    [HideInInspector] private float backgroundScale = 1f;

    // 摄像机的四个边的世界位置
    [HideInInspector] public float xMin;
	[HideInInspector] public float xMax;
	[HideInInspector] public float yMin;
	[HideInInspector] public float yMax;

	// 每个网格的大小
	private float gridSize = 0.6f;

	// 整个网格的左下角的坐标
	private Vector2 origin = Vector2.zero;

	// 根据方向获取坐标偏移
	private int[,] dir4 = { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 } };
	private int[,] dir8 = { { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 } };

	// Unit吸附的最大范围
	private float distanceAbsorption = 0.6f;

	// 当前鼠标持有的Unit
	[HideInInspector] public ClickableObject mouseObject;

    // 当前鼠标持有的Unit 或 上一次鼠标持有的Unit
    [HideInInspector] public ClickableObject mouseObjectLast;

    // 允许接收按住鼠标左键或右键
    // 只有EditorMode是Unit模式下，isClickHold才能为true
    [HideInInspector] private bool isClickHold;

	// 连续购买（是购买并安放的，而非移动网格中现有的）
	private bool buyContinuous = false;

	// 放置位置背景颜色深度
	[SerializeField] private float colorAlpha = 0.2f;

	// 玩家钱数
	[HideInInspector] public int playerMoney;

	// 显示HP
	[HideInInspector] private bool isShowingHP;

	[SerializeField] private GameObject square;

    #region Property Function
    public int XNum
    {
        get => xNum;
        set
        {
            xNum = value;
            editorContent.UpdateUIShowing();
            // 更新网格信息
            UpdateGridAfterResize();
        }
    }
    public int YNum
    {
        get => yNum;
        set
        {
            yNum = value;
            editorContent.UpdateUIShowing();
            // 更新网格信息
            UpdateGridAfterResize();
        }
    }
    public Player PlayerOwner
    {
        get => playerOwner;
        set
        {
            playerOwner = value;
            editorContent.UpdateUIShowing();
        }
    }
    public int PlayerMoneyOrigin
    {
        get => playerMoneyOrigin;
        set
        {
            playerMoneyOrigin = value;
            editorContent.UpdateUIShowing();
        }
    }
    public EditorMode EditorMode
    {
        get => editorMode;
        set
        {
            editorMode = value;
            editorContent.UpdateUIShowing();

            IsClickHold = false;
            shopController.UpdateShop();
            ClearMouseObject();
        }
    }
    public bool IsClickHold
    {
        get => isClickHold;
        set
        {
            isClickHold = value;
            editorContent.UpdateUIShowing();
        }
    }
	public float LightIntensity
    {
        get => lightIntensity;
        set
        {
            lightIntensity = value;
            GameObject.Find("Global Light").GetComponent<Light2D>().intensity = lightIntensity;
            editorContent.UpdateUIShowing();
        }
    }
    public float BackgroundScale
    {
        get => backgroundScale;
        set
        {
            backgroundScale = value;

            Background background = mouseObject is Background ? mouseObject as Background : mouseObjectLast is Background ? mouseObjectLast as Background : null;
            if (background != null)
            {
                float scale = editorContent.GetComponentInChildren<EditorScale>().GetComponent<Slider>().value;
                background.transform.localScale = new Vector2(scale, scale);
            }

            editorContent.UpdateUIShowing();
        }
    }
    public bool IsShowingHP
    {
        get => isShowingHP;
        set
        {
            isShowingHP = value;
            editorContent.UpdateUIShowing();
        }
    }
    #endregion

    // Editor内容显示的UI
    private EditorContent editorContent;

    private GameObject gridObjects;

	void Awake()
	{
        editorContent = GameObject.Find("UI Editor").GetComponentInChildren<EditorContent>();
    }

    void Start()
    {
        Init();
        CreateGridSprites();
        editorContent.UpdateUIShowing();
    }

    void Update()
    {
        // 指令
        Order();

        MyDebug();
    }

    void Init()
    {
        if (gridObjects != null)
        {
            Destroy(gridObjects);
        }
        // 初始化网格
        gridObjects = new GameObject("Grid Objects");
        grid = new Unit[XNum, YNum];

        // 初始化四个边坐标
        xMin = 0;
        yMin = 0;
        xMax = gridSize * XNum;
        yMax = gridSize * YNum;
    }

    void CreateGridSprites()
    {
        // 创建网格
        for (int x = 0; x < XNum; x++)
        {
            for (int y = 0; y < YNum; y++)
            {
                GameObject squareObj = Instantiate(square, gridObjects.transform);
                squareObj.transform.position = CoordToPosition(x, y);
                squareObj.GetComponent<SpriteRenderer>().sortingLayerName = "Area";

                if ((x + y) % 2 == 0)
                {
                    squareObj.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, colorAlpha / 2);
                }
                else
                {
                    squareObj.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, colorAlpha);
                }
            }
        }
    }

    // 鼠标左键点击
    public void LeftClick(ClickableObject clickableObject, bool hold = false)
    {
        // 如果没有开启连续放置模式，但是长按鼠标。则没有效果
        if (!isClickHold && hold)
        {
            return;
        }
        // Unit模式下点击Unit，此时mouseObject一定是Unit或null
        if (clickableObject is Unit && editorMode == EditorMode.Unit)
        {
            Unit unit = clickableObject as Unit;
            LeftClick(unit);
        }
        // Background模式下点击Background，此时mouseObject一定是Background或null
        else if (clickableObject is Background && editorMode == EditorMode.Background)
        {
            Background background = clickableObject as Background;
            LeftClick(background);
        }
    }
    // 鼠标左键点击Unit
    public void LeftClick(Unit unit)
    {
        // 满足阶段
        if (mouseObject != null)
        {
            if (mouseObject as Unit == unit)
            {
                // 安放鼠标上的物体到网格中
                Place(unit);
            }
        }
        else
        {
            // 移动网格中的Unit
            if (
                // 编辑阶段可以任意移动
                gameController.gamePhase == GamePhase.Editor ||
                // 准备阶段：不能移动其他玩家的Unit、不能移动编辑器创建的Unit
                (gameController.gamePhase == GamePhase.Preparation && unit.player == Player.Player && !unit.isEditorCreated))
            {
                buyContinuous = false;
                PlayerOwner = unit.player;
                Pick(unit);
            }
        }
    }
    // 鼠标左键点击Background
    public void LeftClick(Background background)
    {
        // 必须是编辑阶段
        if (gameController.gamePhase == GamePhase.Editor)
        {
            if (mouseObject != null)
            {
                if (mouseObject as Background == background)
                {
                    // 安放鼠标上的背景到背景集合
                    Place(background);
                }
            }
            else
            {
                // 移动背景集合中的背景
                buyContinuous = false;
                Pick(background);
            }
        }
    }
    // 拾起Unit
    public void Pick(Unit unit)
    {
        int x = unit.gridX;
        int y = unit.gridY;
        if (x != -1 && y != -1)
        {
            grid[x, y] = null;
            unit.gridX = unit.gridY = -1;
        }
        unit.SetSpriteLayer("Pick");
        mouseObject = unit;
        mouseObjectLast = unit;
    }
    // 拾起BackGround
    public void Pick(Background background)
    {
        background.SetSpriteLayer("Pick");
        mouseObject = background;
        mouseObjectLast = background;
        BackgroundScale = background.transform.localScale.x;
    }
    // 购买
    public void Buy(GameObject prefab)
    {
        buyContinuous = true;

        ClickableObject clickableObject = prefab.GetComponent<ClickableObject>();

        // 购买Unit，此时mouseObject一定是Unit或null
        if (clickableObject is Unit && editorMode == EditorMode.Unit)
        {
            Unit unit = clickableObject as Unit;
            Buy(unit);
        }
        // 购买Background，此时mouseObject一定是Background或null
        else if (clickableObject is Background && editorMode == EditorMode.Background)
        {
            Background background = clickableObject as Background;
            Buy(background);
        }
    }

    // 安放，吸附到最近的网格,如果可以再次购买,就再次购买
    public void Place(Unit unit)
    {
        Vector2 pos = unit.transform.position;
        int[] coord = PositionToClosestCoord(pos);
        if (coord != null)
        {
            int x = coord[0];
            int y = coord[1];

            if (grid[x, y] == null)
            {
                mouseObject = null;

                // 如果是购买并安放的（而非移动网格中现有的），继续购买
                if (buyContinuous)
                {
                    Buy(unit.gameObject);
                }

                Put(x, y, unit);
                unit.player = playerOwner;
                PlayPutIntoGridSound(unit);
            }
        }
    }
    // 安放，放入背景集合
    public void Place(Background background)
    {
        background.SetSpriteLayer("Background");
        backgrounds.Add(background);
        mouseObject = null;
        Put(background);
    }
    // 购买Unit
    void Buy(Unit unit)
    {
        if (mouseObject == null)
        {
            if (playerMoney >= unit.price || gameController.gamePhase == GamePhase.Editor)
            {
                if (gameController.gamePhase == GamePhase.Preparation)
                {
                    playerMoney -= unit.price;
                }

                Unit unitBought = Instantiate(unit.gameObject).GetComponent<Unit>();
                unitBought.name = unit.gameObject.name;

                Pick(unitBought);
            }
            else
            {
                MoneyNotEnough();
            }
        }
        else
        {
            Unit mouseUnit = mouseObject as Unit;
            if (playerMoney + mouseUnit.price >= unit.price || gameController.gamePhase == GamePhase.Editor)
            {
                if (gameController.gamePhase == GamePhase.Preparation)
                {
                    playerMoney += mouseUnit.price - unit.price;
                }
                Destroy(mouseUnit.gameObject);

                Unit unitBought = Instantiate(unit.gameObject).GetComponent<Unit>();
                unitBought.name = unit.gameObject.name;

                Pick(unitBought);
            }
            else
            {
                MoneyNotEnough();
            }
        }
    }
    // 购买Background
    void Buy(Background background)
    {
        if (mouseObject == null)
        {
            Background backgroundBought = Instantiate(background.gameObject).GetComponent<Background>();
            backgroundBought.name = background.gameObject.name;

            Pick(backgroundBought);
        }
        else
        {
            Background mouseBackground = mouseObject as Background;
            Destroy(mouseBackground.gameObject);

            Background backgroundBought = Instantiate(background.gameObject).GetComponent<Background>();
            backgroundBought.name = background.gameObject.name;

            Pick(backgroundBought);
        }
    }
    // 钱不够
    void MoneyNotEnough()
    {

    }
    // 鼠标右键点击
    public void RightClick(ClickableObject clickableObject, bool hold = false)
    {
        // 如果没有开启连续放置模式，但是长按鼠标。则没有效果
        if (!isClickHold && hold)
        {
            return;
        }
        Sell(clickableObject);
    }
    // 出售
    public void Sell(ClickableObject clickableObject)
    {
        if (clickableObject is Unit && editorMode == EditorMode.Unit)
        {
            Unit unit = clickableObject as Unit;
            Sell(unit);
        }
        else if (clickableObject is Background && editorMode == EditorMode.Background)
        {
            Background background = clickableObject as Background;
            Sell(background);
        }
    }
    // 出售Unit
    public void Sell(Unit unit)
    {
        // 满足出售条件
        if (gameController.gamePhase == GamePhase.Editor ||
            gameController.gamePhase == GamePhase.Preparation && !unit.isEditorCreated)
        {
            if (gameController.gamePhase == GamePhase.Preparation)
            {
                playerMoney += unit.price;
            }
            // 设置unit所在格子为null
            if (IsInGrid(unit))
            {
                grid[unit.gridX, unit.gridY] = null;
            }
            Destroy(unit.gameObject);

            int rand = UnityEngine.Random.Range(0, resourceController.audiosDelete.Length);
            AudioSource.PlayClipAtPoint(resourceController.audiosDelete[rand], unit.transform.position);
        }
    }
    // 出售Background
    public void Sell(Background background)
    {
        backgrounds.Remove(background);
        Destroy(background.gameObject);
    }
	private void PlayPutIntoGridSound(Unit unit)
	{
		int rand = UnityEngine.Random.Range(0, resourceController.audiosPut.Length);
		AudioSource.PlayClipAtPoint(resourceController.audiosPut[rand], unit.transform.position);
	}

    // 根据载入的Unit的坐标，与网格建立联系
    public void UpdateGridWithAllUnits()
    {
        grid = new Unit[XNum, YNum];
        Unit[] units = gameController.GetUnits();
        foreach (Unit unit in units)
        {
            if (IsInGrid(unit))
            {
                grid[unit.gridX, unit.gridY] = unit;
            }
        }
    }

    // 连接所有网格中的Block
    public void LinkAllBlocksInGrid()
    {
        for (int x = 0; x < XNum; x++)
        {
            for (int y = 0; y < YNum; y++)
            {
                Block block = grid[x, y] as Block;
                if (block != null)
                {
                    for (int direction = 0; direction < 4; direction++)
                    {
                        Block another = GetNeighborBlock(x, y, direction);
                        if (another != null && CanLink(block, another, direction))
                        {
                            Link(block, another, direction);
                        }
                    }
                }
            }
        }
    }

    // 连接所有Block
    [Obsolete("All blocks are in the grid now. Use LinkAllBlocksInGrid() instead.")]
    public void LinkAllBlocks()
    {
        Unit[] units = gameController.GetUnits("Block");
        foreach (Unit unit in units)
        {
            Block block = unit as Block;
            for (int direction = 0; direction < 4; direction++)
            {
                if (block.IsLinkAvailable(direction))
                {
                    Block another = GetLinkableBlockByDirection(block, direction);
                    // 连接
                    if (another != null)
                    {
                        Link(block, another, direction);
                    }
                }
            }
        }
    }

    // 检测该方向范围内是否有可连接的Block
    Block GetLinkableBlockByDirection(Block block, int direction)
    {
        // 检测连接距离
        float distanceCheck = 0.3f;

        int directionNeg = GetDirectionNegative(direction);

        Unit[] units = gameController.GetUnits("Block");
        foreach (Unit unit in units)
        {
            Block another = unit as Block;

            if (another != block &&
                // 所属同一名玩家
                block.player == another.player &&
                // Another的位置可以连接
                another.IsLinkAvailable(directionNeg) == true)
            {
                // Another的连接点
                Vector2 absorptionPoint = LinkPoint(another, directionNeg);

                // Block与连接点的距离
                float distance = ((Vector2)block.transform.position - absorptionPoint).magnitude;

                // 满足吸附距离
                if (distance <= distanceCheck)
                {
                    // 返回满足的Another
                    return another;
                }
            }
        }
        return null;
    }

    // 按照方向返回Block的连接点
    public Vector2 LinkPoint(Block block, int direction)
    {
        Vector2 point = block.transform.position;
        switch (direction)
        {
            case 0:
                point += Vector2.right * 2 * block.radius;
                break;
            case 1:
                point += Vector2.up * 2 * block.radius;
                break;
            case 2:
                point += Vector2.left * 2 * block.radius;
                break;
            case 3:
                point += Vector2.down * 2 * block.radius;
                break;
        }
        return point;
    }

    // 连接两Block
    void Link(Block block, Block another, int direction)
    {
        int directionNeg = GetDirectionNegative(direction);

        block.LinkTo(another, direction);
        another.LinkTo(block, directionNeg);
    }

    // 检测两Block是否能连接
    bool CanLink(Block block, Block another, int direction)
    {
        int directionNeg = GetDirectionNegative(direction);
        // Block的玩家的相同
        bool samePlayer = block.player == another.player;
        // Block的相应方向是可连接的
        bool linkAvailable = block.IsLinkAvailable(direction) && another.IsLinkAvailable(directionNeg);        // Block的相应方向未连接

        return samePlayer && linkAvailable;
    }

    // 根据方向获取相邻的Block
    Block GetNeighborBlock(int x, int y, int direction)
    {
        // 超出边界
        if (direction == 0 && x == XNum - 1 || direction == 1 && y == YNum - 1 || direction == 2 && x == 0 || direction == 3 && y == 0)
        {
            return null;
        }
        else
        {
            int anotherX = x + dir4[direction, 0];
            int anotherY = y + dir4[direction, 1];
            return grid[anotherX, anotherY] as Block;
        }
    }

    // 离Position最近的网格的(x, y)
    int[] PositionToClosestCoord(Vector2 position)
    {
        int[] ret = null;
        float distanceClosest = distanceAbsorption;
        for (int x = 0; x < XNum; x++)
        {
            for (int y = 0; y < YNum; y++)
            {
                float distance = (CoordToPosition(x, y) - position).magnitude;
                if (distance < distanceClosest)
                {
                    distanceClosest = distance;
                    ret = new int[] { x, y };
                }
            }

        }
        return ret;
    }

    // 根据Grid[x][y]获取中心坐标
    public Vector2 CoordToPosition(int x, int y)
    {
        Vector2 ret = new Vector2(gridSize * (x + 0.5f), gridSize * (y + 0.5f));
        return ret + origin;
    }

    protected int GetDirectionNegative(int direction)
    {
        return (direction + 2) % 4;
    }

    // 清除Mouse Object
    public void ClearMouseObject()
    {
        if (mouseObject != null)
        {
            Destroy(mouseObject.gameObject);
        }
    }
    // 清除Mouse Object Last
    public void ClearMouseObjectLast()
    {
        if (mouseObjectLast != null)
        {
            Destroy(mouseObjectLast.gameObject);
        }
    }

    // 在网格中
    public bool IsInGrid(Unit unit)
    {
        return unit.gridX >= 0 && unit.gridY >= 0 && unit.gridX < XNum && unit.gridY < YNum;
    }
	
	/// <summary>
	/// 根据Player和Unit获取Layer 
	/// </summary>
	public Layer GetUnitLayer(Player player, Unit unit)
    {
        if (player == Player.Player)
        {
            if (unit is BallGeneral)
            {
                return Layer.PlayerBall;
            }
            else
            {
                return Layer.PlayerBlock;
            }
        }
        else if (player == Player.Enemy)
        {
            if (unit is BallGeneral)
            {
                return Layer.EnemyBall;
            }
            else
            {
                return Layer.EnemyBlock;
            }
        }
        else
        {
            return Layer.Default;
        }
    }

    // 坐标是否合法
    public bool IsLegalCoord(int x, int y)
    {
        return x >= 0 && x < XNum && y >= 0 && y < YNum;
    }

    // 这一格合法且是地面
    bool IsBlockGround(int x, int y)
    {
        return IsLegalCoord(x, y) && grid[x, y] != null && grid[x, y].gameObject.layer == (int)Layer.Ground;
    }

    // 显示或隐藏网格图像
    public void ShowGrids(bool show)
    {
        gridObjects.SetActive(show);
    }

    // 初始化玩家钱数
    public void InitPlayerMoney()
    {
        playerMoney = PlayerMoneyOrigin;
    }

	// xNum与yNum变更后更新网格
	private void UpdateGridAfterResize()
    {
        // 删除网格之外的物体
        Unit[] units = gameController.GetUnits();
        foreach (Unit unit in units)
        {
            if (!IsInGrid(unit))
            {
                Destroy(unit.gameObject);
            }
        }

        // 重建
        Init();
        UpdateGridWithAllUnits();
        CreateGridSprites();
    }

    // 指令
    void Order()
    {
        if (mouseObject != null)
        {
            // 鼠标物体跟随鼠标
            mouseObject.transform.position = MouseController.MouseWorldPosition();
        }

        // 单位指令
        {
            Unit unit =
                mouseObject is Unit ? mouseObject as Unit :
                mouseObjectLast is Unit ? mouseObjectLast as Unit :
                null;

            if (unit == null)
            {
                return;
            }

            // R键旋转
            if (Input.GetKeyDown(KeyCode.R))
            {
                unit.Rotate();
            }
        }
    }
    

	/// <summary>
	/// 将unit放入(x,y), 设置位置。添加到gameController管理
	/// </summary>
	public void Put(int x, int y, Unit unit)
	{
		// 安放
		grid[x, y] = unit;
		unit.gridX = x;
		unit.gridY = y;
		unit.transform.position = CoordToPosition(x, y);

		Add2GameControllerAndSetLayer(unit);
	}

    /// <summary>
	/// 将background添加到gameController管理
	/// </summary>
	public void Put(Background background)
    {
        background.transform.position += new Vector3(0, 0, 1);
        backgrounds.Add(background);

        Add2GameControllerAndSetLayer(background);
    }

    private void Add2GameControllerAndSetLayer(Unit unit)
	{
		unit.transform.parent = gameController.unitObjects.transform;
		unit.isEditorCreated = (gameController.gamePhase == GamePhase.Editor);
		// 如果单位不是地面,则设置所有单位的显示层为Unit
		// 如果单位是地面, 默认拥有者是中立,显示物理层不变
		if (unit.gameObject.layer != (int)Layer.Ground)
		{
			// 设置Unit显示层
			unit.SetSpriteLayer("Unit");
			// 设置Unit物理层
			unit.gameObject.layer = (int)GetUnitLayer(PlayerOwner, unit);
		}
	}
    private void Add2GameControllerAndSetLayer(Background background)
    {
        background.transform.parent = gameController.backgroundObjects.transform;
        // 设置Unit显示层
        background.SetSpriteLayer("Background");
        // 设置Unit物理层
        background.gameObject.layer = (int)Layer.Background;
    }

    // 离开Editor阶段
    public void FromPhaseEditor()
    {
        InitPlayerMoney();

        // 放置物体所有者切换至玩家
        PlayerOwner = Player.Player;

        // 不一直显示HP
        isShowingHP = false;

        // 关闭快速放置
        isClickHold = false;

        EditorMode = EditorMode.Unit;

        ClearMouseObject();
        ClearMouseObjectLast();
    }

    // 进入Editor阶段
    public void ToPhaseEditor()
    {
        // 放置物体所有者切换至中立
        PlayerOwner = Player.Neutral;

        // 显示网格
        editorController.ShowGrids(true);

        ClearMouseObject();
        ClearMouseObjectLast();
    }

    // 清空背景
    public void ClearBackground()
    {
        foreach(Background background in backgrounds)
        {
            Destroy(background.gameObject);
        }
        backgrounds.Clear();
    }

    void MyDebug()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log(backgrounds.Count);
        }
    }
}
