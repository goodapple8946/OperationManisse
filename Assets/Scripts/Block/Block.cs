using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : Unit
{
    /* Link Direction:
     * 0: Right, 1: Top, 2: Left, 3: Bottom
     */

    public int direction = 0;

    public Block[] blocksLinked = new Block[4];

    public Joint2D[] joints = new Joint2D[4];

    // 与另一个Block连接
    public void LinkTo(Block another, int direction)
    {
        int directionNeg = GetDirectionNegative(direction);

        FixedJoint2D  joint = gameObject.AddComponent<FixedJoint2D>();
        joint.connectedBody = another.body;
        joints[direction] = joint;
        blocksLinked[direction] = another;
    }

    // 与另一个Block解除连接
    public void UnlinkTo(Block another, int direction)
    {
        int directionNeg = GetDirectionNegative(direction);

        if (joints[direction] != null)
        {
            Destroy(joints[direction]);
        }
        blocksLinked[direction] = null;
    }

    public void Rotate()
    {
        direction = (direction + 1) % 4;
        transform.Rotate(0, 0, 90f);
    }

    public virtual bool IsLinkAvailable(int direction)
    {
        return true;
    }

    // 根据方向整数获取向量
    protected Vector2 GetVectorByDirection(int direction)
    {
        Vector2 vector = Vector2.zero;
        switch (direction)
        {
            case 0:
                vector = new Vector2(1, 0);
                break;
            case 1:
                vector = new Vector2(0, 1);
                break;
            case 2:
                vector = new Vector2(-1, 0);
                break;
            case 3:
                vector = new Vector2(0, -1);
                break;
        }
        return vector;
    }

    // 根据方向整数获取角度
    protected float GetAngleByDirection(int direction)
    {
        return direction * 90f;
    }

    // 死亡
    protected override void Die()
    {
        // 解除连接
        for (int i = 0; i < 4; i++)
        {
            int direction = i;
            int directionNeg = GetDirectionNegative(direction);

            Block another = blocksLinked[direction];
            if (another != null)
            {
                UnlinkTo(another, direction);
                another.UnlinkTo(this, directionNeg);
            }
        }
        base.Die();
    }

    protected int GetDirectionNegative(int direction)
    {
        return (direction + 2) % 4;
    }
}