using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : Unit
{
    // Block的尺寸
    private float size = 0.6f;

    // 吸附半径与严格吸附半径
    // 拖动Block时，如果与其他Block的吸附点距离较近时，则连接两者，并且调整拖动Block的位置
    public static float adsorptionDistance = 0.3f;
    public static float adsorptionDistanceStrict = 0.3f;

    // 连接其他Block的方向
    public enum LinkDirection { Right, Up, Left, Down }

    // 四周连接的其他Block
    public Block[] blocksLinked = new Block[4];

    // 四周连接的Joint
    public Joint2D[] joints = new Joint2D[4];

    // 提供吸附性，当吸附双方都为false时，不会发生吸附
    public bool absorptionProvision;

    // 遮罩预设
    public GameObject coverPrefab;

    // 用于检测的临时tag
    protected int checkTag;

    // 固定的
    public bool isFixed;

    // 是核心
    public bool isCore;

    // 将GameObject强制类型转换为Block
    public static explicit operator Block(GameObject gameObject)
    {
        return gameObject.GetComponent<Block>();
    }

    /// <summary>
    /// 将目标Block按照方向连接到该Block
    /// </summary>
    /// <param name="block">目标Block</param>
    /// <param name="direction">相对于该Block，目标Block所处的方向</param>
    protected void LinkBlock(Block block, LinkDirection direction)
    {
        // 目标Block所处方向的反方向
        LinkDirection directionNegative = (LinkDirection)(((int)direction + 2) % 4);

        // 该Block连接目标Block
        blocksLinked[(int)direction] = block;
        joints[(int)direction] = gameObject.AddComponent<FixedJoint2D>();
        joints[(int)direction].connectedBody = block.body;
        // joints[(int)direction].enableCollision = true;

        // 目标Block连接该Block
        block.blocksLinked[(int)directionNegative] = (Block)gameObject;
        block.joints[(int)directionNegative] = gameObject.AddComponent<FixedJoint2D>();
        block.joints[(int)directionNegative].connectedBody = body;
        // block.joints[(int)directionNegative].enableCollision = true;
    }

    /// <summary>
    /// 将该Block与所有连接的Block断开连接
    /// </summary>
    protected void Unlink()
    {
        // 已经购买的Block
        if (!isSelling)
        {
            // 遍历所有与该Block连接的Block
            for (int directionIndex = 0; directionIndex < 4; directionIndex++)
            {
                Block block = blocksLinked[directionIndex];
                if (block != null)
                {
                    // 连接的反方向
                    int directionNegativeIndex = (directionIndex + 2) % 4;

                    // 断开该Block
                    blocksLinked[directionIndex] = null;
                    if (joints[directionIndex] != null)
                    {
                        Destroy(joints[directionIndex]);
                    }

                    // 断开连接的Block
                    block.blocksLinked[directionNegativeIndex] = null;
                    if (block.joints[directionNegativeIndex] != null)
                    {
                        Destroy(block.joints[directionNegativeIndex]);
                    }

                    // 更新连接的遮罩
                    block.UpdateWheelCover();
                }
            }
            UpdateWheelCover();
        }
    }

    // 死亡
    protected override IEnumerator Die()
    {
        // 断开所有连接
        Unlink();

        return base.Die();
    }

    /*
     * Block吸附的过程为：
     * 1. 进行AdsorptionCheck
     * 2. AdsorptionCheck中，根据四个方向进行AdsorptionCheckDirection
     * 3. 如果AdsorptionCheckDirection返回值不为null，进行Absorb
     */

    // 根据吸附方向返回Block的吸附点
    protected Vector2 AdsorptionPoint(LinkDirection direction)
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

    // 吸附检测
    public void AdsorptionCheck()
    {
        // 临时吸附半径
        // 当有一个方向成功吸附后，吸附要求应该变得严格
        float adsorptionDistanceTemp = adsorptionDistance;

        // 遍历四个检测方向
        for (int i = 0; i < 4; i++)
        {
            // 检测方向
            LinkDirection direction = (LinkDirection)i;

            // 吸附检测
            Block block = AdsorptionCheckDirection(direction, adsorptionDistanceTemp);

            // 吸附并连接
            if (block != null)
            {
                Absorb(block, direction);

                // 吸附要求应该变得严格
                adsorptionDistanceTemp = adsorptionDistanceStrict;

                // 轮子只能吸附一个方向
                if (isWheel)
                {
                    break;
                }
            }
        }
    }

    // 根据方向检测该Block是否处于其他Block的吸附范围内
    protected Block AdsorptionCheckDirection(LinkDirection direction, float checkDistance)
    {
        // 吸附方向，即检测方向的反方向
        LinkDirection directionNegative = (LinkDirection)(((int)direction + 2) % 4);

        // 找到所有Block
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Block");

        // 遍历所有Block
        foreach (GameObject gameObject in gameObjects)
        {
            Block block = (Block)gameObject;

            if (
                // 所属同一名玩家
                player == block.player &&
                // 满足至少有一方提供吸附性
                (absorptionProvision || block.absorptionProvision) &&
                // 存活且非卖品
                block.isAlive && !isSelling &&
                // 满足遍历到的Block吸附位置没有被占用
                block.blocksLinked[(int)directionNegative] == null
                )
            {
                // 如果遍历到的Block是轮子
                if (block.isWheel)
                {
                    // 要保证这个轮子没有连接任何Block
                    bool cleanWheel = true;
                    foreach (Block checkBlock in block.blocksLinked)
                    {
                        if (checkBlock != null)
                        {
                            cleanWheel = false;
                            break;
                        }
                    }
                    // 这个轮子已经有连接的Block了，跳过
                    if (!cleanWheel)
                    {
                        continue;
                    }
                }
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
    /// <returns>吸附时经过的位移</returns>
    public Vector2 Absorb(Block block, LinkDirection direction)
    {
        // 吸附方向，即检测方向的反方向
        LinkDirection directionNegative = (LinkDirection)(((int)direction + 2) % 4);

        // 该Block的起始位置
        Vector2 startPosition = transform.position;

        // 该Block的目标位置
        Vector2 endPosition = block.transform.position;

        switch (directionNegative)
        {
            case LinkDirection.Right:
                endPosition += Vector2.right * size;
                break;
            case LinkDirection.Up:
                endPosition += Vector2.up * size;
                break;
            case LinkDirection.Left:
                endPosition += Vector2.left * size;
                break;
            case LinkDirection.Down:
                endPosition += Vector2.down * size;
                break;
        }

        // 调整该Block位置
        transform.position = endPosition;

        // 连接Block
        LinkBlock(block, direction);

        // 更新轮子遮罩
        UpdateWheelCover();
        block.UpdateWheelCover();

        // 返回位移
        return endPosition - startPosition;
    }

    // 更新轮子的遮罩
    private void UpdateWheelCover()
    {
        if (isWheel)
        {
            // 删除当前遮罩
            if (transform.childCount > 0)
            {
                GameObject cover = transform.GetChild(0).gameObject;
                Destroy(cover);
            }

            // 遍历四个方向
            for (int i = 0; i < 4; i++)
            {
                // 如果有连接的Block，添加遮罩
                if (blocksLinked[i] != null)
                {
                    // 创建，旋转遮罩
                    GameObject cover = Instantiate(coverPrefab);
                    cover.name = coverPrefab.name;
                    cover.transform.position = transform.position;
                    cover.transform.Rotate(0, 0, i * 90);
                    cover.transform.parent = transform;
                    break;
                }
            }
        }
    }

    // 购买后锁定旋转
    protected override void FreezeRotation()
    {
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    protected override void OnMouseDown()
    {
        base.OnMouseDown();

        if (clickable && gameController.gamePhase == GameController.GamePhase.Preparation && Input.GetMouseButton(0))
        {
            Unlink();
        }
    }

    protected override void OnMouseUp()
    {
        base.OnMouseUp();

        if (clickable && gameController.gamePhase == GameController.GamePhase.Preparation)
        {
            AdsorptionCheck();
        }
    }
}
