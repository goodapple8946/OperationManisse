using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject corePrefab;

	// 玩家所有物体的根节点
    public GameObject playerObjects;
    private GameObject playerObjectsSaved;
    private GameObject playerObjectsInit;

	// 敌人物体的根节点
	public GameObject enemyObjects;
    private GameObject enemyObjectsSaved;

    // Controller
    private CameraController mainCamera;
    private VictoryController victoryController;

    public enum Layer { Default, TransparentFX, IgnoreRaycast, Water = 4, UI, PlayerBall = 8, PlayerBlock, PlayerMissile, EnemyBall, EnemyBlock, EnemyMissile, Goods, Ground }
    public enum GamePhase { Menu, Preparation, Playing, Victory, Defeat, Pause }

    public GamePhase gamePhase;

    private Vector2 coreOriginPosition = new Vector2(0f, 0.64f);

    // 准备阶段放置区右边界
    public float boundRightPreparation;

    // 玩家原始钱数
    private int playerMoneyOrigin;

    // 玩家钱数
    public int playerMoney;

    // 玩家显示钱数Text
    private Text playerMoneyText;

    // Ctrl键是否被按下
    public bool keyCtrl;

    // 当前鼠标拖动的Unit
    public ArrayList unitsDraging = new ArrayList();

    // 上一次购买的物品时，新生成的商品
    public Unit unitBought;

    // 准备阶段的建造范围
    public float xMinBuild;
    public float xMaxBuild;
    public float yMinBuild;
    public float yMaxBuild;

    void Awake()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<CameraController>();
        victoryController = GameObject.Find("Victory Controller").GetComponent<VictoryController>();
        playerMoneyText = GameObject.Find("UI Canvas/UI Money Text").GetComponent<Text>();
    }

    void Start()
    {
        gamePhase = GamePhase.Preparation;
        playerMoneyOrigin = playerMoney;

        SaveBeforeStart();
        NewGame();
    }

    void Update()
    {
        // 鼠标键抬起
        if (Input.GetMouseButtonUp(0))
        {
            FixBlockBodyType();
        }

        // 空格键开始或停止
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            StartOrStop();
        }

        // Q键快速重复上一次购买的物体
        if (Input.GetKeyDown(KeyCode.Q) && gamePhase == GamePhase.Preparation)
        {
            RepeatBuy();
        }

        // Ctrl键按下
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            keyCtrl = true;
        }

        // Ctrl键抬起
        if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
        {
            keyCtrl = false;
        }

        if (gamePhase == GamePhase.Playing)
        {
            VictoryCheck();
        }
        DebugGame();
    }

    void FixedUpdate()
    {
        UpdatePlayerMoney();
    }

    // 开始前保存场景
    void SaveBeforeStart()
    {
        playerObjectsSaved = GameObject.Find("Player Objects");
        playerObjectsSaved.SetActive(false);
        playerObjectsInit = Instantiate(playerObjectsSaved);
        playerObjectsInit.SetActive(false);

        enemyObjectsSaved = GameObject.Find("Enemy Objects");
        enemyObjectsSaved.SetActive(false);
    }

    // 根据位置及预设类型创建物体（Unit）
    public GameObject Create(Vector2 position, GameObject prefab)
    {
        // 创建实例
        GameObject gameObject = Instantiate(prefab);

        // 设置名称
        gameObject.name = prefab.name;

        // 移动位置
        gameObject.transform.position = position;

        // 添加入playerObjects
        gameObject.transform.parent = playerObjects.transform;

        return gameObject;
    }

    // 空格键开始或停止游戏
    void StartOrStop()
    {
        // 开始游戏
        if (gamePhase == GamePhase.Preparation)
        {
            StartGame();
        }
        // 停止游戏
        else if (gamePhase == GamePhase.Playing)
        {
            StopGame();
        }
    }

    // Q键快速重复上一次购买的物体
    void RepeatBuy()
    {
        if (unitBought != null)
        {
            Unit unit = unitBought.Buy();
            if (unit != null)
            {
                unit.transform.position = MouseController.MouseWorldPosition();
                unit.MouseLeftUp();
            }
        }
    }

    // 开始游戏
    public void StartGame()
    {
        gamePhase = GamePhase.Playing;

        // 保存玩家的物体
        Destroy(playerObjectsSaved);
        playerObjectsSaved = Instantiate(playerObjects);
        playerObjectsSaved.SetActive(false);

        // 遍历Block
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject gameObject in gameObjects)
        {
            Block block = gameObject.GetComponent<Block>();

            if (block.isAlive && !block.isSelling)
            {
                // 不再固定Block的Z轴
                if (!block.isFixed && block.body != null)
                {
                    block.body.constraints = RigidbodyConstraints2D.None;
                }

                // 添加粒子预设
                if (block.particle == null && block.particlePrefab != null)
                {
                    block.particle = Instantiate(block.particlePrefab);
                    block.particle.transform.position = block.transform.position;
                    block.particle.transform.parent = block.transform;
                }
            }
        }
    }

    // 停止游戏
    public void StopGame()
    {
        gamePhase = GamePhase.Preparation;

        NewGame();
    }

    // 新游戏
    void NewGame()
    {
        // 重置摄像机
        mainCamera.Init();

        // 清除放置的物体
        if (playerObjects != null)
        {
            Destroy(playerObjects);
        }
        if (enemyObjects != null)
        {
            Destroy(enemyObjects);
        }

        // 放置玩家保存的物体
        playerObjects = Instantiate(playerObjectsSaved);
        playerObjects.SetActive(true);
        playerObjects.name = "Player Objects";

        // 放置敌人保存的物体
        enemyObjects = Instantiate(enemyObjectsSaved);
        enemyObjects.SetActive(true);
        enemyObjects.name = "Enemy Objects";
        EnemyBlockLink();

        victoryController.Init();
    }

    // 玩家物体初始化
    public void Reset()
    {
        Destroy(playerObjectsSaved);
        playerObjectsSaved = Instantiate(playerObjectsInit);
        playerObjectsSaved.SetActive(false);

        playerMoney = playerMoneyOrigin;

        NewGame();
    }

    // 更新玩家钱数
    void UpdatePlayerMoney()
    {
        int textMoney = int.Parse(playerMoneyText.text);
        int difference = Mathf.Abs(playerMoney - textMoney);

        // 每次变化值
        int deltaMoney = 1;

        if (difference > 10000)
        {
            deltaMoney = 10000;
        }
        else if (difference > 1000)
        {
            deltaMoney = 1000;
        }
        else if (difference > 100)
        {
            deltaMoney = 100;
        }
        else if (difference > 10)
        {
            deltaMoney = 10;
        }

        if (textMoney < playerMoney)
        {
            playerMoneyText.text = (textMoney + deltaMoney).ToString();
        }
        else if (textMoney > playerMoney)
        {
            playerMoneyText.text = (textMoney - deltaMoney).ToString();
        }
    }

    // 钱不够提示
    public IEnumerator MoneyNotEnough()
    {
        playerMoneyText.color = Color.red;

        yield return new WaitForSeconds(0.5f);

        playerMoneyText.color = Color.white;
    }

    // 跳跃
    void Jump()
    {
        if (gamePhase == GamePhase.Playing && Input.GetMouseButtonDown(0))
        {
            // 所有玩家的Ball跳跃
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Ball");
            foreach(GameObject gameObject in gameObjects)
            {
                Ball ball = gameObject.GetComponent<Ball>();
                if (ball.body != null && ball.player == 1 && ball.IsGrounded())
                {
                    ball.body.AddForce(new Vector2(50f, 50f));
                }
            }
        }
    }

    // 敌人Block连接，并且不再固定
    void EnemyBlockLink()
    {
        // 遍历Block
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject gameObject in gameObjects)
        {
            Block block = gameObject.GetComponent<Block>();

            if (block.isAlive && !block.isSelling && block.player == 2)
            {
                block.AbsorptionCheck(0.3f);
            }
        }
        foreach (GameObject gameObject in gameObjects)
        {
            Block block = gameObject.GetComponent<Block>();

            if (block.isAlive && !block.isSelling && block.player == 2)
            {
                block.body.bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }

    // 这是一个无可奈何的丑陋的补丁：鼠标抬起时，修正正在拖动Block的BodyType
    void FixBlockBodyType()
    {
        foreach (Unit unit in unitsDraging)
        {
            if (unit.body != null)
            {
                unit.body.bodyType = RigidbodyType2D.Dynamic;
            }
        }
        unitsDraging.Clear();
    }

    // 胜利检测
    void VictoryCheck()
    {
        if (victoryController.IsVictory())
        {
            gamePhase = GamePhase.Victory;
            Debug.Log("Victory");
        }
    }

    // Debug
    void DebugGame()
    {
        // 左Alt + 鼠标左键取鼠标位置
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftAlt))
        {
            Debug.Log(MouseController.MouseWorldPosition());
        }

        // 点击跳跃
        //Jump();
    }
}
