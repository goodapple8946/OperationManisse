using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject corePrefab;

    public GameObject playerObjects;
    private GameObject playerObjectsSaved;
    private GameObject playerObjectsInit;
    public GameObject enemyObjects;
    private GameObject enemyObjectsSaved;

    private CameraController cameraController;

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

    void Start()
    {
        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
        playerMoneyText = GameObject.Find("UI Canvas/UI Money Text").GetComponent<Text>();

        gamePhase = GamePhase.Preparation;
        playerMoneyOrigin = playerMoney;

        SaveBeforeStart();
        NewGame();
    }

    void Update()
    {
        SpaceStart();
        DebugGame();
        KeyCtrlCheck();
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
    void SpaceStart()
    {
        // 开始游戏
        if (gamePhase == GamePhase.Preparation && Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
        // 停止游戏
        else if (gamePhase == GamePhase.Playing && Input.GetKeyDown(KeyCode.Space))
        {
            StopGame();
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
        cameraController.Init();

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

    // 检测Ctrl键是否被按下
    void KeyCtrlCheck()
    {
        
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            keyCtrl = true;
        }
        else
        {
            keyCtrl = false;
        }
    }

    // Debug
    void DebugGame()
    {
        // 点击获取鼠标位置
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Debug.Log(MouseController.MouseWorldPosition());
        //}

        // 点击跳跃
        //Jump();
    }
}
