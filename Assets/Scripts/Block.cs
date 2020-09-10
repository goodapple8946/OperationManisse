using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : Unit
{
    private GameController gameController;

    public static int count = 0;

    // Block的尺寸
    private float size = 0.6f;

    // 吸附半径
    // 拖动Block时，如果与其他Block的吸附点距离较近时，则连接两者，并且调整拖动Block的位置
    private float adsorptionDistance = 0.3f;

    // 可吸附的
    protected bool absorbable = true;

    // 连接其他Block的方向
    public enum LinkDirection { Right, Up, Left, Down }

    // 四周连接的其他Block
    public Block[] blocksLinked = new Block[4];

    // 该Block所处的Entity，即该Block的父物体
    // 功能上，“entity”等效于“(Entity)transform.parent.gameObject”
    // 不能为null
    // 修改时，使用SetEntity，将同步修改父物体
    public Entity entity;

    // 将GameObject强制类型转换为Block
    public static explicit operator Block(GameObject gameObject)
    {
        return gameObject.GetComponent<Block>();
    }

    protected override void Start()
    {
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();

        base.Start();

        health = 10;
    }

    protected override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// 将目标Block按照方向连接到该Block
    /// </summary>
    /// <param name="block">目标Block</param>
    /// <param name="direction">相对于该Block，目标Block所处的方向</param>
    public void LinkBlock(Block block, LinkDirection direction)
    {
        // 目标Block所处方向的反方向
        LinkDirection directionNegative = (LinkDirection)(((int)direction + 2) % 4);

        // 该Block连接目标Block
        blocksLinked[(int)direction] = block;

        // 目标Block连接该Block
        block.blocksLinked[(int)directionNegative] = (Block)gameObject;

        // 更新所有Block的Entity
        gameController.UpdateBlockEntity();
    }

    /// <summary>
    /// 将该Block与所有连接的Block断开连接
    /// </summary>
    public void Unlink()
    {
        // 遍历所有与该Block连接的Block
        for (int directionIndex = 0; directionIndex < 4; directionIndex++)
        {
            Block block = blocksLinked[directionIndex];
            if (block != null)
            {
                // 连接的反方向
                int directionNegativeIndex = (directionIndex + 2) % 4;

                // 断开连接
                blocksLinked[directionIndex] = null;
                block.blocksLinked[directionNegativeIndex] = null;
            }
        }

        // 更新所有Block的Entity
        gameController.UpdateBlockEntity();
    }

    // 死亡
    protected override IEnumerator Die()
    {
        alive = false;

        // 更新所有Block的Entity
        gameController.UpdateBlockEntity();

        return base.Die();
    }

    /// <summary>
    /// 将该Block的Entity修改为目标Entity，同步修改父物体
    /// </summary>
    /// <param name="entity">目标Entity</param>
    public void SetEntity(Entity entity)
    {
        if (entity != null)
        {
            transform.parent = entity.transform;
        }
        this.entity = entity;
    }

    /*
     * Block吸附的过程为：
     * 1. 拖动Block时，对四个方向进行AdsorptionCheck
     * 2. 松开Block时，如果AdsorptionCheck返回值不为null，进行Absorb
     */

    /// <summary>
    /// 根据吸附方向返回Block的吸附点
    /// </summary>
    /// <param name="linkDirection">吸附方向</param>
    /// <returns>吸附点</returns>
    public Vector2 AdsorptionPoint(LinkDirection direction)
    {
        Vector2 point = transform.position;
        switch (direction)
        {
            case LinkDirection.Right:
                point += Vector2.right * size;
                break;
            case LinkDirection.Up:
                point += Vector2.up * size;
                break;
            case LinkDirection.Left:
                point += Vector2.left * size;
                break;
            case LinkDirection.Down:
                point += Vector2.down * size;
                break;
        }
        return point;
    }



    /// <summary>
    /// 检测所有Block的吸附点
    /// </summary>
    /// <param name="direction">检测方向</param>
    /// <param name="checkDistance">检测吸附半径</param>
    /// <returns>满足吸附检测的Block，如果没有则为null</returns>
    public Block AdsorptionCheck(LinkDirection direction, float checkDistance)
    {
        // 吸附方向，即检测方向的反方向
        LinkDirection directionNegative = (LinkDirection)(((int)direction + 2) % 4);

        // 找到所有Block
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Block");

        // 遍历所有Block
        foreach (GameObject gameObject in gameObjects)
        {
            Block block = (Block)gameObject;

            // 满足遍历到的Block吸附位置没有被占用
            if (block.blocksLinked[(int)directionNegative] == null)
            {
                // 遍历到的Block吸附点
                Vector2 adsorptionPoint = block.AdsorptionPoint(directionNegative);

                // 该Block与吸附点的距离
                float distance = ((Vector2)transform.position - adsorptionPoint).magnitude;

                // 满足吸附距离
                if (distance <= checkDistance)
                {
                    // 返回满足的Block
                    return block;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 将该Block与目标Block根据方向吸附并连接
    /// </summary>
    /// <param name="block">目标Block</param>
    /// <param name="direction">检测方向</param>
    public void Absorb(Block block, LinkDirection direction)
    {
        // 吸附方向，即检测方向的反方向
        LinkDirection directionNegative = (LinkDirection)(((int)direction + 2) % 4);

        Vector2 position = block.transform.position;
        switch (directionNegative)
        {
            case LinkDirection.Right:
                position += Vector2.right * size;
                break;
            case LinkDirection.Up:
                position += Vector2.up * size;
                break;
            case LinkDirection.Left:
                position += Vector2.left * size;
                break;
            case LinkDirection.Down:
                position += Vector2.down * size;
                break;
        }

        // 调整该Block位置
        transform.position = position;

        // 连接Block
        LinkBlock(block, direction);
    }

    // 拖动
    private void Drag()
    {
        transform.position += (Vector3)MouseController.offset;
    }

    private void OnMouseDown()
    {
        if (GameController.gamePhase == GameController.GamePhase.Preparation && Input.GetMouseButton(0))
        {
            Unlink();
        }
    }

    private void OnMouseDrag()
    {
        if (GameController.gamePhase == GameController.GamePhase.Preparation && Input.GetMouseButton(0))
        {
            Drag();
        }
    }

    private void OnMouseUp()
    {
        if (GameController.gamePhase == GameController.GamePhase.Preparation)
        {
            // 临时吸附半径
            // 当有一个方向成功吸附后，吸附要求应该变得严格，即吸附半径变为0
            float adsorptionDistanceTemp = adsorptionDistance;

            // 遍历四个检测方向
            for (int i = 0; i < 4; i++)
            {
                // 检测方向
                LinkDirection direction = (LinkDirection)i;

                // 吸附检测
                Block block = AdsorptionCheck(direction, adsorptionDistanceTemp);

                // 吸附并连接
                if (block != null)
                {
                    Absorb(block, direction);
                    adsorptionDistanceTemp = 0;
                }
            }
        }
    }
}
