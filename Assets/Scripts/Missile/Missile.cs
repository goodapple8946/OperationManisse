using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Controller;

public class Missile : MonoBehaviour
{
    // 发射力
    public float forceLaunch;

    // 冲击力施加给目标的力
    public float forceHit;

    // 伤害
    public int damageAmount;

    // 飞行时间（飞行时间为0时，才判定为死亡）
    public float duration;

    // 发生碰撞后, 剩余飞行时间
    public float durationHit;

    // 存活 （不存活时仍然存在，但是不造成伤害）
    [HideInInspector] public bool isAlive = true;

    // 发射粒子预设
    public GameObject particleLaunchPerfab;

    // 飞行粒子预设
    public GameObject particleFlyingPrefab;

    // 撞击粒子预设
    public GameObject particleHitPrefab;

    // 所属玩家
    [HideInInspector] public Player player;

    // 发射者
    [HideInInspector] public Unit unit;

    // 发射音效
    public AudioClip[] audiosLaunch;

    public Rigidbody2D body;

    protected virtual void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    protected virtual void FixedUpdate()
    {
        if (isAlive)
        {
            CreateParticleTail();
            RotateToVelocity();
        }

		UpdateDuration();

        if (duration <= 0)
        {
            Die();
        }
    }

    /// <summary>
	/// 由于layer不会与右方碰撞,所以不会对友方造成伤害
	/// </summary>
	protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (isAlive)
        {
			Unit unit = other.gameObject.GetComponent<Unit>();
			if (unit != null)
			{
				
				unit.TakeDamage(CreateDamage());
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

            // 剩余飞行时间
			duration = durationHit;
			isAlive = false;
        }
    }

    // 尾部粒子
    protected virtual void CreateParticleTail()
    {
        if (particleFlyingPrefab != null)
        {
            GameObject particle = Instantiate(particleFlyingPrefab);
            particle.transform.position = transform.position;
        }
    }

    // 创建发射粒子,施加发射力,播放发射声音
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

		PlayAudioes();
	}

	protected void PlayAudioes()
	{
		if (audiosLaunch.Length > 0)
		{
			int rand = Random.Range(0, audiosLaunch.Length);
			AudioSource.PlayClipAtPoint(audiosLaunch[rand], transform.position);
		}
	}

	protected virtual Damage CreateDamage()
	{
		Damage damage = new Damage(
			this.damageAmount,
			this.GetType(),
			forceHit * transform.right);
		return damage;
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
