using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unit;

public class Missile : MonoBehaviour
{
    // 发射力
    public float forceLaunch;

    // 冲击力
    public float forceHit;

    // 伤害
    public int damage;

    // 飞行时间（飞行时间为0时，才判定为死亡）
    public float duration;

    // 撞击后剩余飞行时间
    public float durationHit;

    // 存活 （不存活时仍然存在，但是不造成伤害）
    public bool isAlive;

    // 发射粒子预设
    public GameObject particleLaunchPerfab;

    // 飞行粒子预设
    public GameObject particleFlyingPrefab;

    // 撞击粒子预设
    public GameObject particleHitPrefab;

    // 近战
    public bool isMelee;

    // 所属玩家
    public Player player;

    // 发射音效
    public AudioClip[] audiosLaunch;

    public Rigidbody2D body;

    protected virtual void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        if (isAlive)
        {
            ParticleTail();
            RotateToVelocity();
        }

        if (!isMelee)
        {
            UpdateDuration();
        }

        if (duration <= 0)
        {
            Die();
        }
    }

    // 撞击
    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (isAlive)
        {
            // 击中Ball
            Ball ball = other.gameObject.GetComponent<Ball>();
            if (ball != null)
            {
				ball.TakeDamage(damage);
                if (ball.body != null)
                {
                    ball.body.AddForce(transform.right * forceHit);
                }
            }

            // 击中Block
            Block block = other.gameObject.GetComponent<Block>();
            if (block != null)
            {
				block.TakeDamage(damage);
                if (block.body != null)
                {
                    block.body.AddForce(transform.right * forceHit);
                }
            }

            // 撞击粒子
            if (particleHitPrefab != null)
            {
                GameObject particle = Instantiate(particleHitPrefab);
                particle.transform.position = transform.position;
                particle.transform.rotation = transform.rotation;
            }

            // 剩余飞行时间
            if (!isMelee)
            {
                duration = durationHit;
                isAlive = false;
            }
        }
    }

    // 尾部粒子
    protected virtual void ParticleTail()
    {
        if (particleFlyingPrefab != null)
        {
            GameObject particle = Instantiate(particleFlyingPrefab);
            particle.transform.position = transform.position;
        }
    }

    // 发射
    public virtual void Launch()
    {
        // 发射粒子
        if (particleLaunchPerfab != null)
        {
            ParticleSystem particle = Instantiate(particleLaunchPerfab).GetComponent<ParticleSystem>();
            particle.transform.position = transform.position;
            particle.transform.rotation = transform.rotation;
        }

        if (body == null)
        {
            body = GetComponent<Rigidbody2D>();
        }
        body.AddForce(transform.right * forceLaunch);

        if (audiosLaunch.Length > 0)
        {
            int rand = Random.Range(0, audiosLaunch.Length);
            AudioSource.PlayClipAtPoint(audiosLaunch[rand], transform.position);
        }
    }

    // 更新飞行时间
    protected void UpdateDuration()
    {
        duration -= Time.deltaTime;
    }

    // 死亡
    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    // 朝向速度方向
    protected void RotateToVelocity()
    {
        transform.right = body.velocity;
    }
}
