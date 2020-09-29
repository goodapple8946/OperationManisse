using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileShuriken : Missile
{
    // 旋转速率
    private float speedRotation = -1080f;
    // 当前飞行时间
    private float timeFly = 0;
    // 至少飞行时间
    private float timeFlyReturn = 0.1f;
    // 距离足够小时，认为已经返回发射者
    private float distanceReturn = 0.3f;
    // 回旋力系数
    float forceScale = 0.2f;

    private void OnTriggerEnter2D(Collider2D other)
	{
		Unit unit = other.gameObject.GetComponent<Unit>();
		if (unit != null && unit.player != this.player)
		{
			unit.TakeDamage(this.damage);
		}
	}

    // 发射
    public override void Launch()
    {
        if (body == null)
        {
            body = GetComponent<Rigidbody2D>();
        }
        body.AddForce(transform.right * forceLaunch);
    }

    protected override void Update()
    {
        timeFly += Time.deltaTime;
        transform.Rotate(0, 0, speedRotation * Time.deltaTime);
        UpdateDuration();

        if (unit != null && unit.IsAlive())
        {
            Vector2 toOwner = unit.transform.position - transform.position;
            body.AddForce(toOwner * forceScale);

            if (duration <= 0 ||
                timeFly >= timeFlyReturn && toOwner.magnitude <= distanceReturn)
            {
                Die();
            }
        }
    }
}
