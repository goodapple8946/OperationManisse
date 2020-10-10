using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 与两个物体建立两个弹簧joint2D
public class BlockSpring : Block
{
	//private float breakForce = 10.0f;
	private static float originDistance;
	private static float maxDistance;
	private static float minDistance;
	// [0, 1]之间, 阻尼比越高恢复原状越快
	private static float DampingRatio = 1f;
	// [0, 1000000]之间, 越高弹簧越硬
	private static float Frequency = 1f;

	// 弹簧相连的两个点，确定弹簧的绘画方向
	// 物体作为绘制起点
	private Block fromObj;
	// fromObj相对自己的方位
	private int fromDir;
	private Block toObj;
	private int toDir;

	protected override void Start()
	{
		base.Start();
		originDistance = 2 * radius;
		maxDistance = 4 * radius;
		minDistance = 0.5f * radius;
	}

	protected override void Update()
	{
		base.Update();

		// 双向链接
		if (fromObj != null && toObj != null)
		{
			Vector2 fromLinkedPoint = GetFromLinkedPoint();
			Vector2 toLinkedPoint = GetToLinkedPoint();
			Redraw(fromLinkedPoint, toLinkedPoint);
			CheckDistance();
		}
		// 单向链接
		else if (fromObj != null || toObj != null)
		{
			transform.localScale = Vector3.one;
		}
		else
		{
			transform.localScale = Vector3.one;
		}
	}

	public override void LinkTo(Block another, int direction)
	{
		Debug.Assert(fromObj == null || toObj == null, "弹簧与超过两个物体连接");
		blocksLinked[direction] = another;

		// 弹簧的正向为绘画的终点
		if (direction == this.direction)
		{
			toObj = another;
			toDir = direction;
		}
		// 弹簧的反向为绘画的终点
		else if (direction == Negative(this.direction))
		{
			fromObj = another;
			fromDir = direction;
		}
		SpringJoint2D joint = gameObject.AddComponent<SpringJoint2D>();
		joint.connectedBody = another.body;
		// 自己的链接点在相连边对边的中心
		joint.anchor = dirVector[Negative(direction)] * radius;
		// 相连物体的链接点在相连边的中心
		joint.connectedAnchor = dirVector[Negative(direction)] * radius;

		// 手动设置初始距离
		joint.autoConfigureDistance = false;
		joint.distance = originDistance;
		// 断开所需力量
		joint.dampingRatio = DampingRatio;
		joint.frequency = Frequency;
		joint.enableCollision = true;
		
		joints[direction] = joint;
		
	}

	// 两向可以链接
	public override bool IsLinkAvailable(int direction)
	{
		int negDir = Negative(this.direction);
		bool canLink = (direction == this.direction || direction == negDir);
		return canLink && joints[direction] == null;
	}

	// 从from向to按比例伸长弹簧图像，使弹簧两端中心点在fromPos，toPos
	private void Redraw(Vector2 fromPos, Vector2 toPos)
	{
		// 与原始大小相比的缩放比
		float scaleRatio = Vector2.Distance(fromPos, toPos) / (originDistance);
		// 右向量与向量差的夹角，即为物体需要旋转的角度
		float angle = Vector2.SignedAngle(Vector2.right, toPos - fromPos);

		// 旋转
		transform.localEulerAngles = new Vector3(0, 0, angle);
		// 平移
		transform.position = (fromPos + toPos) / 2;
		// 缩放
		transform.localScale = new Vector3(scaleRatio, 1, 1);
	}

	/// <summary>
	/// 超过最大距离就自杀 
	/// </summary>
	private void CheckDistance()
	{
		Vector2 fromLinkedPoint = GetFromLinkedPoint();
		Vector2 toLinkedPoint = GetToLinkedPoint();
		float distance = Vector2.Distance(fromLinkedPoint, toLinkedPoint);

		if (distance >= maxDistance || distance <= minDistance)
		{
			// 还原大小
			transform.localScale = Vector3.one;
			// 自杀
			TakeDamage(new Damage(this.healthMax, this.GetType()));
		}
	}

	private Vector2 GetToLinkedPoint()
	{
		return toObj.GetLinkedPoint(Negative(toDir));
	}

	private Vector2 GetFromLinkedPoint()
	{
		return fromObj.GetLinkedPoint(Negative(fromDir));
	}
}
