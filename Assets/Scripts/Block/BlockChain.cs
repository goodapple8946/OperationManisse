using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Controller;

public class BlockChain : Block
{
	private Transform Head;
	private Transform Tail;

	// 质量越大，铁链越难被拉长
	private static float HookMass = 0.25f;
	// 沿着铁链方向的空气阻力
	private static float HookDrag = 1.0f;
	// 垂直于铁链方向的空气阻力
	private static float HookAngularDrag = 1.0f;

	// 每个环
	private static float HookSize = 0.08f;
	// 铁链最长伸展距离，超过距离会断掉与两边的连接
	private static float MaxDistance = 12 * HookSize;

	protected override void Start()
	{
		base.Start();

		Head = transform.Find("Head");
		Tail = transform.Find("Tail");

		// 设置铰链环的刚体属性
		Rigidbody2D[] hookBodies = transform.GetComponentsInChildren<Rigidbody2D>();
		foreach (Rigidbody2D body in hookBodies)
		{
			body.mass = HookMass;
			body.drag = HookDrag;
			body.angularDrag = HookAngularDrag;
		}

		// 从尾遍历到第二个,建立内部铰链
		//for (int i = 0; i < transform.childCount - 1; i++)
		//{
		//	GameObject child = transform.GetChild(i).gameObject;
		//	// 与后一个连接, 如果在此处创建instantiate时会创建多个铰链
		//	HingeJoint2D joint = child.GetComponent<HingeJoint2D>();
		//	joint.connectedBody =
		//		transform.GetChild(i + 1).GetComponent<Rigidbody2D>();
		//}
	}

	public override void GameStart()
	{
		base.GameStart();
		// 开始游戏后取消为鼠标服务的BoxCollider2D
		GetComponent<BoxCollider2D>().enabled = false;
	}

	protected override void Update()
	{
		base.Update();

		// 超过距离断开连接
		if (Vector2.Distance(Head.position, Tail.position) >= MaxDistance)
		{
			BreakLinks();
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
			int anotherLinkDir = Negative(direction);
			joint.connectedBody = anotherChain.GetBody(anotherLinkDir);
		}
		else
		{
			joint.connectedBody = another.body;
		}
		// joint.anchor = AnchorDistance * dirVector[direction];

		joints[direction] = joint;
		blocksLinked[direction] = another;
	}

	// 两向可以链接
	public override bool IsLinkAvailable(int direction)
	{
		int negDir = Negative(this.direction);
		bool canLink = (direction == this.direction || direction == negDir);
		return canLink && joints[direction] == null;
	}

	public int HeadDir()
	{
		return direction;
	}

	public int TailDir()
	{
		return Negative(direction);
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

