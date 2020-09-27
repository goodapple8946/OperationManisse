using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileShuriken : Missile
{
	public enum Status { attack, recall, hold }

	public float shurikenSpeed = 8.0f;
	public float shurikenRotateSpeed = 300.0f;

	private Status status;
	private Vector2 targetPos;
	private Vector2 originLocalPos;
	
	// Start is called before the first frame update
	protected override void Start()
	{
		base.Start();
		status = Status.hold;
		originLocalPos = transform.localPosition;
	}

	// Update is called once per frame
	protected override void Update()
	{
		Vector2 currPos = transform.position;
		Vector2 dir2Tar = targetPos - currPos;
		Vector2 currLocalPos = transform.localPosition;
		Vector2 dir2Origin = originLocalPos - currLocalPos;
		
		if (status == Status.attack)
		{
			Vector2 dir2TarNormal = dir2Tar.normalized;
			transform.GetComponent<Rigidbody2D>().velocity = shurikenSpeed * dir2TarNormal;
			// 自转
			transform.Rotate(Vector3.forward, shurikenRotateSpeed * Time.deltaTime);
			// 距离目标距离小于一定值
			if (dir2Tar.magnitude <= 0.10)
			{
				status = Status.recall;
			}
		}
		else if(status == Status.recall)
		{
			Vector2 dir2OriginNormal = dir2Origin.normalized;
			transform.GetComponent<Rigidbody2D>().velocity = shurikenSpeed * dir2OriginNormal;
			// 自转
			transform.Rotate(Vector3.forward, shurikenRotateSpeed * Time.deltaTime);
			// 回到起始位置, local相对坐标系比绝对坐标系的值要小一点
			if (dir2Origin.magnitude <= 0.03)
			{
				status = Status.hold;
				// 重置球的CD
				transform.parent.GetComponent<BallShuriken>().ResetCD();
			}
		}
		
	}

	public void Attack(Unit target)
	{
		this.targetPos = target.transform.position;
		status = Status.attack;
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		Unit unit = other.gameObject.GetComponent<Unit>();
		if (unit != null && unit.player == Unit.Player.Enemy)
		{
			unit.TakeDamage(this.damage);
			if (unit.body != null)
			{
				unit.body.AddForce(transform.right * forceHit);
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
