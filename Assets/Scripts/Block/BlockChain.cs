using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Controller;

public class BlockChain : Block
{
	[HideInInspector] public Transform Head;
	[HideInInspector] public Transform Tail;
	public static float hookSize = 0.04f;
	public float hookMass = 0.05f;
	public float hookDrag = 0.25f;
	public float hookAngularDrag = 0.25f;

	protected override void Start()
	{
		Head = transform.Find("Head");
		Tail = transform.Find("Tail");

		// 设置子物体的刚体属性
		Rigidbody2D[] hookBodies = transform.GetComponentsInChildren<Rigidbody2D>();
		foreach(Rigidbody2D body in hookBodies)
		{
			// mass的大小决定铁链受力伸长的长度
			body.mass = hookMass;
			// linearDrag和angularDrag使铁链不会无限spinning
			body.drag  = hookDrag;
			body.angularDrag = hookAngularDrag;
		}
	}

	// 与另一个Block连接
	public override void LinkTo(Block another, int direction)
	{
		HingeJoint2D joint;
		// 根据自己的连接方向在头或尾创建铰链
		if (direction == HeadDir())
		{
			joint = Head.gameObject.AddComponent<HingeJoint2D>();
		}
		else
		{
			joint = Tail.gameObject.AddComponent<HingeJoint2D>();
		}

		// 如果另一个也是铰链
		if (another is BlockChain)
		{
			// 获取对方的头或尾部刚体
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
		blocksLinked[direction] = another;
	}

	// 两向可以链接
	public override bool IsLinkAvailable(int direction)
	{
		int negDir = GetDirectionNegative(this.direction);
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

