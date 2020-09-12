using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject corePrefab;
    public GameObject playerObjectsPrefab;

    public GameObject playerObjects;
    private GameObject playerObjectsSaved;
    public GameObject enemyObjects;
    private GameObject enemyObjectsSaved;

    private CameraController cameraController;

    public enum Layer { Default, TransparentFX, IgnoreRaycast, Water = 4, UI, PlayerUnit = 8, PlayerMissile, EnemyUnit, EnemyMissle, Entity, Goods }
    public enum GamePhase { Menu, Preparation, Playing, Victory, Defeat, Pause }

    public static GamePhase gamePhase;

    private Vector2 coreOriginPosition = new Vector2(0f, 0.64f);

    void Start()
    {
        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
        enemyObjectsSaved = GameObject.Find("Enemy Objects");
        enemyObjectsSaved.SetActive(false);
        gamePhase = GamePhase.Preparation;

        NewGame();
    }

    void Update()
    {
        SpaceStart();
        Jump();
    }

    /// <summary>
    /// 根据位置及预设类型创建物体（Unit）
    /// </summary>
    /// <param name="position">位置</param>
    /// <param name="blockPrefab">预设</param>
    /// <returns>创建的物体</returns>
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

        // Layer
        gameObject.layer = (int)Layer.PlayerUnit;

        return gameObject;
    }

    // 开始或重新开始游戏
    void SpaceStart()
    {
        // 开始游戏
        if (gamePhase == GamePhase.Preparation && Input.GetKeyDown(KeyCode.Space))
        {
            gamePhase = GamePhase.Playing;

            // 保存玩家的物体
            if (playerObjectsSaved != null)
            {
                Destroy(playerObjectsSaved);
            }
            playerObjectsSaved = Instantiate(playerObjects);
            playerObjectsSaved.SetActive(false);

            // 不再固定Block的Z轴
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Block");
            foreach (GameObject gameObject in gameObjects)
            {
                Block block = (Block)gameObject;
                if(!block.isFixed && block.body != null)
                {
                    block.body.constraints = RigidbodyConstraints2D.None;
                }
            }
        }
        // 回到准备阶段
        else if (gamePhase == GamePhase.Playing && Input.GetKeyDown(KeyCode.Space))
        {
            gamePhase = GamePhase.Preparation;

            NewGame();
        }
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
        if (playerObjectsSaved != null)
        {
            playerObjects = playerObjectsSaved;
            playerObjects.SetActive(true);
            playerObjectsSaved = null;
        }
        else
        {
            playerObjects = Instantiate(playerObjectsPrefab);
            
            // 创建Core
            Create(coreOriginPosition, corePrefab);
        }
        playerObjects.name = "Player Objects";

        // 放置敌人保存的物体
        enemyObjects = Instantiate(enemyObjectsSaved);
        enemyObjects.SetActive(true);
        enemyObjects.name = "Enemy Objects";
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
                Ball ball = (Ball)gameObject;
                if (ball.body != null && ball.player == 1 && ball.IsGrounded())
                {
                    ball.body.AddForce(new Vector2(50f, 50f));
                }
            }
        }
    }
}
