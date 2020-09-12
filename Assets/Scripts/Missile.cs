using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    // 发射力
    public float forceLaunch;

    // 冲击力
    public float forceHit;

    // 生命值（生命值为0时，仅不能造成伤害）
    public float health;

    // 伤害
    public int damage;

    // 飞行时间（飞行时间为0时，才判定为死亡）
    public float duration;

    // 撞击后剩余飞行时间
    public float durationHit;

    // 存活
    public bool alive;

    // 粒子预设
    public GameObject particlePrefab;

    protected Rigidbody2D body;

    protected virtual void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        UpdateDuration();
        DeathCheck();
        ParticleTail();
    }

    // 将GameObject强制类型转换为Missile
    public static explicit operator Missile(GameObject gameObject)
    {
        return gameObject.GetComponent<Missile>();
    }
    
    // 尾部粒子
    protected virtual void ParticleTail()
    {
        if (particlePrefab != null)
        {
            GameObject particle = Instantiate(particlePrefab);
            particle.transform.position = transform.position;
        }
    }

    public virtual void Launch()
    {
        if (body == null)
        {
            body = GetComponent<Rigidbody2D>();
        }
        body.AddForce(transform.right * forceLaunch);
    }

    // 更新飞行时间
    protected void UpdateDuration()
    {
        duration -= Time.deltaTime;
    }

    // 死亡检测
    protected void DeathCheck()
    {
        // 生命值不足0
        if (health <= 0)
        {
            alive = false;
        }

        // 飞行时间不足0
        if (duration <= 0)
        {
            Die();
        }
    }

    // 死亡
    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (alive)
        {
            // 击中Ball
            Ball ball = other.gameObject.GetComponent<Ball>();
            if (ball != null)
            {
                ball.health -= damage;
                ball.body.AddForce(transform.right * forceHit);
            }

            // 击中Block
            Block block = other.gameObject.GetComponent<Block>();
            if (block != null)
            {
                block.health -= damage;
            }

            duration = durationHit;
            alive = false;
        }
    }
}
