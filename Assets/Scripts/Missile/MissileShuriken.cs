using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileShuriken : Missile
{
	public float shurikenSpeed = 8.0f;
	public float shurikenRotateSpeed = 300.0f;

	// 手里剑切向速度
	private float tangentSpeed = 2f;
	
	private Unit target;

	// 进攻还是收回
	private bool IsAttacking = true;

	// 一次攻击的目标位置 = 当前位置 + findEnemyRange*敌人方向向量
	private Vector2 attackPos;

	protected override void Start()
	{
		Vector2 currPos = unit.transform.position;
		float attackRange = ((BallShuriken)unit).findEnemyRange;
		attackPos = currPos + attackRange * NormalDir(transform, target.transform);
	}

	// Update is called once per frame
	protected override void FixedUpdate()
	{
		// 永远自转
		transform.Rotate(Vector3.forward, shurikenRotateSpeed * Time.deltaTime);

		if (IsAttacking)
		{
			// 一次攻击的目标位置 = 敌人的位置，可以锁定敌人
			// attackPos = target.transform.position;
			MoveToward(attackPos);
			// 距离targetPos小于一定值
			if (Vector2.Distance(transform.position, attackPos) <= 0.10)
			{
				IsAttacking = false;
			}
		}
		else
		{
			// 不存在敌人且发射球没死, 回到初始位置
			BallShuriken originBall = (BallShuriken)unit;
			if (originBall != null)
			{
				MoveToward(originBall.transform.position);
				// 回到起始位置, local相对坐标系比绝对坐标系的值要小一点
				if (Vector2.Distance(transform.position, originBall.transform.position) <= 0.10)
				{
					// 重新显示手里剑
					originBall.ReholdShuriken();
					Die();
				}
			}
			else
			{
				Destroy(gameObject, 3.0f);
			}
		}
	}

	// 设置目标为某个敌人,朝向敌人发射
	public void Launch(Unit target)
	{
		Debug.Assert(target != null, "不合法目标");

		this.target = target;

		Launch();
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		Unit target = other.gameObject.GetComponent<Unit>();
		if (target != null && this.player != target.player)
		{
			target.TakeDamage(this.damage);
			if (target.body != null)
			{
				target.body.AddForce(transform.right * forceHit);
			}
		}
	}

	/// <summary>
	/// 设置速度偏向目标
	/// </summary>
	private void MoveToward(Vector2 position)
	{
		// 计算速度
		Vector2 dir2Tar = NormalDir(transform.position, position);
		// 加切向速度形成曲线效果
		Vector2 tangentDir = Tangent(dir2Tar);
		Vector2 velocity = shurikenSpeed * dir2Tar + Random.Range(0, tangentSpeed)  * tangentDir;
		transform.GetComponent<Rigidbody2D>().velocity = velocity;
	}

	/// <summary> from到to的标准方向向量</summary>
	private Vector2 NormalDir(Transform from, Transform to)
	{
		return (to.position - from.position).normalized;
	}

	/// <summary> from到to的标准方向向量</summary>
	private Vector2 NormalDir(Vector2 from, Vector2 to)
	{
		return (to - from).normalized;
	}

	/// <summary>返回逆时针标准切向量 </summary>
	private Vector2 Tangent(Vector2 vector)
	{
		return Vector3.Cross(Vector3.forward, vector).normalized;
	}

}
