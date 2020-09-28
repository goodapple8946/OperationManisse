using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Controller;

public class Block : Unit
{
    public Block[] blocksLinked = new Block[4];

    public Joint2D[] joints = new Joint2D[4];

    // 断开扭矩
    private float breakTorquePlayer = 75f;
    private float breakTorque = 10f;

    // 与另一个Block连接
    public void LinkTo(Block another, int direction)
    {
        int directionNeg = GetDirectionNegative(direction);

        FixedJoint2D  joint = gameObject.AddComponent<FixedJoint2D>();
        if (player == Player.Player)
        {
            joint.breakTorque = breakTorquePlayer;
        }
        else
        {
            joint.breakTorque = breakTorque;
        }
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

    public override void Rotate()
    {
        Direction = (Direction + 1) % 4;
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
        for (int direction = 0; direction < 4; direction++)
        {
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