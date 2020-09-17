using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBalloon : Block
{
    public Material materialLine;

    public LineRenderer line;

    public float lineDistance;

    public float lineDistanceMax;

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

        Block block = blocksLinked[(int)LinkDirection.Down];
        if (line != null)
        {
            if (block != null)
            {
                // 画线
                line.SetPosition(0, (Vector2)transform.position);
                line.SetPosition(1, (Vector2)block.transform.position + new Vector2(0, block.radius));

                // 调整距离
                ((DistanceJoint2D)joints[(int)LinkDirection.Down]).distance = lineDistance;

                // 受到负重力
                body.gravityScale = -0.5f;

                // 气球底部朝向被连接Block
                transform.GetChild(0).up = transform.position - block.transform.position;
            }
            else
            {
                // 删除线
                Destroy(line);
                
                // 重新受到重力
                body.gravityScale = 1;
            }
        }
    }

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

                Unlink();

                body.bodyType = RigidbodyType2D.Static;
                SetSpriteSortingLayer("Pick");
            }

            // 鼠标左键抬起
            if (Input.GetMouseButtonUp(0))
            {
                body.bodyType = RigidbodyType2D.Dynamic;
                SetSpriteSortingLayer("Unit");

                AdsorptionCheck();
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
                // block.blocksLinked[directionNegativeIndex] = null;
                // if (block.joints[directionNegativeIndex] != null)
                // {
                //     Destroy(block.joints[directionNegativeIndex]);
                // }

                // 更新连接的遮罩
                block.UpdateCover();
            }
            // 更新遮罩
            UpdateCover();

            // 删除线
            if (line != null)
            {
                Destroy(line);
            }
        }
    }

    // 吸附检测，仅检测下方
    public void AdsorptionCheck()
    {
        // 检测线起始点
        Vector2 origin = (Vector2)transform.position + new Vector2(0, -(radius + groundCheckDistance));
        
        // 向下方发射检测射线
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, lineDistanceMax, 1 << LayerMask.NameToLayer("PlayerBlock"));

        // 检测到物体
        if (hit.transform != null)
        {
            Block block = hit.transform.GetComponent<Block>();

            // 物体是Block，Block不是商品、不是单向连接的、上方没有连接
            if (block != null && block.isAlive && !block.isSelling && !block.isOneLink && block.blocksLinked[(int)LinkDirection.Up] == null)
            {
                // 该Block连接目标Block
                blocksLinked[(int)LinkDirection.Down] = block;

                DistanceJoint2D joint = gameObject.AddComponent<DistanceJoint2D>();
                joint.connectedBody = block.body;
                joint.maxDistanceOnly = true;
                lineDistance = joint.distance;

                joints[(int)LinkDirection.Down] = joint;


                // 添加线
                line = gameObject.AddComponent<LineRenderer>();
                line.positionCount = 2;
                line.material = materialLine;
                line.startWidth = line.endWidth = 0.04f;
            }
        }
    }
}
