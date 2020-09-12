using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject battleObjectsPrefab;
    public GameObject entityPrefab;
    public GameObject blockGreyPrefab;
    public GameObject ballBluePrefab;
    public GameObject corePrefab;

    public GameObject battleObjects;

    private CameraController cameraController;

    public enum Layer { Default, TransparentFX, IgnoreRaycast, Water = 4, UI, PlayerUnit = 8, PlayerMissile, EnemyUnit, EnemyMissle, Entity, Goods }
    public enum GamePhase { Menu, Preparation, Playing, Victory, Defeat, Pause }

    public static GamePhase gamePhase;

    // 商店初始位置
    private Vector2 shopOriginPosition = new Vector2(-5.02f, 20.8f);

    void Start()
    {
        cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
        gamePhase = GamePhase.Preparation;
    }

    void Update()
    {
        AltEntityMove();
        SpaceStart();
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

        // 添加入battleObjects
        gameObject.transform.parent = battleObjects.transform;

        // Layer
        gameObject.layer = (int)Layer.PlayerUnit;

        return gameObject;
    }

    /// <summary>
    /// 创建Entity
    /// </summary>
    /// <returns>创建的Entity</returns>
    public Entity CreateEntity()
    {
        // 创建Entity实例
        Entity entity = (Entity)Instantiate(entityPrefab);

        entity.name = entityPrefab.name;
        entity.transform.parent = battleObjects.transform;

        return entity;
    }

    /// <summary>
    /// 更新所有Block的Entity
    /// </summary>
    public void UpdateBlockEntity()
    {
        ClearEntity();

        // 遍历所有Block
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject gameObject in gameObjects)
        {
            Block block = (Block)gameObject;

            if (block.alive && block.tag != "Goods" && block.entity == null)
            {
                Entity entity = CreateEntity();
                entity.transform.position = block.transform.position;

                // 设置所有与该Block连接的Block的Entity
                SetBlockLinkedEntity(block, entity);
            }
        }
    }

    // 设置所有与该Block连接的Block的Entity
    public void SetBlockLinkedEntity(Block block, Entity entity)
    {
        block.SetEntity(entity);
        foreach (Block blockLinked in block.blocksLinked)
        {
            if (blockLinked != null && blockLinked.alive && blockLinked.entity == null)
            {
                SetBlockLinkedEntity(blockLinked, entity);
            }
        }
    }

    /// <summary>
    /// 清除所有Entity，保留Block
    /// </summary>
    public void ClearEntity()
    {
        // 清除所有Entity
        GameObject[] gameObjects;
        gameObjects = GameObject.FindGameObjectsWithTag("Entity");
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.transform.DetachChildren();
            Destroy(gameObject);
        }

        // 将所有Block的Entity设置为null
        gameObjects = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject gameObject in gameObjects)
        {
            Block block = (Block)gameObject;
            block.SetEntity(null);
        }
    }

    /// <summary>
    /// 清除没有Block的Entity
    /// </summary>
    public void DestroyEmptyEntity()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Entity");

        foreach (GameObject gameObject in gameObjects)
        {
            Entity entity = (Entity)gameObject;
            if (entity.GetComponentsInChildren<Block>().Length == 0)
            {
                Destroy(entity);
            }
        }
    }

    /// <summary>
    /// 清除并重建BattleObjects
    /// </summary>
    public void ClearBattleObjects()
    {
        Destroy(battleObjects);
        battleObjects = Instantiate(battleObjectsPrefab);
        battleObjects.name = battleObjectsPrefab.name;
    }

    // 获得或失去Rigidbody
    private void SetRigidbody(GameObject gameObject, bool getRigidbody)
    {
        if (getRigidbody)
        {
            Rigidbody2D body = gameObject.GetComponent<Rigidbody2D>();
            if (body == null)
            {
                body = gameObject.AddComponent<Rigidbody2D>();
                body.useAutoMass = true;
            }
            body.bodyType = RigidbodyType2D.Dynamic;
        }
        else
        {
            Rigidbody2D body = gameObject.GetComponent<Rigidbody2D>();
            if (body != null)
            {
                Destroy(body);
            }
        }
    }

    // Entity整体移动控制
    private void AltEntityMove()
    {
        if (gamePhase == GamePhase.Preparation)
        {
            // 按下Alt，关闭Entity整体移动
            if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
            {
                GameObject[] gameObjects;

                // Entity失去Rigidbody
                gameObjects = GameObject.FindGameObjectsWithTag("Entity");
                foreach (GameObject gameObject in gameObjects)
                {
                    SetRigidbody(gameObject, false);
                }

                // Block获得Rigidbody
                gameObjects = GameObject.FindGameObjectsWithTag("Block");
                foreach (GameObject gameObject in gameObjects)
                {
                    SetRigidbody(gameObject, true);
                }
            }

            // 抬起Alt，开启Entity整体移动
            if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
            {
                GameObject[] gameObjects;

                // Entity获得Rigidbody
                gameObjects = GameObject.FindGameObjectsWithTag("Entity");
                foreach (GameObject gameObject in gameObjects)
                {
                    SetRigidbody(gameObject, false);
                }

                // Block失去Rigidbody
                gameObjects = GameObject.FindGameObjectsWithTag("Block");
                foreach (GameObject gameObject in gameObjects)
                {
                    SetRigidbody(gameObject, true);
                }
            }
        }
    }

    // 开始或重新开始游戏
    private void SpaceStart()
    {
        GameObject[] gameObjects;

        // 开始游戏
        if (gamePhase == GamePhase.Preparation && Input.GetKeyDown(KeyCode.Space))
        {
            gamePhase = GamePhase.Playing;

            // Entity获得Rigidbody，类型为Dynamic
            gameObjects = GameObject.FindGameObjectsWithTag("Entity");
            foreach (GameObject gameObject in gameObjects)
            {
                Entity entity = (Entity)gameObject;
                if (entity.body == null)
                {
                    entity.body = entity.gameObject.AddComponent<Rigidbody2D>();
                }
                entity.body.bodyType = RigidbodyType2D.Dynamic;
                entity.body.useAutoMass = true;
            }
        }
        // 回到准备阶段
        else if (gamePhase == GamePhase.Playing && Input.GetKeyDown(KeyCode.Space))
        {
            gamePhase = GamePhase.Preparation;

            // 清除放置的物体
            ClearBattleObjects();

            // 创建Core
            Core core = (Core)Create(Vector2.zero, corePrefab);
            core.Init();

            cameraController.Init();
            cameraController.core = core;
        }
    }
}
