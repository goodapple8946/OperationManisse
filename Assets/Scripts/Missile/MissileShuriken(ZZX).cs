using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileShuriken : Missile
{
	public float shurikenSpeed = 8.0f;
	public float shurikenRotateSpeed = 300.0f;

	public BallShuriken originBall;
	private Unit target;
	// 当前位置+findEnemyRange*敌人方向向量
	private Vector2 attackTarPos;

	// Update is called once per frame
	protected override void Update()
	{
		Vector2 currPos = transform.position;
		// 存在敌人，速度朝向敌人方向
		if (target != null)
		{
			Vector2 dir2Tar = attackTarPos - currPos;
			Vector2 dir2TarNormal = dir2Tar.normalized;
			transform.GetComponent<Rigidbody2D>().velocity = shurikenSpeed * dir2TarNormal;
			// 自转
			transform.Rotate(Vector3.forward, shurikenRotateSpeed * Time.deltaTime);
			// 距离目标距离小于一定值
			if (dir2Tar.magnitude <= 0.10)
			{
				target = null;
			}
		}
		else
		{
			// 不存在敌人，发射球没死
			if (originBall.IsAlive())
			{
				Vector2 originPos = originBall.transform.position;
				Vector2 dir2Origin = originPos - currPos;
				Vector2 dir2OriginNormal = dir2Origin.normalized;
				transform.GetComponent<Rigidbody2D>().velocity = shurikenSpeed * dir2OriginNormal;
				// 自转
				transform.Rotate(Vector3.forward, shurikenRotateSpeed * Time.deltaTime);
				// 回到起始位置, local相对坐标系比绝对坐标系的值要小一点
				if (dir2Origin.magnitude <= 0.10)
				{
					// 重置球
					originBall.ReholdShuriken();
					Die();
				}
			}
		}
	}

	public void Attack(Unit target)
	{
		this.target = target;
		if(target != null)
		{
			Vector2 dir2Tar = target.transform.position - transform.position;
			float ratio = originBall.findEnemyRange / dir2Tar.magnitude;
			Vector2 currPos = transform.position;
			attackTarPos = currPos + ratio * dir2Tar;
		}
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

		// 撞击粒子
		if (particleHitPrefab != null)
		{
			GameObject particle = Instantiate(particleHitPrefab);
			particle.transform.position = transform.position;
			particle.transform.rotation = transform.rotation;
		}
	}
}
