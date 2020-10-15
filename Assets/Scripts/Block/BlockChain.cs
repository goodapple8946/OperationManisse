using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Controller;

public class BlockChain : Block
{
	//private Transform Head;
	//private Transform Tail;
	public GameObject[] Heads = new GameObject[4];

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

		for(int i = 0; i < Heads.Length - 1; i++)
		{
			for (int j = i+1; j < Heads.Length; j++)
			{
				if(Heads[i] != null && Heads[j] != null)
				{
					// 任意两头超过距离断开连接
					if (Vector2.Distance(Heads[i].transform.position,
						Heads[j].transform.position) >= MaxDistance)
					{
						BreakLinks();
					}
				}
			}
		}
	}

	// 与另一个Block连接
	public override void LinkTo(Block another, int direction)
	{
		HingeJoint2D joint = Heads[direction].gameObject.AddComponent<HingeJoint2D>();

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
		
		return Heads[direction] != null && joints[direction] == null;
	}

	public override void Rotate()
	{
		base.Rotate();
		Debug.Log("asdd");
		// 偏移Heads
		GameObject temp = Heads[3];
		Heads[3] = Heads[2];
		Heads[2] = Heads[1];
		Heads[1] = Heads[0];
		Heads[0] = temp;
	}

	public Rigidbody2D GetBody(int dir)
	{
		return Heads[dir].GetComponent<Rigidbody2D>();
	}
}
