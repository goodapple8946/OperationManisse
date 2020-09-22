using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    // 生命最大值
    public int healthMax;

    // 生命值
    public int health;

    // 无敌的
    public bool isInvincible;

    // 是否存活
    public bool isAlive;

    // 死亡持续时间
    protected float deathDuration = 3f;

    // 价格
    public int price;

    // 可重复购买
    public bool rebuyable;

    // 预设
    public GameObject prefab;

    // 可点击
    public bool clickable;

    // 正在出售
    public bool isSelling;

    // 可以出售
    public bool isSellable;

    // 所属玩家
    public int player;

    // 作为目标时的优先级
    public int priority;

    // 半径
    public float radius;

    // 地面检测射线起始点在底部的向下偏移
    protected float groundCheckOffset = 0.01f;

    // 地面检测射线长度
    protected float groundCheckDistance = 0.05f;

    // 死亡时的效果力矩
    protected float torqueDeath = 1000f;

    // 质量
    public float mass;

    // 是力的提供者
    public bool isForceProvider;

    // 力
    public float force;

    // 力的角度
    public float forceAngle;

    // 最大速度（未达最大速度时可以提供力）
    public float speedMax;

    // 粒子预设
    public GameObject particlePrefab;

    // 粒子
    public GameObject particle;

    // 可能的Sprite
    public Sprite[] sprites;

    // 碰撞时对对方造成的伤害（每1有效相对速度造成damageCollision点伤害）
    public float damageCollision = 10f;

    // 碰撞造成伤害的最小相对速度
    protected float velocityCollision = 3f;

    public Rigidbody2D body;
    protected GameController gameController;
    protected ResourceController resourceController;

	// 一次伤害闪烁次数
	private int flashTimes = 2;
	// 剩余闪烁次数
	private int currFlashTimes = 0;
	// 每次闪烁间隔
	private float flashGapTime = 0.02f;
	// 当前剩余闪烁间隔
	private float currFlashGapTime = 0.0f;

	protected virtual void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        resourceController = GameObject.Find("Resource Controller").GetComponent<ResourceController>();
    }

    protected virtual void Start()
    {
        // 生命值条
        HPBar hPBar = Instantiate(resourceController.hpBarPrefab).GetComponent<HPBar>();
        hPBar.unit = this;

        // 贴图变种
        if (sprites.Length > 0)
        {
            int rand = Random.Range(0, sprites.Length);
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = sprites[rand];
        }
    }

    protected virtual void Update()
    {
        if (isAlive)
        {
            if (!isSelling && isForceProvider && (gameController.gamePhase == GameController.GamePhase.Playing || gameController.gamePhase == GameController.GamePhase.Victory))
            {
                ProvideForce();
            }
            
            if (health <= 0)
            {
                StartCoroutine(Die());
            }
        }
        if (clickable && isAlive && !isSelling && gameController.gamePhase == GameController.GamePhase.Preparation)
        {
            BuildLocationCheck();
        }
		if (currFlashTimes > 0)
		{
			//WaitAndFlash();
		}
	}

    // 碰撞
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        // 可以造成碰撞伤害
        if (isAlive && !isSelling && gameController.gamePhase == GameController.GamePhase.Playing)
        {
            // 与之碰撞的另一个Unit
            Unit unit = collision.gameObject.GetComponent<Unit>();

            // 造成伤害的有效相对速度
            float velocity = collision.relativeVelocity.magnitude - velocityCollision;

            // 满足触发伤害的条件
            if (unit != null && unit.isAlive && !unit.isSelling && !unit.isInvincible && velocity >= 0)
            {
                float damage = velocity * damageCollision;

                unit.TakeDamage((int)damage);
            }
        }
    }

    protected virtual void OnMouseOver()
    {
        // 准备阶段
        if (clickable && gameController.gamePhase == GameController.GamePhase.Preparation)
        {
            // 鼠标左键按下
            if (Input.GetMouseButtonDown(0))
            {
                MouseLeftDown();
            }

            // 鼠标左键抬起
            if (Input.GetMouseButtonUp(0))
            {
                MouseLeftUp();
            }
        }

        // 鼠标右键按下
        if (Input.GetMouseButtonDown(1))
        {
            if (clickable && !isSelling && isSellable)
            {
                MouseRightDown();
            }
        }
    }

    // 鼠标左键按下
    public virtual void MouseLeftDown()
    {
        if (isSelling)
        {
            Buy();
        }

        if (body != null)
        {
            body.bodyType = RigidbodyType2D.Static;
            gameController.unitsDraging.Add(this);
        }

        SetSpriteSortingLayer("Pick");
    }

    // 鼠标左键抬起
    public virtual void MouseLeftUp()
    {
        SetSpriteSortingLayer("Unit");

        if (body != null)
        {
            body.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    // 鼠标右键按下
    public virtual void MouseRightDown()
    {
        // 准备阶段，出售
        if (gameController.gamePhase == GameController.GamePhase.Preparation)
        {
            Sell();
        }
        // 游戏阶段，删除
        else if (gameController.gamePhase == GameController.GamePhase.Playing)
        {
            Delete();
        }
    }

	// 受伤
	public void TakeDamage(int damage)
	{
        // 生命值减少
        health -= damage;
		//FlashOnce();
	}

	private void FlashOnce()
	{
		currFlashTimes = (currFlashTimes % flashTimes) + flashTimes;
	}

	private void WaitAndFlash()
	{
		if (currFlashGapTime > 0)
		{
			// wait for gaptime
			currFlashGapTime -= Time.deltaTime;
		}
		else
		{
			// flash
			SpriteRenderer renderer = transform.GetComponent<SpriteRenderer>();
			if(renderer != null)
			{
				float alpha = (renderer.color.a == 1.0f) ? 0.5f : 1.0f;
				renderer.color = new Color(
					renderer.color.r, renderer.color.g, renderer.color.b, alpha);
				currFlashGapTime = flashGapTime;
				currFlashTimes--;
			}
		}
	}

	protected virtual void OnMouseDrag()
    {
        if (clickable && isAlive && !isSelling && gameController.gamePhase == GameController.GamePhase.Preparation && Input.GetMouseButton(0))
        {
            // 拖动
            transform.Translate(MouseController.offset);
        }
    }

    // 死亡
    protected virtual IEnumerator Die()
    {
        // 开始死亡效果
        isAlive = false;
        Destroy(gameObject.GetComponent<Collider2D>());

        // 删除粒子
        if (particle != null)
        {
            Destroy(particle);
        }

        // 死亡优先显示
        SetSpriteSortingLayer("Dead");

        if (body != null)
        {
            body.constraints = RigidbodyConstraints2D.None;

            // 受力矩旋转
            if (player == 1)
            {
                body.AddTorque(torqueDeath);
            }
            else
            {
                body.AddTorque(-torqueDeath);
            }
        }

        // 等待死亡持续时间
        yield return new WaitForSeconds(deathDuration);

        // 摧毁物体
        Destroy(gameObject);
    }

    // 购买，生成复制商品，返回购买得到的物品
    public virtual Unit Buy()
    {
        // 钱足够
        if (gameController.playerMoney >= price)
        {
            // 可以重复购买
            if (rebuyable)
            {
                // 在同一位置创建相同的商品
                Unit unit = gameController.Create(transform.position, prefab).GetComponent<Unit>();
                unit.transform.parent = transform.parent;
                gameController.unitBought = unit;
            }
            isSelling = false;
            player = 1;
            transform.parent = gameController.playerObjects.transform;

            // 添加Rigidbody
            body = gameObject.AddComponent<Rigidbody2D>();
            body.mass = mass;
            FreezeRotation();

            gameController.playerMoney -= price;
            return this;
        }
        // 钱不够
        else
        {
            StartCoroutine(gameController.MoneyNotEnough());
            return null;
        }
    }

    // 购买后锁定旋转
    protected virtual void FreezeRotation()
    {
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    // 出售
    public virtual void Sell()
    {
        gameController.playerMoney += price;
        Destroy(gameObject);
    }

    // 删除
    public virtual void Delete()
    {
        Destroy(gameObject);
    }

    // 是否在地面上
    public bool IsGrounded()
    {
        // 射线起始点
        Vector2 origin = (Vector2)transform.position + Vector2.down * (radius + groundCheckOffset);

        // 向下发射射线
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance);

        // 如果射线触碰到物体
        if (hit.transform != null)
        {
            return true;
        }
        return false;
    }

    // 是否在地面上，out为下方的Block
    public bool IsGrounded(out Block block)
    {
        // 射线起始点
        Vector2 origin = (Vector2)transform.position + Vector2.down * (radius + groundCheckOffset);

        // 向下发射射线
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance);

        // 如果射线触碰到物体
        if (hit.transform != null)
        {
            block = hit.transform.gameObject.GetComponent<Block>();
            return true;
        }
        block = null;
        return false;
    }

    // 恢复Clickable
    protected IEnumerator ResetClickable()
    {
        yield return new WaitForSeconds(0.5f);
        clickable = true;
    }

    // 提供力
    protected void ProvideForce()
    {
        // 未达最大速度
        if (body != null && body.velocity.magnitude <= speedMax)
        {
            Vector2 forceAdded = Quaternion.AngleAxis(forceAngle, Vector3.forward) * transform.right * force;
            body.AddForce(forceAdded);
        }
    }

    // 设置图像层级（不包括子物体）
    protected void SetSpriteSortingLayer(string layer)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.sortingLayerName = layer;
        }
    }

    // 设置图像层级（包括子物体）
    protected void SetSpriteAndChildSortingLayer(string layer)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.sortingLayerName = layer;
        }
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer spriteChild in sprites)
        {
            if (spriteChild.sortingLayerName != "Cover" && spriteChild.sortingLayerName != "Covered" && spriteChild.sortingLayerName != "Outline")
            {
                spriteChild.sortingLayerName = layer;
            }
        }
    }

    // 准备阶段的建造范围
    protected void BuildLocationCheck()
    {
		// Unit四个边界的坐标
		float rightMost = transform.position.x + radius;
		float leftMost = transform.position.x - radius;
		float topMost = transform.position.y + radius;
		float bottomMost = transform.position.y - radius;
		// 四种超出边界的判断
		bool outOfRightArea = rightMost > gameController.xMaxBuild;
		bool outOfLeftArea = leftMost < gameController.xMinBuild;
		bool outOfTopArea = topMost > gameController.yMaxBuild;
		bool outofBottomArea = bottomMost < gameController.yMinBuild;
		bool outOfBuildingArea = outOfRightArea 
			|| outOfLeftArea || outOfTopArea || outofBottomArea;

		if (outOfBuildingArea)
		{
			Vector3 transVec = Vector3.zero;
			if (outOfRightArea)
			{
				transVec += new Vector3(gameController.xMaxBuild - rightMost - 0.01f, 0, 0);
			}
			else if (outOfLeftArea)
			{
				transVec += new Vector3(gameController.xMinBuild - leftMost + 0.01f, 0, 0);
			}
			if (outOfTopArea)
			{
				transVec += new Vector3(0, gameController.yMaxBuild - topMost - 0.01f, 0);
			}
			else if (outofBottomArea)
			{
				transVec += new Vector3(0, gameController.yMinBuild - bottomMost + 0.01f, 0);
			}
			OutOfBuildingAreaAction(transVec);
			// 更改建造范围框透明度
			float alpha = gameController.GetBuildingAreaAlpha();
			gameController.SetBuildingAreaAlpha(Mathf.Min(alpha + 0.03f, 1.0f));
		}
		else
		{
			// 更改建造范围框透明度
			float alpha = gameController.GetBuildingAreaAlpha();
			gameController.SetBuildingAreaAlpha(Mathf.Max(alpha - 0.03f, 0.0f));
		}
	}

	protected virtual void OutOfBuildingAreaAction(Vector3 transVec)
	{
		// 超出边界模拟鼠标往反方向拖
		MouseLeftDown();
		transform.Translate(transVec);
		MouseLeftUp();
	}

}
