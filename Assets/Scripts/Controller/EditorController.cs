using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using static GameController;

public class EditorController : MonoBehaviour
{
	// 编辑模式
	public enum EditorMode { None, Unit }

	// 当前编辑模式
	[HideInInspector] public EditorMode editorMode;

	// 玩家放置物体的网格
	private Unit[,] grid;
	[HideInInspector] public Unit[,] Grid { get => grid;}

	// Editor面板：网格尺寸x
	[HideInInspector] private int xNum = 8;
	// Editor面板：网格尺寸y
	[HideInInspector] private int yNum = 8;
	// Editor面板：编辑者当前使用的UnitOwner
	[HideInInspector] private Player playerOwner = Player.Player;
	// Editor面板：编辑者当前设置的玩家初始钱数
	[HideInInspector] private int playerMoneyOrigin = 0;
	// Editor面板：编辑者当前设置的全局光照
	[HideInInspector] private float lightIntensity = 1.0f;
	#region Property Function
	// 更新EditorSizeX的显示
	public int XNum
	{
		get => xNum;
		set
		{
			xNum = value;
			// 同步数据变更到UI显示
			editorSizeX.ShowX(xNum);
			// 更新网格信息
			UpdateGridAfterResize();
		}
	}
	// 更新EditorSizeY的显示
	public int YNum
	{
		get => yNum;
		set
		{
			yNum = value;
			// 同步数据变更到UI显示
			editorSizeY.ShowY(yNum);
			// 更新网格信息
			UpdateGridAfterResize();
		}
	}
	// 更新Editor的显示
	public Player PlayerOwner
	{
		get => playerOwner;
		set
		{
			playerOwner = value;
			editorOwener.ShowOwner(playerOwner);
		}
	}
	// 更新Editor的显示
	public int PlayerMoneyOrigin
	{
		get => playerMoneyOrigin;
		set
		{
			playerMoneyOrigin = value;
			editorMoney.ShowMoney(playerMoneyOrigin);
		}
	}
	// 改变光照强度并更新Editor的显示
	public float LightIntensity
	{
		get => lightIntensity;
		set
		{
			lightIntensity = value;
			GameObject.Find("Global Light").GetComponent<Light2D>().intensity = lightIntensity;
			editorLight.ShowLight(lightIntensity);
		}
	}
	#endregion

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
	[HideInInspector] public Unit mouseUnit;

	// 允许接收按住鼠标左键或右键
	[HideInInspector] public bool isClickHold;

	// 连续购买（是购买并安放的，而非移动网格中现有的）
	private bool buyContinuous = false;

	// 放置位置背景颜色深度
	[SerializeField] private float colorAlpha = 0.2f;

	// 玩家钱数
	[HideInInspector] public int playerMoney;

	// 显示HP
	[HideInInspector] public bool isShowingHP;

	[SerializeField] private GameObject square;
    private GameController gameController;
	private ResourceController resourceController;
	private EditorMoney editorMoney;
	private EditorOwner editorOwener;
	private EditorLight editorLight;
	private EditorSizeX editorSizeX;
	private EditorSizeY editorSizeY;

	private GameObject gridObjects;

	void Awake()
	{
		gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
		resourceController = GameObject.Find("Resource Controller").GetComponent<ResourceController>();
		editorMoney = GameObject.Find("UI Editor").GetComponentInChildren<EditorMoney>();
		editorOwener = GameObject.Find("UI Editor").GetComponentInChildren<EditorOwner>();
		editorLight = GameObject.Find("UI Editor").GetComponentInChildren<EditorLight>();
		editorSizeX = GameObject.Find("UI Editor").GetComponentInChildren<EditorSizeX>();
		editorSizeY = GameObject.Find("UI Editor").GetComponentInChildren<EditorSizeY>();
	}

    void Start()
    {
        Init();
        CreateGridSprites();
    }

    void Update()
    {
        // 更新编辑模式
        UpdateEditorMode();

        // 键盘指令
        KeyOrder();
    }

    void Init()
    {
        // 初始化网格
        gridObjects = new GameObject("Grid Objects");
        grid = new Unit[XNum, YNum];

        // 初始化四个边坐标
        xMin = 0;
        yMin = 0;
        xMax = gridSize * XNum;
        yMax = gridSize * YNum;
    }

    void UpdateEditorMode()
    {
        if (mouseUnit == null)
        {
            editorMode = EditorMode.None;
        }
        else
        {
            editorMode = EditorMode.Unit;
        }
    }

    public void LeftClickUnit(Unit unit, bool hold = false)
    {
        if (!isClickHold && hold)
        {
            return;
        }
        if (mouseUnit != null)
        {
            if (mouseUnit == unit)
            {
                // 安放鼠标上的物体到网格中
                CheckAbsorption(unit);
            }
            else
            {
                // 将网格物体与鼠标物体交换
            }
        }
        else
        {
            // 移动网格中的物体
            if (gamePhase == GamePhase.Editor ||
                gamePhase == GamePhase.Preparation && unit.player == Player.Player)
            {
                buyContinuous = false;
                Pick(unit);
            }
        }
    }

    public void RightClickUnit(Unit unit, bool hold = false)
    {
        if (!isClickHold && hold)
        {
            return;
        }
        Sell(unit);
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

    // 购买
    public void Buy(GameObject prefab)
    {
        buyContinuous = true;

        Unit unit = prefab.GetComponent<Unit>();

        if (mouseUnit == null)
        {
            if (playerMoney >= unit.price || gamePhase == GamePhase.Editor)
            {
                if (gamePhase == GamePhase.Preparation)
                {
                    playerMoney -= unit.price;
                }

                Unit unitBought = Instantiate(prefab).GetComponent<Unit>();
                unitBought.name = prefab.name;

                Pick(unitBought);
            }
            else
            {
                MoneyNotEnough();
            }
        }
        else
        {
            if (playerMoney + mouseUnit.price >= unit.price || gamePhase == GamePhase.Editor)
            {
                if (gamePhase == GamePhase.Preparation)
                {
                    playerMoney += mouseUnit.price - unit.price;
                }
                Destroy(mouseUnit.gameObject);

                Unit unitBought = Instantiate(prefab).GetComponent<Unit>();
                unitBought.name = prefab.name;

                Pick(unitBought);
            }
            else
            {
                MoneyNotEnough();
            }
        }
    }

    // 钱不够
    void MoneyNotEnough()
    {

    }

    // 出售
    public void Sell(Unit unit)
    {
        // 满足出售条件
        if (gamePhase == GamePhase.Editor ||
            gamePhase == GamePhase.Preparation && !unit.isEditorCreated)
        {
            if (gamePhase == GamePhase.Preparation)
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

    // 拾起
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
        mouseUnit = unit;
    }

    // 安放，吸附到最近的网格
    public void CheckAbsorption(Unit unit)
    {
        Vector2 pos = unit.transform.position;
        int[] coord = PositionToClosestCoord(pos);
        if (coord != null)
        {
            int x = coord[0];
            int y = coord[1];

            if (grid[x, y] == null)
            {
                mouseUnit = null;

                // 如果是购买并安放的（而非移动网格中现有的），继续购买
                if (buyContinuous)
                {
                    Buy(unit.gameObject);
                }

				Put(x, y, unit);
				PlayPutIntoGridSound(unit);
			}
        }
    }

	private void PlayPutIntoGridSound(Unit unit)
	{
		int rand = UnityEngine.Random.Range(0, resourceController.audiosPut.Length);
		AudioSource.PlayClipAtPoint(resourceController.audiosPut[rand], unit.transform.position);
	}

    // 根据载入的Unit的坐标，与网格建立联系
    public void UpdateGridWithAllUnits()
    {
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
                if (block.blocksLinked[direction] == null && block.IsLinkAvailable(direction))
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
                another.IsLinkAvailable(directionNeg) == true &&
                // Another的位置没有被占用
                another.blocksLinked[directionNeg] == null)
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
        bool linkAvailable = block.IsLinkAvailable(direction) && another.IsLinkAvailable(directionNeg);
        // Block的相应方向未连接
        bool unlinked = block.blocksLinked[direction] == null && another.blocksLinked[directionNeg] == null;

        return samePlayer && linkAvailable && unlinked;
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
    Vector2 CoordToPosition(int x, int y)
    {
        Vector2 ret = new Vector2(gridSize * (x + 0.5f), gridSize * (y + 0.5f));
        return ret + origin;
    }

    protected int GetDirectionNegative(int direction)
    {
        return (direction + 2) % 4;
    }

    // 清除MouseUnit
    public void ClearMouseUnit()
    {
        if (mouseUnit != null)
        {
            Destroy(mouseUnit.gameObject);
        }
    }

    // 在网格中
    bool IsInGrid(Unit unit)
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
            if (unit is Ball)
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
            if (unit is Ball)
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
    bool IsLegalCoord(int x, int y)
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

    // 结束编辑模式
    public void FinishEditor()
    {
        InitPlayerMoney();

        // 放置物体所有者切换至玩家
        PlayerOwner = Player.Player;

        // 不一直显示HP
        isShowingHP = false;

        // 关闭快速放置
        isClickHold = false;
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

        // 删除当前网格
        Destroy(gridObjects);

        // 重建
        Init();
        UpdateGridWithAllUnits();
        CreateGridSprites();
    }

    // 键盘指令
    void KeyOrder()
    {
        if (mouseUnit != null)
        {
            mouseUnit.transform.position = MouseController.MouseWorldPosition();

            // R键旋转
            if (Input.GetKeyDown(KeyCode.R))
            {
				mouseUnit.Rotate();
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

	private void Add2GameControllerAndSetLayer(Unit unit)
	{
		unit.transform.parent = gameController.unitObjects.transform;
		unit.isEditorCreated = (gamePhase == GamePhase.Editor);
		// 如果单位不是地面,则设置所有单位的显示层为Unit
		// 如果单位是地面, 默认拥有者是中立,显示物理层不变
		if (unit.gameObject.layer != (int)Layer.Ground)
		{
			// 设置Unit显示层
			unit.SetSpriteLayer("Unit");
			// 设置player
			unit.player = PlayerOwner;
			// 设置Unit物理层
			unit.gameObject.layer = (int)GetUnitLayer(PlayerOwner, unit);
		}
	}

}
