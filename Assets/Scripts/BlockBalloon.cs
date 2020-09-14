using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBalloon : Block
{
    public LineRenderer line;

    protected override void Start()
    {
        base.Start();

        // 避免右、上、左连接Block
        for(int directionIndex = 0; directionIndex<3; directionIndex++)
        {
            blocksLinked[directionIndex] = gameObject.GetComponent<Block>();
        }
    }

    protected override void Update()
    {
        base.Update();

        // 画线
        Block block = blocksLinked[(int)LinkDirection.Down];
        if (line != null && block != null)
        {
            line.SetPosition(0, (Vector2)transform.position + new Vector2(0, -radius));
            line.SetPosition(1, (Vector2)block.transform.position + new Vector2(0, block.radius));
        }
    }

    // 将该Block与下面连接的Block断开连接
    protected override void Unlink()
    {
        // 已经购买的Block
        if (!isSelling)
        {
            // 仅断开下面的Block
            int directionIndex = 3;

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
            // 更新遮罩
            UpdateCover();

            // 重新受到重力
            body.gravityScale = 1;

            // 删除线
            if (line != null)
            {
                Destroy(line);
            }
        }
    }

    // 吸附检测，仅检测下方
    public override void AdsorptionCheck()
    {
        // 检测线起始点
        Vector2 origin = (Vector2)transform.position + new Vector2(0, -(radius + groundCheckDistance));
        
        // 向下方发射检测射线
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down);

        // 检测到物体
        if (hit.transform != null)
        {
            Block block = hit.transform.GetComponent<Block>();

            // 物体是Block，Block不是商品、不是单向连接的、上方没有连接
            if (block != null && block.isAlive && !block.isSelling && !block.isOneLink && block.blocksLinked[(int)LinkDirection.Up] == null)
            {
                // 该Block连接目标Block
                blocksLinked[(int)LinkDirection.Down] = block;
                joints[(int)LinkDirection.Down] = gameObject.AddComponent<DistanceJoint2D>();
                joints[(int)LinkDirection.Down].connectedBody = block.body;

                // 目标Block连接该Block
                block.blocksLinked[(int)LinkDirection.Up] = gameObject.GetComponent<Block>();
                block.joints[(int)LinkDirection.Up] = block.gameObject.AddComponent<DistanceJoint2D>();
                block.joints[(int)LinkDirection.Up].connectedBody = body;

                // 不再受到重力
                body.gravityScale = 0;

                // 添加线
                line = gameObject.AddComponent<LineRenderer>();
                line.positionCount = 2;
            }
        }
    }

    // 购买后锁定旋转
    protected override void FreezeRotation()
    {
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
