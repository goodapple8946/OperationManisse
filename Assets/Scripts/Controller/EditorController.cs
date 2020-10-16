using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UI;
using static Controller;


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
            editorContent.UpdateUIShowing<EditorEditorMode>();
            editorContent.RefreshByEditorMode();
            // 不再快速放置
            IsClickHold = false;
            // 更新商店
            shopController.UpdateShop(GamePhase.Editor);
            // 更新物体碰撞（能否被鼠标点击）
            UpdateObjectsCollider(editorMode);
        }
    }
    private EditorMode editorMode;

    // 放置物体的网格
    public Grid MainGrid;

    public Grid BuildingGrid;

    // 编辑者放置的背景
    public HashSet<Background> Backgrounds => backgrounds;
    private readonly HashSet<Background> backgrounds = new HashSet<Background>();

    // 编辑者放置的地形
    public HashSet<TerrainA> Terrains => terrains;
    private readonly HashSet<TerrainA> terrains = new HashSet<TerrainA>();

    // MainGrid的起始世界位置
    public static readonly Vector2 MAINGRID_POS = Vector2.zero;

    // Editor面板：网格尺寸x
    public int XNum
    {
        get => xNum;
        set
        {
            xNum = value;
            // 更新UI
            editorContent.UpdateUIShowing<EditorSizeX>();
            // 更新网格信息
            RecreateMainGrid(gameController.GetUnits());
            // 如果放置区超出了MainGrid就清空
            if (!MainGrid.InGrid(BuildingCoord1) || !MainGrid.InGrid(BuildingCoord2))
            {
                BuildingCoord1 = new Coord(0, 0);
                BuildingCoord2 = new Coord(0, 0);
                BuildingGrid.ClearSquares();
                BuildingGrid = CreateBuildingGrid(BuildingCoord1, BuildingCoord2);
            }
			// 更新天气粒子scale
			currAmbience.Clear();
			currAmbience.Apply();
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
            editorContent.UpdateUIShowing<EditorSizeY>();
            // 更新网格信息
            RecreateMainGrid(gameController.GetUnits());
            // 如果放置区超出了MainGrid就清空
            if (!MainGrid.InGrid(BuildingCoord1) || !MainGrid.InGrid(BuildingCoord2))
            {
                BuildingCoord1 = new Coord(0, 0);
                BuildingCoord2 = new Coord(0, 0);
                BuildingGrid.ClearSquares();
                BuildingGrid = CreateBuildingGrid(BuildingCoord1, BuildingCoord2);
            }
			// 更新天气粒子scale
			currAmbience.Clear();
			currAmbience.Apply();
		}
    }
    private int yNum = 8;

    /// <summary> building在世界的坐标</summary>
    public Coord BuildingCoord1
    {
        get => buildingCoord1;
        set
        {
            buildingCoord1 = value;
            // 更新前端展示
            EditorPointer.point1.UpdateShowing(buildingCoord1);
            // 更新网格信息
            BuildingGrid.ClearSquares();
            BuildingGrid = CreateBuilding(buildingCoord1, buildingCoord2);
            // 取消选中
            EditorPointer.point1.SetOn(false);
        }
    }
    private Coord buildingCoord1 = new Coord(0, 0);

    /// <summary> building在世界的坐标</summary>
    public Coord BuildingCoord2
    {
        get => buildingCoord2;
        set
        {
            buildingCoord2 = value;
            // 更新前端展示
            EditorPointer.point2.UpdateShowing(buildingCoord2);
            // 更新网格信息
            BuildingGrid.ClearSquares();
            BuildingGrid = CreateBuilding(buildingCoord1, buildingCoord2);
            // 取消选中
            EditorPointer.point2.SetOn(false);
        }
    }
    private Coord buildingCoord2 = new Coord(0, 0);

    // Editor面板：编辑者当前使用的UnitOwner
    public Player PlayerOwner
    {
        get => playerOwner;
        set
        {
            playerOwner = value;
            editorContent.UpdateUIShowing<EditorOwner>();
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
            editorContent.UpdateUIShowing<EditorMoney>();
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
            editorContent.UpdateUIShowing<EditorLight>();
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

            Background background =
                MouseObject is Background ? MouseObject as Background :
                MouseObjectLast is Background ? MouseObjectLast as Background :
                null;
            if (background != null)
            {
                background.transform.localScale = new Vector2(backgroundScale, backgroundScale);
            }

            editorContent.UpdateUIShowing<EditorScale>();
        }
    }
    private float backgroundScale = 1f;

    // Editor面板：地形宽度
    public int TerrainWidth
    {
        get => terrainWidth;
        set
        {
            terrainWidth = value;

            TerrainA terrain =
                MouseObject is TerrainA ? MouseObject as TerrainA :
                MouseObjectLast is TerrainA ? MouseObjectLast as TerrainA :
                null;
            if (terrain != null)
            {
                terrain.Width = terrainWidth;
            }

            editorContent.UpdateUIShowing<EditorTerrainWidth>();
        }
    }
    private int terrainWidth;

    // Editor面板：地形高度
    public int TerrainHeight
    {
        get => terrainHeight;
        set
        {
            terrainHeight = value;

            TerrainA terrain =
                MouseObject is TerrainA ? MouseObject as TerrainA :
                MouseObjectLast is TerrainA ? MouseObjectLast as TerrainA :
                null;
            if (terrain != null)
            {
                terrain.Height = terrainHeight;
            }

            editorContent.UpdateUIShowing<EditorTerrainHeight>();
        }
    }
    private int terrainHeight;

    // Editor面饭：是否显示HP
    public bool IsShowingHP
    {
        get => isShowingHP;
        set
        {
            isShowingHP = value;
            editorContent.UpdateUIShowing<EditorShowHP>();
        }
    }
    private bool isShowingHP = true;

    // Editor面饭：面板上文本为Fast Click
    // 允许按住鼠标左键或右键来放置物体，只有EditorMode是Unit模式下，isClickHold才能为true
    public bool IsClickHold
    {
        get => isClickHold;
        set
        {
            isClickHold = value;
            editorContent.UpdateUIShowing<EditorHold>();
        }
    }
    private bool isClickHold;

    // 当前鼠标持有的Unit或Background或Terrain
    public ClickableObject MouseObject { get => mouseObject; set => mouseObject = value; }
    private ClickableObject mouseObject;

    // 当前鼠标持有的Unit或Background或Terrain 或者 上一次鼠标持有的Unit或Background或Terrain
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

    // 获取MouseObject时，鼠标与MouseObject中心位置的偏移
    private Vector2 mouseObjectOffset;

    // 连续购买（是购买并安放的，而非移动网格中现有的）
    private bool isFastClick = false;

    // 显示Editor内容的物体
    private EditorContent editorContent;

    // 玩家剩余钱数
    public int PlayerMoney { get => playerMoney; set => playerMoney = value; }
    private int playerMoney;

    public Ambience CurrAmbience
	{
        get => currAmbience;
        set
        {
			currAmbience.Clear();
			currAmbience = value;
			currAmbience.Apply();
        }
    }
    private Ambience currAmbience;


    //------------------------  成员函数 ----------------------//

    void Awake()
    {
        editorContent = GameObject.Find("UI Editor").GetComponentInChildren<EditorContent>();
    }

    void Update()
    {
        Order();

        if (Input.GetMouseButtonDown(0) && GetMouseCoord() != Coord.OUTSIDE && !EventSystem.current.IsPointerOverGameObject())
        {
            // 设置放置网格点
            if (EditorPointer.point1.IsOn())
            {
                BuildingCoord1 = GetMouseCoord();
            }
            else if (EditorPointer.point2.IsOn())
            {
                BuildingCoord2 = GetMouseCoord();
            }
        }

        MyDebug();
    }

    public void StartInit()
    {
        CreateMainGrid();

        BuildingGrid = CreateBuildingGrid(BuildingCoord1, BuildingCoord2);

        currAmbience = resourceController.ambiences[0];
        currAmbience.Apply();
    }

    /// <summary>
    /// 创建空的面板并设置摄像机视角
    /// </summary>
    void CreateMainGrid()
    {
        MainGrid = new Grid(XNum, YNum, MAINGRID_POS);
        cameraController.SetView(
            editorController.MainGrid.OriginPos,
            editorController.MainGrid.GetRightTopPos());
    }

    /// <summary>
    ///	根据现有面板配置创建Grid
    /// 并设置摄像机视角 
    /// </summary>
    public void RecreateMainGrid(Unit[] units)
    {
        MainGrid.ClearSquares();
        MainGrid = new Grid(XNum, YNum, MAINGRID_POS, units);
        cameraController.SetView(
            editorController.MainGrid.OriginPos,
            editorController.MainGrid.GetRightTopPos());
    }

    /// <summary>
    /// 左下闭右上开区间，coord不需要保证有序性
    /// 不能画空网格
    /// </summary>
    Grid CreateBuilding(Coord coord1, Coord coord2)
    {
        Coord lbCoord = new Coord(
            Mathf.Min(coord1.x, coord2.x),
            Mathf.Min(coord1.y, coord2.y));
        // 保证rtCoord被画出
        Coord rtCoord = new Coord(
            Mathf.Max(coord1.x, coord2.x) + 1,
            Mathf.Max(coord1.y, coord2.y) + 1);
        return CreateBuildingGrid(lbCoord, rtCoord);
    }

    /// <summary>
    ///	世界坐标的左下和右上角,左闭右开区间
    ///	可以画空网格
    ///	BUIDING_ALPHA: 设置building透明度
    /// </summary>
    Grid CreateBuildingGrid(Coord lbCoord, Coord rtCoord)
    {
        //Debug.Log(lbCoord.ToString());
        //Debug.Log(rtCoord.ToString());
        //Debug.Assert(lbCoord.x <= rtCoord.x && lbCoord.y <= rtCoord.y
        //	&& MainGrid.InGrid(lbCoord) && MainGrid.InGrid(rtCoord));

        Vector2 origin = MainGrid.Coord2WorldPos(lbCoord, false);
        return new Grid(
            rtCoord.x - lbCoord.x,
            rtCoord.y - lbCoord.y,
            origin, Grid.BUIDING_ALPHA);
    }

    /// <summary>
    /// 进入Editor阶段
    /// </summary>
    public void EnterPhaseEditor()
    {
        // 放置物体所有者切换至中立
        PlayerOwner = Player.Neutral;
        // 显示HP
        IsShowingHP = true;
        // 更新物体的Collider
        UpdateObjectsCollider(EditorMode);
        // 切换放置模式为Unit
        EditorMode = EditorMode.Unit;
        // 清除鼠标上的物体
        DestroyMouseObject();
        MouseObjectLast = null;
        MouseModule = null;
    }

    /// <summary>
    /// 离开Editor阶段
    /// </summary>
    public void LeavePhaseEditor()
    {
        // 设置玩家钱数
        InitPlayerMoney();
        // 放置物体所有者切换至玩家
        PlayerOwner = Player.Player;
        // 不一直显示HP
        IsShowingHP = false;
        // 关闭快速放置
        IsClickHold = false;
        // 切换放置模式为Unit
        EditorMode = EditorMode.Unit;
        // 地形启用Collider
        SetTerrainsCollider(true);
        // 清除鼠标上的物体
        DestroyMouseObject();
        MouseObjectLast = null;
        MouseModule = null;
        // 关闭鼠标选择放置区
        EditorPointer.point1.SetOn(false);
        EditorPointer.point2.SetOn(false);
        // 清除当前的提示条
        Tooltip.tooltip.Hide();
    }

    /// <summary>
    /// 鼠标左键点击，hold代表是否正在长按鼠标
    /// </summary>
    public void LeftClick(ClickableObject clickableObject, bool hold = false)
    {
        // 如果没有开启连续放置模式，但是长按鼠标。则没有效果
        if (!IsClickHold && hold)
        {
            return;
        }
        // Unit模式下点击Unit，此时mouseObject一定是Unit或null
        if (clickableObject is Unit)
        {
            LeftClick(clickableObject as Unit);
        }
        // Background模式下点击Background，此时mouseObject一定是Background或null
        else if (clickableObject is Background)
        {
            LeftClick(clickableObject as Background);
        }
        // Terrain模式下点击Terrain，此时mouseObject一定是Terrain或null
        else if (clickableObject is TerrainA)
        {
            LeftClick(clickableObject as TerrainA);
        }
    }

    // 鼠标左键点击Unit
    private void LeftClick(Unit unit)
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
                // 编辑阶段：可以移动任意的Unit
                gameController.GamePhase == GamePhase.Editor ||
                // 准备阶段：不能移动其他玩家的Unit、不能移动编辑器创建的Unit
                (gameController.GamePhase == GamePhase.Preparation && unit.player == Player.Player && !unit.isEditorCreated))
            {
                isFastClick = false;
                PlayerOwner = unit.player;
                Pick(unit);
            }
        }
    }

    // 鼠标左键点击Background
    private void LeftClick(Background background)
    {
        // 必须是编辑阶段
        if (gameController.GamePhase != GamePhase.Editor)
        {
            return;
        }
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
            isFastClick = false;
            Pick(background);
        }
    }

    // 鼠标左键点击Terrain
    private void LeftClick(TerrainA terrain)
    {
        // 必须是编辑阶段
        if (gameController.GamePhase != GamePhase.Editor)
        {
            return;
        }
        if (MouseObject != null)
        {
            if (MouseObject as TerrainA == terrain)
            {
                // 安放鼠标上的地形到地形集合
                Place(terrain);
            }
        }
        else
        {
            // 移动地形集合中的地形
            isFastClick = false;
            Pick(terrain);
        }
    }

    /// <summary>
    /// 鼠标右键点击，hold代表是否正在长按鼠标
    /// </summary>
    public void RightClick(ClickableObject clickableObject, bool hold = false)
    {
        // 如果没有开启连续放置模式，但是长按鼠标。则没有效果
        if (!IsClickHold && hold)
        {
            return;
        }
        Sell(clickableObject);
    }

    // 拾起Unit
    private void Pick(Unit unit)
    {
        if (unit.coord != Coord.OUTSIDE)
        {
            MainGrid.Set(unit.coord, null);
            unit.coord = Coord.OUTSIDE;
        }
        Util.SetSpriteLayer(unit.gameObject, "Pick");
        MouseObject = unit;
        MouseObjectLast = unit;
    }

    // 拾起BackGround
    private void Pick(Background background)
    {
        Util.SetSpriteLayer(background.gameObject, "Pick");
        MouseObject = background;
        MouseObjectLast = background;
        BackgroundScale = background.transform.localScale.x;
        mouseObjectOffset = (Vector2)background.transform.position - MouseController.MouseWorldPosition();
    }

    // 拾起Terrain
    private void Pick(TerrainA terrain)
    {
        Util.SetSpriteLayer(terrain.gameObject, "Pick");
        MouseObject = terrain;
        MouseObjectLast = terrain;
        TerrainWidth = terrain.Width;
        TerrainHeight = terrain.Height;
        mouseObjectOffset = (Vector2)terrain.transform.position - MouseController.MouseWorldPosition();
    }

    // 购买
    public void Buy(GameObject prefab)
    {
        isFastClick = true;

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
        // 购买Terrain，此时mouseObject一定是Terrain或null
        else if (clickableObject is TerrainA && EditorMode == EditorMode.Terrain)
        {
            TerrainA terrain = clickableObject as TerrainA;
            Buy(terrain);
        }
    }

    // 安放Unit，吸附到最近的网格,如果可以再次购买,就再次购买
    public void Place(Unit unit)
    {
        Vector2 pos = unit.transform.position;
        Coord worldCoord = MainGrid.GetClosestCoord(pos);

        // 排除不符合放置条件的情况
        if (gameController.GamePhase == GamePhase.Editor)
        {
            if (!MainGrid.InGrid(worldCoord) || MainGrid.Get(worldCoord) != null)
                return;
        }
        else if (gameController.GamePhase == GamePhase.Preparation)
        {
            Coord coord = BuildingGrid.GetClosestCoord(pos);
            if (!BuildingGrid.InGrid(coord) || MainGrid.Get(worldCoord) != null)
                return;
        }

        MouseObject = null;
        // 如果是购买并安放的（而非移动网格中现有的），继续购买
        if (isFastClick)
        {
            Buy(unit.gameObject);
        }

		Put(worldCoord, unit, playerOwner);

        PlayPutIntoGridSound(unit);
    }

    // 安放Background
    public void Place(Background background)
    {
        MouseObject = null;
        Put(background);
		
	}

    // 安放Terrain，吸附镶嵌到最近的网格
    public void Place(TerrainA terrain)
    {
        terrain.AdjustPosition();
        MouseObject = null;
        Put(terrain);
    }

    // 购买Unit
    void Buy(Unit unit)
    {
        if (MouseObject == null)
        {
            if (PlayerMoney >= unit.price || gameController.GamePhase == GamePhase.Editor)
            {
                if (gameController.GamePhase == GamePhase.Preparation)
                {
                    PlayerMoney -= unit.price;
                }
				Unit unitBought = CreateObject(unit);
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
            if (PlayerMoney + mouseUnit.price >= unit.price || gameController.GamePhase == GamePhase.Editor)
            {
                if (gameController.GamePhase == GamePhase.Preparation)
                {
                    PlayerMoney += mouseUnit.price - unit.price;
                }
                Destroy(mouseUnit.gameObject);

                Unit unitBought = CreateObject(unit);
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
            Background backgroundBought = CreateObject(background);
            Pick(backgroundBought);
        }
        else
        {
            Background mouseBackground = MouseObject as Background;
            Destroy(mouseBackground.gameObject);

            Background backgroundBought = CreateObject(background);
            Pick(backgroundBought);
        }
    }
    // 购买Terrain
    void Buy(TerrainA terrain)
    {
        if (MouseObject == null)
        {
            TerrainA terrainBought = CreateObject(terrain);
            Pick(terrainBought);
        }
        else
        {
            TerrainA mouseTerrain = MouseObject as TerrainA;
            Destroy(mouseTerrain.gameObject);

            TerrainA terrainBought = CreateObject(terrain);
            Pick(terrainBought);
        }
    }

    // 在鼠标位置，从商店复制创建一个ClickableObject
    private T CreateObject<T>(T src) where T : ClickableObject
    {
		//GameObject prefab = resourceController.gameObjDictionary[src.gameObject.name];
		//T ret = Instantiate(prefab).GetComponent<T>();
		T ret = Instantiate(src.gameObject).GetComponent<T>();
		ret.name = src.name;
        ret.transform.position = MouseController.MouseWorldPosition();
        return ret;
    }

    // 钱不够
    void MoneyNotEnough()
    {

    }

    // 出售
    void Sell(ClickableObject clickableObject)
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
        else if (clickableObject is TerrainA && EditorMode == EditorMode.Terrain)
        {
            TerrainA terrain = clickableObject as TerrainA;
            Sell(terrain);
        }
    }

    // 出售Unit
    void Sell(Unit unit)
    {
        // 满足出售条件
        if (gameController.GamePhase == GamePhase.Editor ||
            gameController.GamePhase == GamePhase.Preparation && !unit.isEditorCreated)
        {
            if (gameController.GamePhase == GamePhase.Preparation)
            {
                PlayerMoney += unit.price;
            }
            // 设置unit所在格子为null
            if (MainGrid.InGrid(unit.coord))
            {
                MainGrid.Set(unit.coord, null);
            }
            Destroy(unit.gameObject);

            int rand = UnityEngine.Random.Range(0, resourceController.audiosDelete.Length);
            AudioSource.PlayClipAtPoint(resourceController.audiosDelete[rand], unit.transform.position);
        }
    }
    // 出售Background
    void Sell(Background background)
    {
        Backgrounds.Remove(background);
        Destroy(background.gameObject);
    }
    // 出售Terrain
    void Sell(TerrainA terrain)
    {
        Terrains.Remove(terrain);
        Destroy(terrain.gameObject);
    }

    // 播放Put音效
    private void PlayPutIntoGridSound(Unit unit)
    {
        int rand = UnityEngine.Random.Range(0, resourceController.audiosPut.Length);
        AudioSource.PlayClipAtPoint(resourceController.audiosPut[rand], unit.transform.position);
    }

    /// <summary>
    /// 根据Player和Unit获取Layer 
    /// </summary>
    Layer GetUnitLayer(Player player, Unit unit)
    {
        if (player == Player.Player)
        {
            return unit is Ball ? Layer.PlayerBall : Layer.PlayerBlock;
        }
        else if (player == Player.Enemy)
        {
            return unit is Ball ? Layer.EnemyBall : Layer.EnemyBlock;
        }
        else
        {
            return Layer.Default;
        }
    }

    // 初始化玩家钱数
    public void InitPlayerMoney()
    {
        PlayerMoney = PlayerMoneyOrigin;
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
                MouseObject.transform.position = MouseController.MouseWorldPosition() + mouseObjectOffset;
            }
        }
        // 编辑模式：Terrain
        else if (EditorMode == EditorMode.Terrain)
        {
            if (MouseObject != null)
            {
                // 鼠标物体跟随鼠标
                MouseObject.transform.position = MouseController.MouseWorldPosition() + mouseObjectOffset;
            }
        }
        // 编辑模式：Module
        else if (EditorMode == EditorMode.Module)
        {
            Coord mouseCoord = GetMouseCoord();
            if (mouseCoord != null && MouseModule != null)
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
    /// 将Unit放入(x,y), 设置位置。添加到gameController管理
    /// </summary>
    public void Put(Coord coord, Unit unit, Player player)
    {
        Debug.Assert(coord != Coord.OUTSIDE && unit != null);
        // 安放
        MainGrid.Set(coord, unit);
        unit.coord = coord;
        unit.transform.position = MainGrid.Coord2WorldPos(coord);
        unit.transform.parent = gameController.unitObjs.transform;
        unit.GetComponent<Collider2D>().enabled = (EditorMode == EditorMode.Unit);
        unit.isEditorCreated = (gameController.GamePhase == GamePhase.Editor);
        // 设置Unit所有者
        unit.player = player;
        // 设置Unit显示层
        Util.SetSpriteLayer(unit.gameObject, "Unit");
        // 设置Unit物理层
        unit.gameObject.layer = (int)GetUnitLayer(player, unit);
        // 设置红色
        if (unit is Ball)
        {
			if(unit.player == Player.Enemy)
			{
				Util.SetColor(unit.gameObject, Color.red);
			}
			else
			{
				Util.SetOriginColor(unit.gameObject);
			}
        }
    }

    /// <summary>
	/// 将Background添加到gameController管理
	/// </summary>
	public void Put(Background background)
    {
        Backgrounds.Add(background);
        background.transform.parent = gameController.backgroundObjects.transform;
        background.GetComponent<Collider2D>().enabled = (EditorMode == EditorMode.Background);
        Util.SetSpriteLayer(background.gameObject, "Background");
        background.gameObject.layer = (int)Layer.Background;
    }
    /// <summary>
    /// 将Terrain添加到gameController管理
    /// </summary>
    public void Put(TerrainA terrain)
    {
        Terrains.Add(terrain);
        terrain.transform.parent = gameController.terrainObjects.transform;
        terrain.GetComponent<Collider2D>().enabled = (EditorMode == EditorMode.Terrain);
        Util.SetSpriteLayer(terrain.gameObject, "Terrain");
        terrain.gameObject.layer = (int)Layer.Terrain;
    }

    // 摧毁鼠标上附着的Unit或Background
    void DestroyMouseObject()
    {
        if (MouseObject != null)
        {
            Destroy(MouseObject.gameObject);
        }
    }

    /// <summary>
    /// 清空所有Editor阶段创造的所有物品
    /// </summary>
    public void Clear()
    {
        // 清空网格中的单位
        MainGrid.ClearUnits();

        // 清空背景
        ClearBackground();

        // 清空地形
        ClearTerrain();
    }

    // 清空背景
    void ClearBackground()
    {
        foreach (Background background in Backgrounds)
        {
            Destroy(background.gameObject);
        }
        Backgrounds.Clear();
    }

    // 清空地形
    void ClearTerrain()
    {
        foreach (TerrainA terrain in terrains)
        {
            Destroy(terrain.gameObject);
        }
        Terrains.Clear();
    }

    /// <summary>
    /// 返回鼠标的MainGrid网格坐标
    /// 返回null: 如果不在网格中
    /// </summary>
    Coord GetMouseCoord()
    {
        return MainGrid.GetClosestCoord(MouseController.MouseWorldPosition());
    }

    /// <summary>
    /// 切换Editor Mode时，启用或禁用Unit、Background、Terrain的Collider，来接受或拒绝鼠标点击
    /// </summary>
    void UpdateObjectsCollider(EditorMode editorMode)
    {
        // Unit的Collider，只有在Editor Mode为Unit时才启用
        SetUnitsCollider(editorMode == EditorMode.Unit);

        // Background的Collider，只有在Editor Mode为Background时才启用
        SetBackgroundsCollider(editorMode == EditorMode.Background);

        // Terrain的Collider，只有在Editor Mode为Terrain时才启用
        SetTerrainsCollider(editorMode == EditorMode.Terrain);
    }

    // 启用或禁用Unit的Collider
    void SetUnitsCollider(bool active)
    {
        foreach (Unit unit in gameController.GetUnits())
        {
            unit.GetComponent<Collider2D>().enabled = active;
        }
    }
    // 启用或禁用Background的Collider
    void SetBackgroundsCollider(bool active)
    {
        foreach (Background background in editorController.backgrounds)
        {
            background.GetComponent<Collider2D>().enabled = active;
        }
    }
    // 启用或禁用Terrain的Collider
    void SetTerrainsCollider(bool active)
    {
        foreach (TerrainA terrain in editorController.terrains)
        {
            terrain.GetComponent<Collider2D>().enabled = active;
        }
    }

    void MyDebug()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log(GetMouseCoord().x + " " + GetMouseCoord().y);
        }
    }

    #region Obselete
    // 连接所有Block
    //[Obsolete("All blocks are in the grid now. Use LinkAllBlocksInGrid() instead.")]
    //public void LinkAllBlocks()
    //{
    //    Unit[] units = gameController.GetUnits("Block");
    //    foreach (Unit unit in units)
    //    {
    //        Block block = unit as Block;
    //        for (int direction = 0; direction < 4; direction++)
    //        {
    //            if (block.IsLinkAvailable(direction))
    //            {
    //                Block another = GetLinkableBlockByDirection(block, direction);
    //                // 连接
    //                if (another != null)
    //                {
    //                    Link(block, another, direction);
    //                }
    //            }
    //        }
    //    }
    //}

    // 检测该方向范围内是否有可连接的Block
    //Block GetLinkableBlockByDirection(Block block, int direction)
    //{
    //    // 检测连接距离
    //    float distanceCheck = 0.3f;

    //    int directionNeg = Negative(direction);

    //    Unit[] units = gameController.GetUnits("Block");
    //    foreach (Unit unit in units)
    //    {
    //        Block another = unit as Block;

    //        if (another != block &&
    //            // 所属同一名玩家
    //            block.player == another.player &&
    //            // Another的位置可以连接
    //            another.IsLinkAvailable(directionNeg) == true)
    //        {
    //            // Another的连接点
    //            Vector2 absorptionPoint = LinkPoint(another, directionNeg);

    //            // Block与连接点的距离
    //            float distance = ((Vector2)block.transform.position - absorptionPoint).magnitude;

    //            // 满足吸附距离
    //            if (distance <= distanceCheck)
    //            {
    //                // 返回满足的Another
    //                return another;
    //            }
    //        }
    //    }
    //    return null;
    //}
    // 按照方向返回Block的连接点
    //public Vector2 LinkPoint(Block block, int direction)
    //{
    //	Vector2 point = block.transform.position;
    //	switch (direction)
    //	{
    //		case 0:
    //			point += Vector2.right * 2 * block.radius;
    //			break;
    //		case 1:
    //			point += Vector2.up * 2 * block.radius;
    //			break;
    //		case 2:
    //			point += Vector2.left * 2 * block.radius;
    //			break;
    //		case 3:
    //			point += Vector2.down * 2 * block.radius;
    //			break;
    //	}
    //	return point;
    //}
    #endregion
}
