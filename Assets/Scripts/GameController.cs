using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject entityPrefab;
    public GameObject blockGreyPrefab;

    public enum GamePhase { Menu, Preparation, Playing, Victory, Defeat, Pause }

    public static GamePhase gamePhase;

    void Start()
    {
        gamePhase = GamePhase.Preparation;
    }

    void Update()
    {
        AltEntityMove();
        SpaceStart();
    }

    /// <summary>
    /// 根据位置及预设类型创建Block
    /// </summary>
    /// <param name="position">位置</param>
    /// <param name="blockPrefab">Block预设</param>
    /// <returns>创建的Block</returns>
    public Block CreateBlock(Vector2 position, GameObject blockPrefab)
    {
        // 创建Block实例
        Block block = (Block)Instantiate(blockPrefab);

        // 设置Block名称
        block.name = blockPrefab.name + Block.count;
        Block.count++;

        // 移动Block位置
        block.transform.position = position;

        // 创建Block的Entity
        block.SetEntity(CreateEntity());

        return block;
    }

    /// <summary>
    /// 创建Entity
    /// </summary>
    /// <returns>创建的Entity</returns>
    public Entity CreateEntity()
    {
        // 创建Entity实例
        Entity entity = (Entity)Instantiate(entityPrefab);

        // 设置Entity名称
        entity.name = entityPrefab.name + Entity.count;
        Entity.count++;

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

            if (block.alive && block.entity == null)
            {
                Entity entity = CreateEntity();

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
            block.entity = null;
        }
    }
    
    /// <summary>
    /// 清除所有Block及Entity
    /// </summary>
    public void Clear()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Entity");
        foreach (GameObject gameObject in gameObjects)
        {
            Destroy(gameObject);
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

    // Entity整体移动控制
    private void AltEntityMove()
    {
        if (gamePhase == GamePhase.Preparation)
        {
            // 按下Alt，开启Entity整体移动
            if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
            {
                GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Entity");
                foreach (GameObject gameObject in gameObjects)
                {
                    Rigidbody2D body = gameObject.AddComponent<Rigidbody2D>();
                    body.bodyType = RigidbodyType2D.Static;
                }
            }

            // 抬起Alt，关闭Entity整体移动
            if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
            {
                GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Entity");
                foreach (GameObject gameObject in gameObjects)
                {
                    Rigidbody2D body = gameObject.GetComponent<Rigidbody2D>();
                    if (body != null)
                    {
                        Destroy(body);
                    }
                }
            }
        }
    }

    // 开始或重新开始游戏
    private void SpaceStart()
    {
        // 开始游戏
        if (gamePhase == GamePhase.Preparation && Input.GetKeyDown(KeyCode.Space))
        {
            gamePhase = GamePhase.Playing;

            // Entity获得Rigidbody，类型为Dynamic
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Entity");
            foreach (GameObject gameObject in gameObjects)
            {
                Rigidbody2D body = gameObject.GetComponent<Rigidbody2D>();
                if (body == null)
                {
                    body = gameObject.AddComponent<Rigidbody2D>();
                }
                body.bodyType = RigidbodyType2D.Dynamic;
            }
        }
        // 重新开始游戏
        else if (gamePhase == GamePhase.Playing && Input.GetKeyDown(KeyCode.Space))
        {
            gamePhase = GamePhase.Preparation;

            Clear();
        }
    }
}
