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

		HingeJoint2D joint;
		if (direction == HeadDir())
		{
			joint = Head.gameObject.AddComponent<HingeJoint2D>();
			
		}
		else
		{
			joint = Tail.gameObject.AddComponent<HingeJoint2D>();
		}

		if(another is BlockChain)
		{
			// 如果另一个也是铰链
			BlockChain anotherChain = (BlockChain)another;
			int anotherLinkDir = GetDirectionNegative(direction);
			joint.connectedBody = anotherChain.GetBody(anotherLinkDir);
		}
		else
		{
			joint.connectedBody = another.body;
		}

		joint.anchor = hookSize * dirVector[direction];
		joints[direction] = joint;


			//// 如果是自己的头部才链接
			//if (direction == this.direction)
			//{
			//	HingeJoint2D joint = Head.gameObject.AddComponent<HingeJoint2D>();
			//	if(another is BlockChain)
			//	{
			//		// 如果另一个也是铰链
			//		BlockChain anotherChain = (BlockChain)another;
			//		joint.connectedBody = anotherChain.Tail.GetComponent<Rigidbody2D>();
			//	}
			//	else
			//	{
			//		joint.connectedBody = another.body;
			//	}
		}

	public override bool IsLinkAvailable(int direction)
	{
		int negDir = GetDirectionNegative(this.direction);
		// 两向都可以链接
		bool canLink = (direction == this.direction || direction == negDir);
		return canLink && joints[direction] == null;
	}

	public int HeadDir()
	{
		return direction;
	}

	public int TailDir()
	{
		return GetDirectionNegative(direction);
	}

	public Rigidbody2D GetBody(int dir)
	{
		if(dir == HeadDir())
		{

			return Head.GetComponent<Rigidbody2D>();
		}
		else if(dir == TailDir())
		{
			return Tail.GetComponent<Rigidbody2D>();
		}
		else
		{
			return null;
		}
	}
}

