using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Controller;

public class BlockChain : Block
{
	public Transform Head;
	public Transform Tail;
	public static float hookSize = 0.05f;


	protected override void Start()
	{
		Head = transform.Find("Head");
		Tail = transform.Find("Tail");
	}

	// 与另一个Block连接
	public override void LinkTo(Block another, int direction)
	{
		blocksLinked[direction] = another;

		// 自己头部链接
		if (direction == this.direction)
		{
			HingeJoint2D joint = Head.gameObject.AddComponent<HingeJoint2D>();
			if(another is BlockChain)
			{
				BlockChain anotherChain = (BlockChain)another;
				joint.connectedBody = anotherChain.Tail.GetComponent<Rigidbody2D>();
			}
			else
			{
				joint.connectedBody = another.body;
			}
			joint.anchor = new Vector2(hookSize, 0);
			joints[direction] = joint;
		}
		else
		{
			//HingeJoint2D joint = Tail.gameObject.AddComponent<HingeJoint2D>();
			//joint.connectedBody = another.body;
			//joint.anchor = new Vector2(-hookSize, 0);
			//joints[direction] = joint;
		}
	}

	public override bool IsLinkAvailable(int direction)
	{
		int negDir = GetDirectionNegative(this.direction);
		// 两向都可以链接
		bool canLink = (direction == this.direction || direction == negDir);
		return canLink && joints[direction] == null;
	}
}

