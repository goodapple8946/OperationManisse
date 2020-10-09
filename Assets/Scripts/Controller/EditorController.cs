using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;
using static Controller;

// 类型定义
using Coord = System.Tuple<int, int>;

public class EditorController : MonoBehaviour
{
    // 当前编辑模式
    public EditorMode EditorMode
    {
        get => editorMode;
        set
        {
            editorMode = value;
            // 当编辑模式发生改变时：
            // 清除鼠标上的Unit、Background
            DestroyMouseObject();
            // 不再记录上次放置的Unit、Background
            MouseObjectLast = null;
            // 清除鼠标上的Module
            MouseModule = null;
            // 更新UI显示
            editorContent.UpdateUIShowing();
            // 不再快速放置
            IsClickHold = false;
            // 更新商店
            shopController.UpdateShop();
        }
    }
    private EditorMode editorMode;

    // 放置物体的网格
    public Unit[,] Grid { get => grid; set => grid = value; }
    private Unit[,] grid;

    // 编辑者放置的背景
    public HashSet<Background> Backgrounds => backgrounds;
    private readonly HashSet<Background> backgrounds = new HashSet<Background>();

    // Editor面板：网格尺寸x
    public int XNum
    {
        get => xNum;
        set
        {
            xNum = value;
            // 更新UI
            editorContent.UpdateUIShowing();
            // 更新网格信息
            UpdateGridAfterResize();
        }
    }
    private int xNum = 8;

    // Editor面板：网格尺寸y
    public int YNum
    {
        get => yNum;
        set
        {
            yNum = value;
            // 更新UI
            editorContent.UpdateUIShowing();
            // 更新网格信息
            UpdateGridAfterResize();
        }
    }
    private int yNum = 8;

    // Editor面板：编辑者当前使用的UnitOwner
    public Player PlayerOwner
    {
        get => playerOwner;
        set
        {
            playerOwner = value;
            editorContent.UpdateUIShowing();
        }
    }
    private Player playerOwner;

    // Editor面板：编辑者当前设置的玩家初始钱数
    public int PlayerMoneyOrigin
    {
        get => playerMoneyOrigin;
        set
        {
            playerMoneyOrigin = value;
            editorContent.UpdateUIShowing();
        }
    }
    private int playerMoneyOrigin = 0;

    // Editor面板：编辑者当前设置的全局光照
    public float LightIntensity
    {
        get => lightIntensity;
        set
        {
            lightIntensity = value;
            GameObject.Find("Global Light").GetComponent<Light2D>().intensity = LightIntensity;
            editorContent.UpdateUIShowing();
        }
    }
    private float lightIntensity = 1.0f;

    // Editor面饭：背景尺寸
    public float BackgroundScale
    {
        get => backgroundScale;
        set
        {
            backgroundScale = value;

            Background background = MouseObject is Background ? MouseObject as Background : MouseObjectLast is Background ? MouseObjectLast as Background : null;
            if (background != null)
            {
                float scale = editorContent.GetComponentInChildren<EditorScale>().GetComponent<Slider>().value;
                background.transform.localScale = new Vector2(scale, scale);
            }

            editorContent.UpdateUIShowing();
        }
    }
    private float backgroundScale = 1f;

    // Editor面饭：是否显示HP
    public bool IsShowingHP
    {
        get => isShowingHP;
        set
        {
            isShowingHP = value;
            editorContent.UpdateUIShowing();
        }
    }
    private bool isShowingHP;

    // Editor面饭：玩家钱数
    public int PlayerMoney { get => playerMoney; set => playerMoney = value; }
    private int playerMoney;

    // Editor面饭：面板上文本为Fast Click
    // 允许按住鼠标左键或右键来放置物体，只有EditorMode是Unit模式下，isClickHold才能为true
    public bool IsClickHold
    {
        get => isClickHold;
        set
        {
            isClickHold = value;
            editorContent.UpdateUIShowing();
        }
    }
    private bool isClickHold;

    // 当前鼠标持有的Unit或Background
    public ClickableObject MouseObject { get => mouseObject; set => mouseObject = value; }
    private ClickableObject mouseObject;

    // 当前鼠标持有的Unit或Background 或者 上一次鼠标持有的Unit或Background
    public ClickableObject MouseObjectLast { get => mouseObjectLast; set => mouseObjectLast = value; }
    private ClickableObject mouseObjectLast;

    // 当前鼠标持有的Module
    public XMLModule MouseModule
    {
        get => mouseModule;
        set
        {
            if (value == null)
            {
                // 清空Mouse Module时，应该同时清除上次绘制的残影
                EditorLoadModule.ClearLastDisplay();
            }
            else
            {
                EditorMode = EditorMode.Module;
            }
            mouseModule = value;
        }
    }
    private XMLModule mouseModule;

    // 摄像机的四个边的世界位置
    [HideInInspector] public float xMin;
    [HideInInspector] public float xMax;
    [HideInInspector] public float yMin;
    [HideInInspector] public float yMax;

    // 当前选中的文件
    [HideInInspector] public string fileSelected = "";

    // 当前选中的模型
    [HideInInspector] public string moduleSelected = "";

    // 网格背景物体
    public GameObject square;

    // 每个网格的大小
    private float gridSize = 0.6f;

    // 整个网格的左下角的坐标
    private Vector2 origin = Vector2.zero;

    // 根据方向获取坐标偏移
    private int[,] dir4 = { { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 } };
    private int[,] dir8 = { { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 }, { -1, -1 }, { 0, -1 }, { 1, -1 } };

    // Unit吸附的最大范围
    private float distanceAbsorption = 0.6f;

    // 连续购买（是购买并安放的，而非移动网格中现有的）
    private bool buyContinuous = false;

    // 放置位置背景颜色深度
    private float colorAlpha = 0.2f;

    // 显示Editor内容的物体
    private EditorContent editorContent;

    // 网格物体
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
        Grid = new Unit[XNum, YNum];

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
        if (!IsClickHold && hold)
        {
            return;
        }
        // Unit模式下点击Unit，此时mouseObject一定是Unit或null
        if (clickableObject is Unit && EditorMode == EditorMode.Unit)
        {
            Unit unit = clickableObject as Unit;
            LeftClick(unit);
        }
        // Background模式下点击Background，此时mouseObject一定是Background或null
        else if (clickableObject is Background && EditorMode == EditorMode.Background)
        {
            Background background = clickableObject as Background;
            LeftClick(background);
        }
    }
    // 鼠标左键点击Unit
    public void LeftClick(Unit unit)
    {
        // 满足阶段
        if (MouseObject != null)
        {
            if (MouseObject as Unit == unit)
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
            if (MouseObject != null)
            {
                if (MouseObject as Background == background)
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
            Grid[x, y] = null;
            unit.gridX = unit.gridY = -1;
        }
        unit.SetSpriteLayer("Pick");
        MouseObject = unit;
        MouseObjectLast = unit;
    }
    // 拾起BackGround
    public void Pick(Background background)
    {
        background.SetSpriteLayer("Pick");
        MouseObject = background;
        MouseObjectLast = background;
        BackgroundScale = background.transform.localScale.x;
    }
    // 购买
    public void Buy(GameObject prefab)
    {
        buyContinuous = true;

        ClickableObject clickableObject = prefab.GetComponent<ClickableObject>();

        // 购买Unit，此时mouseObject一定是Unit或null
        if (clickableObject is Unit && EditorMode == EditorMode.Unit)
        {
            Unit unit = clickableObject as Unit;
            Buy(unit);
        }
        // 购买Background，此时mouseObject一定是Background或null
        else if (clickableObject is Background && EditorMode == EditorMode.Background)
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

            if (Grid[x, y] == null)
            {
                MouseObject = null;

                // 如果是购买并安放的（而非移动网格中现有的），继续购买
                if (buyContinuous)
                {
                    Buy(unit.gameObject);
                }

                unit.player = PlayerOwner;
                // 如果单位不是地面,则设置所有单位的显示层为Unit
                // 如果单位是地面, 默认拥有者是中立,显示物理层不变
                if (unit.gameObject.layer != (int)Layer.Ground)
                {
                    // 设置Unit显示层
                    unit.SetSpriteLayer("Unit");
                    // 设置Unit物理层
                    unit.gameObject.layer = (int)GetUnitLayer(PlayerOwner, unit);
                }
                Put(x, y, unit);

                PlayPutIntoGridSound(unit);
            }
        }
    }
    // 安放，放入背景集合
    public void Place(Background background)
    {
        background.SetSpriteLayer("Background");
        background.gameObject.layer = (int)Layer.Background;
        Backgrounds.Add(background);
        MouseObject = null;
        Put(background);
    }
    // 购买Unit
    void Buy(Unit unit)
    {
        if (MouseObject == null)
        {
            if (PlayerMoney >= unit.price || gameController.gamePhase == GamePhase.Editor)
            {
                if (gameController.gamePhase == GamePhase.Preparation)
                {
                    PlayerMoney -= unit.price;
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
            Unit mouseUnit = MouseObject as Unit;
            if (PlayerMoney + mouseUnit.price >= unit.price || gameController.gamePhase == GamePhase.Editor)
            {
                if (gameController.gamePhase == GamePhase.Preparation)
                {
                    PlayerMoney += mouseUnit.price - unit.price;
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
        if (MouseObject == null)
        {
            Background backgroundBought = Instantiate(background.gameObject).GetComponent<Background>();
            backgroundBought.name = background.gameObject.name;

            Pick(backgroundBought);
        }
        else
        {
            Background mouseBackground = MouseObject as Background;
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
        if (!IsClickHold && hold)
        {
            return;
        }
        Sell(clickableObject);
    }
    // 出售
    public void Sell(ClickableObject clickableObject)
    {
        if (clickableObject is Unit && EditorMode == EditorMode.Unit)
        {
            Unit unit = clickableObject as Unit;
            Sell(unit);
        }
        else if (clickableObject is Background && EditorMode == EditorMode.Background)
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
                PlayerMoney += unit.price;
            }
            // 设置unit所在格子为null
            if (IsInGrid(unit))
            {
                Grid[unit.gridX, unit.gridY] = null;
            }
            Destroy(unit.gameObject);

            int rand = UnityEngine.Random.Range(0, resourceController.audiosDelete.Length);
            AudioSource.PlayClipAtPoint(resourceController.audiosDelete[rand], unit.transform.position);
        }
    }
    // 出售Background
    public void Sell(Background background)
    {
        Backgrounds.Remove(background);
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
        Grid = new Unit[XNum, YNum];
        Unit[] units = gameController.GetUnits();
        foreach (Unit unit in units)
        {
            if (IsInGrid(unit))
            {
                Grid[unit.gridX, unit.gridY] = unit;
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
                Block block = Grid[x, y] as Block;
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
            return Grid[anotherX, anotherY] as Block;
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
        return IsLegalCoord(x, y) && Grid[x, y] != null && Grid[x, y].gameObject.layer == (int)Layer.Ground;
    }

    // 显示或隐藏网格图像
    public void ShowGrids(bool show)
    {
        gridObjects.SetActive(show);
    }

    // 初始化玩家钱数
    public void InitPlayerMoney()
    {
        PlayerMoney = PlayerMoneyOrigin;
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
        // 编辑模式：Unit
        if (EditorMode == EditorMode.Unit)
        {
            if (MouseObject != null)
            {
                // 鼠标物体跟随鼠标
                MouseObject.transform.position = MouseController.MouseWorldPosition();
            }

            // 单位指令
            {
                Unit unit =
                    MouseObject is Unit ? MouseObject as Unit :
                    MouseObjectLast is Unit ? MouseObjectLast as Unit :
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
        // 编辑模式：Background
        else if (EditorMode == EditorMode.Background)
        {
            if (MouseObject != null)
            {
                // 鼠标物体跟随鼠标
                MouseObject.transform.position = MouseController.MouseWorldPosition();
            }
        }
        // 编辑模式：Module
        else if (EditorMode == EditorMode.Module)
        {
            Coord mouseCoord = GetMouseCoord();
            if (mouseCoord.Item1 != -1 && mouseCoord.Item2 != -1 && MouseModule != null)
            {
                // 清空之前的绘制
                bool canPlace = EditorLoadModule.CanPlaceModuleCenter(MouseModule, mouseCoord);
                if (canPlace)
                {
                    // 鼠标产生了偏移
                    if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
                    {
                        // 清除上一帧的绘制
                        EditorLoadModule.ClearLastDisplay();
                        EditorLoadModule.DisplayModuleCenter(MouseModule, mouseCoord);
                    }
                    // 鼠标左键点击
                    if (Input.GetMouseButtonDown(0))
                    {
                        // 清除之前的最后一帧
                        EditorLoadModule.ClearLastDisplay();
                        EditorLoadModule.LoadModuleCenter(MouseModule, mouseCoord);
                    }
                }
                else
                {
                    // 清除之前的最后一帧
                    EditorLoadModule.ClearLastDisplay();
                }
                // 鼠标右键点击
                if (Input.GetMouseButtonDown(1))
                {
                    // 清除Mouse Module
                    MouseModule = null;
                }
            }
        }
    }

    /// <summary>
    /// 将unit放入(x,y), 设置位置。添加到gameController管理
    /// </summary>
    public void Put(int x, int y, Unit unit)
    {
        // 安放
        Grid[x, y] = unit;
        unit.gridX = x;
        unit.gridY = y;
        unit.transform.position = CoordToPosition(x, y);
        unit.transform.parent = gameController.unitObjects.transform;
        unit.isEditorCreated = (gameController.gamePhase == GamePhase.Editor);
    }

    /// <summary>
	/// 将background添加到gameController管理
	/// </summary>
	public void Put(Background background)
    {
        background.transform.position += new Vector3(0, 0, 1);
        Backgrounds.Add(background);
        background.transform.parent = gameController.backgroundObjects.transform;
    }

    // 离开Editor阶段
    public void FromPhaseEditor()
    {
        InitPlayerMoney();

        // 放置物体所有者切换至玩家
        PlayerOwner = Player.Player;

        // 不一直显示HP
        IsShowingHP = false;

        // 关闭快速放置
        IsClickHold = false;

        EditorMode = EditorMode.Unit;

        DestroyMouseObject();
        MouseObjectLast = null;
        MouseModule = null;
    }

    // 进入Editor阶段
    public void ToPhaseEditor()
    {
        // 放置物体所有者切换至中立
        PlayerOwner = Player.Neutral;

        // 显示网格
        editorController.ShowGrids(true);

        DestroyMouseObject();
        MouseObjectLast = null;
        MouseModule = null;
    }

    // 摧毁鼠标上附着的Unit或Background
    public void DestroyMouseObject()
    {
        if (MouseObject != null)
        {
            Destroy(MouseObject);
        }
    }

    // 清空网格
    public void ClearGrid()
    {
        foreach(Unit unit in Grid)
        {
            if (unit != null)
            {
                Destroy(unit.gameObject);
            }
        }
    }

    // 清空背景
    public void ClearBackground()
    {
        foreach (Background background in Backgrounds)
        {
            Destroy(background.gameObject);
        }
        Backgrounds.Clear();
    }

    /// <summary>
    /// 获得鼠标所在网格坐标，如果不在网格中返回-1, -1
    /// </summary>
    public Coord GetMouseCoord()
    {
        int x = (int)(MouseController.MouseWorldPosition().x / gridSize);
        int y = (int)(MouseController.MouseWorldPosition().y / gridSize);

        if (IsLegalCoord(x, y))
        {
            return new Coord(x, y);
        }
        else
        {
            return new Coord(-1, -1);
        }
    }

    // 更新显示的文件
    public void UpdateFiles()
    {
        GameObject.Find("Files Scroll View").GetComponentInChildren<EditorFiles>().UpdateFiles();
    }

    // 更新显示的模型
    public void UpdateModules()
    {
        GameObject.Find("Modules Scroll View").GetComponentInChildren<EditorModules>().UpdateModules();
    }

    /// <summary>
    /// 坐标减法
    /// </summary>
    public static Coord Minus(Coord a, Coord b)
    {
        return new Coord(a.Item1 - b.Item1, a.Item2 - b.Item2);
    }

    ///  <summary>
    ///  获取第一列（行）有非空格子的横（纵）坐标，
    /// 参数：Left：从左向右数，Right：从右向左数，Bottom：从下向上数，Top：从上向下数，
    /// 如果网格为空返回-1
    /// </summary>
    public int GetGridCoordNonEmpty(string dir)
    {
        switch (dir)
        {
            case "Left":
                for (int x = 0; x < XNum; x++)
                {
                    for (int y = 0; y < YNum; y++)
                    {
                        if (Grid[x, y] != null)
                        {
                            return x;
                        }
                    }
                }
                break;
            case "Right":
                for(int x = XNum - 1; x >= 0; x--)
                {
                    for (int y = 0; y < YNum; y++)
                    {
                        if (Grid[x, y] != null)
                        {
                            return x;
                        }
                    }
                }
                break;
            case "Bottom":
                for (int y = 0; y < YNum; y++)
                {
                    for (int x = 0; x < XNum; x++)
                    {
                        if (Grid[x, y] != null)
                        {
                            return y;
                        }
                    }
                }
                break;
            case "Top":
                for (int y = YNum - 1; y >= 0; y--)
                {
                    for (int x = 0; x < XNum; x++)
                    {
                        if (Grid[x, y] != null)
                        {
                            return y;
                        }
                    }
                }
                break;
        }
        return -1;
    }

    void MyDebug()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log(GetMouseCoord().Item1 + " " + GetMouseCoord().Item2);
        }
    }
}
