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
	
    // 死亡时的效果力矩
    protected static float torqueDeath = 1000f;

    // 可能的Sprite
    public Sprite[] sprites;

    // 碰撞时对对方造成的伤害（每1有效相对速度造成damageCollision点伤害）
    public float damageCollision = 10f;

    // 碰撞造成伤害的最小相对速度
    protected float velocityCollision = 3f;

    // 玩家
    [HideInInspector] public Player player;

	// 在网格中的位置（-1代表未在网格中）
	[HideInInspector] public int gridX = -1;
	[HideInInspector] public int gridY = -1;


	// Link Direction:
	// 0: Right, 1: Top, 2: Left, 3: Bottom
	/// <summary> 物体朝向的方向 </summary>
	public int direction;
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
        DeathCheck();
	}

    protected virtual void FixedUpdate()
    {

    }

    // 游戏开始时调用
    public virtual void GameStart()
    {
        // 设置刚体类型
        body.bodyType = RigidbodyType2D.Dynamic;
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
		Rotate(1);
	}

	/// <summary> times必须是正数 </summary>
	public void Rotate(int times)
	{
		Debug.Assert(times >= 0);
		direction = (direction + times) % 4;
		transform.Rotate(0, 0, times * 90f);
	}

	// 碰撞
	protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameController.gamePhase == GamePhase.Playing)
        {
            // 与之碰撞的另一个Unit
            Unit unit = collision.gameObject.GetComponent<Unit>();

            if (unit != null && unit.gameObject.layer != (int)Layer.Ground)
            {
                // 造成伤害的有效相对速度
                float velocity = collision.relativeVelocity.magnitude - velocityCollision;

                if (velocity >= 0)
                {
                    float damage = velocity * damageCollision;

                    unit.TakeDamage((int)damage);
                }
            }
        }
    }

	// 受伤
	public void TakeDamage(int damage)
	{
        // 生命值减少
        health -= damage;
	}

    // 死亡检测
    protected virtual void DeathCheck()
    {
        if (!IsAlive())
        {
			Destroy(gameObject);
			//创建一个尸体, deathDuration后删除
			GameObject corpse = CreateDeathClone(this.gameObject);
			Destroy(corpse, deathDuration);
		}
    }
	
	protected virtual void OnDestroy()
	{
		
	}

	/// <summary>
	/// 创建一个保留renderer,旋转rigidbody,无script组件和碰撞体的克隆
	/// </summary>
	protected GameObject CreateDeathClone(GameObject origin)
	{
		// 将origin整体复制
		Transform transClone = Instantiate(origin.transform);
		// 将tag改成Untagged就不会被findEnemy
		transClone.tag = "Untagged";

		// 移除所有脚本和碰撞体
		MonoBehaviour[] scripts = transClone.GetComponents<MonoBehaviour>();
		System.Array.ForEach(scripts, script => Destroy(script));
		Collider2D[] colliders = transClone.GetComponents<Collider2D>();
		System.Array.ForEach(colliders, collider => Destroy(collider));

		// 取消刚体固定
		Rigidbody2D cloneBody = transClone.GetComponent<Rigidbody2D>();
		cloneBody.constraints = RigidbodyConstraints2D.None;
		// 无阻力
		cloneBody.drag = 0;
		cloneBody.angularDrag = 0;
		// 死亡扭矩
		cloneBody.AddTorque(torqueDeath);

		return transClone.gameObject;
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
