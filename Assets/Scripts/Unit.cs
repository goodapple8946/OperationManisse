using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static GameController;

public abstract class Unit : MonoBehaviour
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
    protected float torqueDeath = 1000f;

    // 可能的Sprite
    public Sprite[] sprites;

    // 碰撞时对对方造成的伤害（每1有效相对速度造成damageCollision点伤害）
    public float damageCollision = 10f;

    // 碰撞造成伤害的最小相对速度
    protected float velocityCollision = 3f;

    // 玩家
    public Player player;

    // 在网格中的位置（-1代表未在网格中）
    public int gridX = -1;
    public int gridY = -1;

    // 价格
    public int price;

    // 是编辑器创建的
    public bool isEditorCreated;

    public Rigidbody2D body;
    protected GameController gameController;
    protected EditorController editorController;
    protected ResourceController resourceController;

	protected virtual void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        editorController = GameObject.Find("Editor Controller").GetComponent<EditorController>();
        resourceController = GameObject.Find("Resource Controller").GetComponent<ResourceController>();
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

    protected void OnMouseOver()
    {
        // 鼠标不在UI上
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            // 鼠标左键按下
            if (Input.GetMouseButton(0))
            {
                LeftClick();
            }

            // 鼠标右键按下
            if (Input.GetMouseButton(1))
            {
                RightClick();
            }
        }
    }

    // 鼠标左键按下
    protected virtual void LeftClick()
    {
        editorController.LeftClickUnit(this);
    }

    // 鼠标右键按下
    protected virtual void RightClick()
    {
        editorController.RightClickUnit(this);
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

    // 碰撞
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (gamePhase == GamePhase.Playing)
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
            Die();
        }
    }

    // 死亡
    protected virtual void Die()
    {
        // 解除固定
        body.constraints = RigidbodyConstraints2D.None;

        // 无阻力
        body.drag = 0;
        body.angularDrag = 0;

        // 移除碰撞
        Destroy(GetComponent<Collider2D>());


        // 摧毁物体
        Destroy(gameObject, deathDuration);
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

    // 设置图像层级
    public void SetSpriteLayer(string layer)
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.sortingLayerName = layer;
        }
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer spriteChild in sprites)
        {
            if (spriteChild.sortingLayerName != "Cover" && spriteChild.sortingLayerName != "Outline")
            {
                spriteChild.sortingLayerName = layer;
            }
        }
    }

    // 是否存活
    public bool IsAlive()
    {
        return health > 0;
    }
}
