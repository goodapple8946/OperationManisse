using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : Unit
{
    // 吸附半径与严格吸附半径
    // 拖动Block时，如果与其他Block的吸附点距离较近时，则连接两者，并且调整拖动Block的位置
    public static float adsorptionDistance = 0.3f;
    public static float adsorptionDistanceStrict = 0.01f;

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

    // 是单向连接的（四周只能有一个方向连接Block）
    public bool isOneLink;

    protected override void OnMouseOver()
    {
        // 准备阶段
        if (clickable && gameController.gamePhase == GameController.GamePhase.Preparation)
        {
            // 鼠标左键按下
            if (Input.GetMouseButtonDown(0))
            {
                if (isSelling)
                {
                    Buy();
                }

                // 未按下Ctrl，断开连接
                if (!gameController.keyCtrl)
                {
                    Unlink();
                }

                int rand = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
                SetBlockLinkedSelected(true, rand);
            }

            // 鼠标左键抬起
            if (Input.GetMouseButtonUp(0))
            {
                int rand;

                rand = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
                SetBlockLinkedSelected(false, rand);

                AdsorptionCheck(adsorptionDistance);

                // 完成后，递归地对所有连接的Block进行吸附（位置调整优化）
                rand = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
                AbsorbPositionRecursively(rand);
            }
        }

        // 鼠标右键按下
        if (Input.GetMouseButtonDown(1))
        {
            // 准备阶段，出售
            if (gameController.gamePhase == GameController.GamePhase.Preparation)
            {
                Sell();
            }
            // 游戏阶段，删除
            else if (gameController.gamePhase == GameController.GamePhase.Playing)
            {
                Delete();
            }
        }
    }

    protected override void OnMouseDrag()
    {
        if (clickable && isAlive && !isSelling && gameController.gamePhase == GameController.GamePhase.Preparation && Input.GetMouseButton(0))
        {
            int rand = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            TranslateBlockLinked(MouseController.offset, rand);
        }
    }

    // 所有连接的Block同时移动
    protected virtual void TranslateBlockLinked(Vector3 translation, int checkTag)
    {
        this.checkTag = checkTag;
        transform.Translate(translation);

        foreach (Block block in blocksLinked)
        {
            if (block != null && block.checkTag != checkTag)
            {
                block.TranslateBlockLinked(translation, checkTag);
            }
        }
    }

    // 设置所有连接的Block为选中状态（设置BodyType与Layer），CheckTag为随机值
    protected virtual void SetBlockLinkedSelected(bool selected, int checkTag)
    {
        this.checkTag = checkTag;

        if (selected)
        {
            body.bodyType = RigidbodyType2D.Static;
            GetComponent<SpriteRenderer>().sortingLayerName = "Pick";
        }
        else
        {
            body.bodyType = RigidbodyType2D.Dynamic;
            GetComponent<SpriteRenderer>().sortingLayerName = "Unit";
        }
        foreach (Block block in blocksLinked)
        {
            if (block != null && block.checkTag != checkTag)
            {
                block.SetBlockLinkedSelected(selected, checkTag);
            }
        }
    }

    // 将目标Block按照方向连接到该Block
    protected void LinkBlock(Block block, LinkDirection direction)
    {
        // 目标Block所处方向的反方向
        LinkDirection directionNegative = (LinkDirection)(((int)direction + 2) % 4);

        // 该Block连接目标Block
        blocksLinked[(int)direction] = block;
        joints[(int)direction] = gameObject.AddComponent<FixedJoint2D>();
        joints[(int)direction].connectedBody = block.body;

        // 目标Block连接该Block
         block.blocksLinked[(int)directionNegative] = gameObject.GetComponent<Block>();
        // block.joints[(int)directionNegative] = block.gameObject.AddComponent<FixedJoint2D>();
        // block.joints[(int)directionNegative].connectedBody = body;
    }

    // 将该Block与所有连接的Block断开连接
    protected virtual void Unlink()
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
                block.UpdateCover();
            }
        }
        // 更新遮罩
        UpdateCover();
    }

    // 死亡
    protected override IEnumerator Die()
    {
        if (!isSelling)
        {
            // 断开所有连接
            Unlink();
        }

        return base.Die();
    }

    /*
     * Block吸附的过程为：
     * 1. 进行AdsorptionCheck
     * 2. AdsorptionCheck中，根据四个方向进行AdsorptionCheckDirection
     * 3. 如果AdsorptionCheckDirection返回值不为null，进行Absorb
     */

    // 根据吸附方向返回Block的吸附点
    public Vector2 AdsorptionPoint(LinkDirection direction)
    {
        Vector2 point = transform.position;
        switch (direction)
        {
            case LinkDirection.Right:
                point += Vector2.right * 2 * radius;
                break;
            case LinkDirection.Up:
                point += Vector2.up * 2 * radius;
                break;
            case LinkDirection.Left:
                point += Vector2.left * 2 * radius;
                break;
            case LinkDirection.Down:
                point += Vector2.down * 2 * radius;
                break;
        }
        return point;
    }

    // 吸附检测
    public virtual void AdsorptionCheck(float adsorptionDistance)
    {
        // 遍历四个检测方向
        for (int i = 0; i < 4; i++)
        {
            // 检测方向
            LinkDirection direction = (LinkDirection)i;

            // 吸附检测
            Block block = AdsorptionCheckDirection(direction, adsorptionDistance);

            // 吸附并连接
            if (block != null)
            {
                Absorb(block, direction);

                // 单向连接的Block只能吸附一个方向
                if (isOneLink)
                {
                    break;
                }
            }
        }
    }

    // 根据方向检测该Block是否处于其他Block的吸附范围内
    protected virtual Block AdsorptionCheckDirection(LinkDirection direction, float checkDistance)
    {
        // 吸附方向，即检测方向的反方向
        LinkDirection directionNegative = (LinkDirection)(((int)direction + 2) % 4);

        // 找到所有Block
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Block");

        // 遍历所有Block
        foreach (GameObject gameObject in gameObjects)
        {
            Block block = gameObject.GetComponent<Block>();

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
                // 如果遍历到的Block是单向连接的
                if (block.isOneLink)
                {
                    // 要保证遍历到的Block没有连接任何其他Block
                    bool cleanWheel = true;
                    foreach (Block checkBlock in block.blocksLinked)
                    {
                        if (checkBlock != null)
                        {
                            cleanWheel = false;
                            break;
                        }
                    }
                    // 遍历到的Block已经有连接的Block了，跳过
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

    // 将该Block与目标Block根据方向吸附并连接
    public void Absorb(Block block, LinkDirection direction)
    {
        // 吸附方向，即检测方向的反方向
        LinkDirection directionNegative = (LinkDirection)(((int)direction + 2) % 4);

        // 连接Block
        LinkBlock(block, direction);

        // 更新遮罩
        UpdateCover();
        block.UpdateCover();
    }
    
    // 对所有连接的Block，递归地根据方向吸附并连接
    public void AbsorbPositionRecursively(int checkTag)
    {
        this.checkTag = checkTag;

        // 该Block的起始位置
        Vector2 startPosition = transform.position;

        for (int i = 0; i < 4; i++)
        {
            // 检测方向
            LinkDirection direction = (LinkDirection)i;

            // 吸附方向，即检测方向的反方向
            LinkDirection directionNegative = (LinkDirection)(((int)direction + 2) % 4);

            // 目标Block
            Block block = blocksLinked[i];

            if (block != null && block.checkTag != checkTag)
            {
                // 目标Block将要移动到的位置
                Vector2 endPosition = transform.position;

                switch (direction)
                {
                    case LinkDirection.Right:
                        endPosition += Vector2.right * 2 * radius;
                        break;
                    case LinkDirection.Up:
                        endPosition += Vector2.up * 2 * radius;
                        break;
                    case LinkDirection.Left:
                        endPosition += Vector2.left * 2 * radius;
                        break;
                    case LinkDirection.Down:
                        endPosition += Vector2.down * 2 * radius;
                        break;
                }

                // 调整目标Block的位置
                block.transform.position = endPosition;

                // 递归
                block.AbsorbPositionRecursively(checkTag);
            }
        }
        AdsorptionCheck(adsorptionDistanceStrict);
    }

    // 更新遮罩
    public virtual void UpdateCover()
    {
        // 如果有遮罩预设
        if (coverPrefab != null)
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
}
