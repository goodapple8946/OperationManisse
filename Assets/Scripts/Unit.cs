using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using static Controller;

public abstract class Unit : ClickableObject
{
	// 生命最大值
	public int healthMax;

	// 生命值
	public int health;

	// 死亡持续时间
	protected float deathDuration = 3f;

	// 作为目标时的优先级
	public int priority;

	// 半径
	public float radius;

	// 地面检测射线起始点在底部的向下偏移
	protected float groundCheckOffset = 0.01f;

	// 地面检测射线长度
	protected float groundCheckDistance = 0.05f;

	// 可能的Sprite
	public Sprite[] sprites;

	// 碰撞时对对方造成的伤害（每1有效相对速度造成damageCollision点伤害）
	public float damageCollision = 10f;

	// 碰撞造成伤害的最小相对速度
	protected float velocityCollision = 3f;

	// 玩家
	[HideInInspector] public Player player;

	// 在网格中的位置（OUTSIDE代表未在网格中）
	public Coord coord = Coord.OUTSIDE;


	// Link Direction:
	// 0: Right, 1: Top, 2: Left, 3: Bottom
	/// <summary> 物体朝向的方向 </summary>
	public int direction;
	protected Vector2[] dirVector = {
		new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1) };
	//public int Direction
	//{
	//	get => direction;
	//	set
	//	{
	//		// 设置direction并更新方向
	//		transform.Rotate(0, 0, (value - Direction) * 90f);
	//		direction = value;
	//	} 
	//}

	// 价格
	public int price;

	// 是编辑器创建的
	[HideInInspector] public bool isEditorCreated;

	public Rigidbody2D body;

	protected virtual void Awake()
	{
		body = GetComponent<Rigidbody2D>();
	}

	protected virtual void Start()
	{
		HPBarInit();
		UpdateSprite();
	}

	protected virtual void Update()
	{

	}

	protected virtual void FixedUpdate()
	{

	}

	// 游戏开始时调用
	public virtual void GameStart()
	{
		// 设置自身和所有子物体的刚体类型
		body.bodyType = RigidbodyType2D.Dynamic;

		Rigidbody2D[] childBodies = gameObject.GetComponentsInChildren<Rigidbody2D>();
		System.Array.ForEach(childBodies, childBody => childBody.bodyType = RigidbodyType2D.Dynamic);
	}

	// 更新贴图变种
	public void UpdateSprite()
	{
		// 贴图变种
		if (sprites.Length > 0)
		{
			int rand = Random.Range(0, sprites.Length);
			transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = sprites[rand];
		}
	}

	// 生命值条初始化
	protected virtual void HPBarInit()
	{
		// 生命值条
		HPBar hPBar = Instantiate(resourceController.hpBarPrefab).GetComponent<HPBar>();
		hPBar.unit = this;
		hPBar.transform.parent = gameController.hpBarObjects.transform;
	}

	/// <summary>
	/// 顺时针转动一下
	/// </summary>
	public virtual void Rotate()
	{
		transform.Rotate(0, 0, 90f);
		direction = (direction + 1) % 4;
	}

	/// <summary> 旋转多少个90度 </summary>
	public void Rotate(int dirBias)
	{
		while(dirBias < 0 || dirBias >= 4)
		{
			if (dirBias < 0)
				dirBias += 4;
			else if (dirBias >= 4)
				dirBias -= 4;
		}

		if(this is Block)
		{
			while (dirBias-- > 0) Rotate();
		}
		else if(this is Ball)
		{
			while (dirBias > 0)
			{
				Rotate();
				dirBias -= 2;
			}
		}
		
	}

	// 碰撞
	protected virtual void OnCollisionEnter2D(Collision2D collision)
	{
		if (gameController.GamePhase == GamePhase.Playing)
		{
			// 与之碰撞的另一个Unit
			Unit unit = collision.gameObject.GetComponent<Unit>();

			if (unit != null && unit.gameObject.layer != (int)Layer.Terrain)
			{
				// 造成伤害的有效相对速度
				float velocity = collision.relativeVelocity.magnitude - velocityCollision;

				if (velocity >= 0)
				{
					float damageAmount = velocity * damageCollision;
					float HIT_FORCE_FACTOR = 30f;
					Damage damage = new Damage(
						(int)damageAmount,
						unit.GetType(),
						HIT_FORCE_FACTOR * velocity * transform.right);
					unit.TakeDamage(damage);
				}
			}
		}
	}

	// 受伤
	public void TakeDamage(Damage damage)
	{
		// 生命值减少
		health -= damage.Amount;

		// 死亡检测
		if (!IsAlive())
		{
			ProcessDeath(damage);
		}
	}

	/// <summary>
	/// 根据伤害类型来进行死亡效果
	/// </summary>
	protected void ProcessDeath(Damage damage)
	{
		System.Type damageType = damage.DamageType;
		Vector2 force = damage.Force;

		Destroy(gameObject);
		//创建一个尸体, deathDuration后删除
		GameObject corpse;
		if (damageType == typeof(MissileFlamethrower))
		{
			corpse = Util.CreateBurningClone(gameObject);
		}
		// 射伤
		else if (damageType == typeof(Missile)
			|| damageType.IsInstanceOfType(typeof(Missile))
			|| damageType == typeof(BlockSpring)) 
		{
			corpse = Util.CreateShootedClone(gameObject);
		}
		// 撞击
		else
		{
			corpse = Util.CreatePunchClone(gameObject, force);
		}
		Destroy(corpse, deathDuration);
	}
	
	protected virtual void OnDestroy()
	{
		
	}

	// 是否在地面上
	public bool IsGrounded()
    {
        // 射线起始点
        Vector2 origin = (Vector2)transform.position + Vector2.down * (radius + groundCheckOffset);

        // 向下发射射线
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance);

        // 如果射线触碰到物体
        return hit.transform != null;
    }

    // 是否存活
    public bool IsAlive()
    {
        return health > 0;
    }
}
