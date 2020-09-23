using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public enum Layer { Default, TransparentFX, IgnoreRaycast, Water = 4, UI, PlayerBall = 8, PlayerBlock, PlayerMissile, EnemyBall, EnemyBlock, EnemyMissile, Goods, Ground }
    public enum GamePhase { Menu, Preparation, Playing, Victory, Defeat, Pause }

    // 玩家所有物体的根节点
    public GameObject playerObjects;
    private GameObject playerObjectsSaved;
    private GameObject playerObjectsInit;

    // 敌人物体的根节点
    public GameObject enemyObjects;
    private GameObject enemyObjectsSaved;

    // Controller
    private CameraController cameraController;
    private VictoryController victoryController;
    private PreparationController preparationController;

    // 玩家显示钱数Text
    private Text playerMoneyText;

    public GamePhase gamePhase;

    // 玩家原始钱数
    private int playerMoneyOrigin;

    // 玩家钱数
    public int playerMoney;

    // 商店
    private GameObject shop;

    // 胜利音效
    public AudioClip audioVictory;

    // 胜利对话框
    private GameObject victoryDialog;

    void Awake()
    {
        playerObjects = GameObject.Find("Player Objects");
        enemyObjects = GameObject.Find("Enemy Objects");
        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
        victoryController = GameObject.Find("Victory Controller").GetComponent<VictoryController>();
        preparationController = GameObject.Find("Preparation Controller").GetComponent<PreparationController>();
        playerMoneyText = GameObject.Find("UI Canvas/UI Money Text").GetComponent<Text>();
        victoryDialog = GameObject.Find("UI Canvas/UI Victory");
        shop = GameObject.Find("UI Canvas/UI Shop");

        victoryDialog.SetActive(false);

        gamePhase = GamePhase.Preparation;
    }

	void Start()
    {
		SaveBeforeStart();
        NewGame();
    }

    void Update()
    {
        // 空格键开始或停止
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            StartOrStop();
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
        // 保存玩家钱数
        playerMoneyOrigin = playerMoney;

        // 保存玩家物体
        playerObjectsSaved = Instantiate(playerObjects);
        playerObjectsSaved.SetActive(false);
        playerObjectsInit = Instantiate(playerObjectsSaved);
        playerObjectsInit.SetActive(false);

        // 保存敌人物体
        enemyObjectsSaved = Instantiate(enemyObjects);
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

    // 开始游戏
    public void StartGame()
    {
        gamePhase = GamePhase.Playing;

        Unit mouseUnit = preparationController.mouseUnit;
        if (mouseUnit != null)
        {
            playerMoney += mouseUnit.price;
            Destroy(mouseUnit.gameObject);
        }

        // 保存玩家的物体
        Destroy(playerObjectsSaved);
        playerObjectsSaved = Instantiate(playerObjects);
        playerObjectsSaved.SetActive(false);

        // Block连接
        // preparationController.LinkAllBlocksInGrid();
        preparationController.LinkAllBlocks();

        // 向所有物体发送消息
        playerObjects.BroadcastMessage("GameStart");
        enemyObjects.BroadcastMessage("GameStart");

        // 隐藏商店
        shop.SetActive(false);

        // 隐藏网格
        preparationController.gameObject.SetActive(false);
    }

    // 停止游戏
    public void StopGame()
    {
        gamePhase = GamePhase.Preparation;

        NewGame();
    }

    // 新游戏
    private void NewGame()
    {
		// 重置摄像机
		cameraController.Init();

        // 显示商店
        shop.SetActive(true);

        // 显示网格
        preparationController.gameObject.SetActive(true);

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

        // 网格Unit安放
        preparationController.PutAllUnits();

        // 放置敌人保存的物体
        enemyObjects = Instantiate(enemyObjectsSaved);
        enemyObjects.SetActive(true);
        enemyObjects.name = "Enemy Objects";

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

    // 胜利检测
    void VictoryCheck()
    {
        if (victoryController.isVictory)
        {
            gamePhase = GamePhase.Victory;
            StartCoroutine(Victory());
        }
    }

    // 胜利
    IEnumerator Victory()
    {
        float victoryWaitTime = 2f;

        yield return new WaitForSeconds(victoryWaitTime);

        victoryDialog.SetActive(true);
        AudioSource.PlayClipAtPoint(audioVictory, cameraController.transform.position);
        cameraController.movable = false;
    }

    // 玩家物体中心
    public Vector2 GetCenterPlayerObjects()
    {
        Vector2 center = Vector2.zero;
        int len = playerObjects.transform.childCount;
        int cnt = 0;
        for (int i = 0; i < len; i++)
        {
            Unit unit = playerObjects.transform.GetChild(i).GetComponent<Unit>();
            if (unit != null && unit.IsAlive())
            {
                center += (Vector2)unit.transform.position;
                cnt++;
            }
        }
        if (cnt != 0)
        {
            center /= cnt;
        }
        return center;
    }

    // 玩家物体平均速度
    public Vector2 GetVelocityPlayerObjects()
    {
        Vector2 velocity = Vector2.zero;
        int len = playerObjects.transform.childCount;
        int cnt = 0;
        for (int i = 0; i < len; i++)
        {
            Unit unit = playerObjects.transform.GetChild(i).GetComponent<Unit>();
            if (unit != null && unit.IsAlive() && unit.body != null)
            {
                velocity += unit.body.velocity;
                cnt++;
            }
        }
        if (cnt != 0)
        {
            velocity /= cnt;
        }
        return velocity;
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
