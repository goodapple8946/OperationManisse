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
    protected float breakTorquePlayer = 75f;
	protected float breakTorque = 10f;

    // 与另一个Block连接
    public virtual void LinkTo(Block another, int direction)
    {
		blocksLinked[direction] = another;

		// 对方是铰链
		if(another is BlockChain)
		{
			//BlockChain anotherChain = (BlockChain)another;
			//int anotherLinkDir = GetDirectionNegative(direction);
			//// 与铁链的尾部链接，创建HingeJoint2D
			//if (anotherLinkDir == anotherChain.TailDir())
			//{
			//	HingeJoint2D joint = gameObject.AddComponent<HingeJoint2D>();
			//	joint.connectedBody = anotherChain.Tail.GetComponent<Rigidbody2D>();
			//	joint.anchor = radius * dirVector[direction];
			//	joints[direction] = joint;
			//}
		}
		else // 普通链接，创建FixedJoint2D
		{
			FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
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
		}
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

	public virtual bool IsLinkAvailable(int direction)
    {
        return joints[direction] == null;
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

	protected override void OnDestroy()
	{
		base.OnDestroy();
		BreakLinks();
	}

	// 解除所有自己的关节,和相连物体朝向自己方向的关节
	protected void BreakLinks()
    {
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
    }

    protected int GetDirectionNegative(int direction)
    {
        return (direction + 2) % 4;
    }
}