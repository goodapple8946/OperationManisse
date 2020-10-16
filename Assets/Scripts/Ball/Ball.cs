using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Controller;

public class Ball: Unit
{
	// 武器发射相对中心点的偏移
	public float weaponOffset;

	// 武器冷却最大值
	public float weaponCDMax;

	// 武器发射夹角的一半, 以及子弹发射的随机偏移角度
	public float weaponAngle;

	// 索敌范围
	public float findEnemyRange;

	// 弹药预设
	public GameObject missilePrefab;

	// 小球旋转速率
	protected float rotationSpeed = 3f;

	// 当前武器冷却
	protected float weaponCD = 0;

	// 目标优先级容差
	private float priorityTolerant = 3f;

	private Unit currTarget = null;

	protected override void Update()
	{
		base.Update();
		// 更新朝向，保证小球图像顶部朝上
		UpdateFlip();
	}

	// 索敌。返回最佳目标，如果没有则返回null
	protected Unit FindEnemyOptimize()
	{
		if (currTarget != null && !InRange(currTarget))
		{
			currTarget = null;
		}

		if(currTarget != null)
		{
			return currTarget;
		}
		else
		{
			return FindEnemy();
		}
	}

	// 索敌。返回最近目标，如果没有则返回null
	protected Unit FindEnemy()
	{
		string maskName = null;
        if (player == Player.Neutral)
		{
			return null;
		}
		else if (player == Player.Player)
		{
			maskName = "Enemy";
		}
		else if (player == Player.Enemy)
		{
			maskName = "Player";
		}

		// 先找Ball
		Collider2D collider = Physics2D.OverlapCircle(
			transform.position,
			findEnemyRange,
			LayerMask.GetMask(maskName + "Ball"));
		
		if (collider != null)
		{
			return collider.GetComponent<Unit>();
		}
		else
		{
			// 如果没找到Ball，再找Block
			collider = Physics2D.OverlapCircle(
				transform.position,
				findEnemyRange,
				LayerMask.GetMask(maskName + "Block"));
			// 没找到再返回null
			return collider != null ? collider.GetComponent<Unit>() : null;
		}
	}

	// 索敌。返回最佳目标，如果没有则返回null
	//protected Unit FindEnemy()
	//{
	//	List<Unit> units = gameController.GetUnitsList();
	//	Unit target = null;
	//	float maxPriority = float.MinValue;
	//	foreach (Unit unit in units)
	//	{
	//		// 目标合法
	//		if (IsLegalTarget(unit))
	//		{
	//			float priority = CalculatePriority(unit);
	//			// 优先级更高的目标
	//			if (priority > maxPriority + priorityTolerant)
	//			{
	//				target = unit;
	//				maxPriority = priority;
	//			}
	//		}
	//	}
	//	return target;
	//}

	protected bool InRange(Unit unit)
	{
		return (unit.transform.position - transform.position).magnitude 
			<= findEnemyRange;
	}

	/// <summary>
	/// true: 目标存活在范围内且是敌对的
	/// </summary>
	protected bool IsLegalTarget(Unit unit)
	{
		// 敌对正营
		bool unitIsOpponent =
			player == Player.Player && unit.player == Player.Enemy ||
			player == Player.Enemy && unit.player == Player.Player;
		return InRange(unit) && unitIsOpponent;
	}

	// 索敌优先级
	protected int CalculatePriority(Unit unit)
	{
		float distance = (unit.transform.position - transform.position).magnitude;
		int priority = 0;

		// 目标在射程内
		if (distance <= findEnemyRange)
		{
			priority += (int)((findEnemyRange - distance) * 10f);
			priority += unit.priority;
		}
		return priority;
	}

	/// <summary>
	/// 以rotationSpeed转向目标,如果夹角很小就锁定目标
	/// </summary>
	protected void RotateToward(Unit target)
	{
		// 需要旋转的角度
		float angle = CalculateAngle(target);

		// 朝向敌人
		if (angle > rotationSpeed)
		{
			transform.Rotate(0, 0, rotationSpeed);
		}
		else if (angle < -rotationSpeed)
		{
			transform.Rotate(0, 0, -rotationSpeed);
		}
		else
		{
			transform.Rotate(0, 0, angle);
		}
	}

	/// <summary>
	/// 计算与unit之间的夹角,返回SignedAngle
	/// </summary>
	protected float CalculateAngle(Unit unit)
	{
		Debug.Assert(unit != null);
		// 自己的朝向
		Vector2 vector = transform.right;
		// 两者间的向量
		Vector2 vectorTarget = unit.transform.position - transform.position;
		// 需要旋转的角度
		float angle = Vector2.SignedAngle(vector, vectorTarget);
		return angle;
	}

	// 武器冷却
	protected virtual void WeaponCoolDown()
	{
		weaponCD -= Time.deltaTime;
	}

	/// <summary>
	/// 更正小球图片的显示方向, 判断小球是否应该翻转
	/// </summary>
	protected virtual void UpdateFlip()
	{
		// 根据欧拉角设置图片翻转
		if (transform.rotation.eulerAngles.z < 90f || transform.rotation.eulerAngles.z > 270f)
		{
			SetFlipY(false);
		}
		else
		{
			SetFlipY(true);
		}
	}

	// 远程攻击
	protected virtual void RangedAttack()
	{
		// 创建弹药
		Missile missile = CreateMissile(missilePrefab);

		// 发射弹药
		missile.Launch();
	}

	// 创建弹药
	protected virtual Missile CreateMissile(GameObject missilePrefab)
	{
		// 创建弹药
		Missile missile = Instantiate(missilePrefab).GetComponent<Missile>();
		// 弹药玩家
		missile.player = player;
		// 弹药发射者
		missile.unit = this;
		// 弹药发射点
		missile.transform.position = transform.position + transform.right * weaponOffset;
		// 弹药朝向
		missile.transform.rotation = transform.rotation;
		// 弹药随机角度
		missile.transform.Rotate(0, 0, UnityEngine.Random.Range(-weaponAngle, weaponAngle));
		// 弹药父物体
		missile.transform.parent = gameController.missileObjects.transform;

		// Layer
		if (gameObject.layer == (int)Layer.PlayerBall)
		{
			missile.gameObject.layer = (int)Layer.PlayerMissile;
		}
		else if (gameObject.layer == (int)Layer.EnemyBall)
		{
			missile.gameObject.layer = (int)Layer.EnemyMissile;
		}
        else
        {
			throw new Exception("Unknown missile launcher layer.");
        }

		return missile;
	}

	/// <summary>
	/// 转动两下
	/// </summary>
	public override void Rotate()
	{
		transform.Rotate(0, 0, 180f);
		direction = (direction + 2) % 4;
	}

	/// <summary>
	/// 设置flipY,只改变图片显示不改变欧拉角
	/// </summary>
	protected void SetFlipY(bool value)
	{
		SpriteRenderer[] renders = transform.GetComponentsInChildren<SpriteRenderer>();
		foreach (SpriteRenderer render in renders)
		{
			render.flipY = value;
		}
	}
}