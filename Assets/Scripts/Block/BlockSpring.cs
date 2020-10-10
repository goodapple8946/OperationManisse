using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 与两个物体建立两个弹簧joint2D
public class BlockSpring : Block
{
	public float breakForce = 10.0f;
	public float maxDistance = 1.0f;

	// 弹簧相连的两个点，确定弹簧的绘画方向
	private float originDistance;
	private GameObject drawStartObj;
	private GameObject drawEndObj;

	protected override void Start()
	{
		base.Start();
		originDistance = 4 * radius;
	}

	protected override void Update()
	{
		base.Update();
		if(drawStartObj != null && drawEndObj != null)
		{
			Redraw(drawStartObj, drawEndObj);
			CheckMaxDistance();
		}
	}

	public override void LinkTo(Block another, int direction)
	{
		Debug.Assert(drawStartObj == null || drawEndObj == null, "弹簧与超过两个物体连接");

		blocksLinked[direction] = another;
		// 弹簧的正向为绘画的终点
		if (direction == this.direction)
		{
			drawEndObj = another.gameObject;
		}
		// 弹簧的反向为绘画的终点
		else
		{
			drawStartObj = another.gameObject;
		}
		
		SpringJoint2D joint = gameObject.AddComponent<SpringJoint2D>();
		joint.connectedBody = another.body;
		// 自己的链接点在自己的中心
		joint.anchor = Vector2.zero;
		// 相连物体的链接点在对方的中心
		joint.connectedAnchor = Vector2.zero;
		// 手动设置距离
		joint.autoConfigureDistance = false;
		joint.distance = 2 * radius;
		// 断开所需力量
		joint.breakForce = breakForce;

		joints[direction] = joint;
	}

	// 两向可以链接
	public override bool IsLinkAvailable(int direction)
	{
		int negDir = GetDirectionNegative(this.direction);
		bool canLink = (direction == this.direction || direction == negDir);
		return canLink && joints[direction] == null;
	}

	// 从from向to按比例伸长弹簧图像，使弹簧两端在from,to的中心点
	private void Redraw(GameObject from, GameObject to)
	{
		Vector2 pos1 = from.transform.position;
		Vector2 pos2 = to.transform.position;
		// 与原始大小相比的缩放比
		float scaleRatio = Vector2.Distance(pos1, pos2) / (originDistance);
		// 右向量与向量差的夹角，即为物体需要旋转的角度
		float angle = Vector2.SignedAngle(Vector2.right, pos2 - pos1);

		// 旋转
		transform.localEulerAngles = new Vector3(0, 0, angle);
		// 平移
		transform.position = (pos1 + pos2) / 2;
		// 缩放
		transform.localScale = new Vector3(scaleRatio, 1, 1);
	}
	
	/// <summary>
	/// 超过最大距离就自杀 
	/// </summary>
	private void CheckMaxDistance()
	{
		float distance = Vector2.Distance(
			drawStartObj.transform.position, drawEndObj.transform.position);

		if(distance >= maxDistance)
		{
			TakeDamage(new Damage(this.healthMax, this.GetType()));
		}
	}
}
