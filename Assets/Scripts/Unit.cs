using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // 生命值
    public int health;

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

    // 是轮子
    public bool isWheel;

    // 正在出售
    public bool isSelling;

    // 所属玩家
    public int player;

    // 作为目标时的优先级
    public int priority;

    // 半径
    protected float radius = 0.3f;

    // 地面检测射线起始点在底部的向下偏移
    protected float groundCheckOffset = 0.01f;

    // 地面检测射线长度
    protected float groundCheckDistance = 0.01f;

    public Rigidbody2D body;
    protected GameController gameController;

    // 将GameObject强制类型转换为Unit
    public static explicit operator Unit(GameObject gameObject)
    {
        return gameObject.GetComponent<Unit>();
    }

    protected virtual void Start()
    {
        body = GetComponent<Rigidbody2D>();
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    protected virtual void Update()
    {
        DeathCheck();
    }

    /// <summary>
    /// 修改显示层级
    /// </summary>
    /// <param name="layer">显示层级</param>
    public void SetLayer(int layer)
    {
        GetComponent<SpriteRenderer>().sortingOrder = layer;
    }

    // 死亡检测
    protected void DeathCheck()
    {
        if (isAlive)
        {
            // 生命值为0或以下
            if (health <= 0)
            {
                StartCoroutine(Die());
            }
        }
    }

    // 死亡
    protected virtual IEnumerator Die()
    {
        // 开始死亡效果
        isAlive = false;
        Destroy(gameObject.GetComponent<Collider2D>());

        if (body != null)
        {
            body.constraints = RigidbodyConstraints2D.None;
        }

        // 等待死亡持续时间
        yield return new WaitForSeconds(deathDuration);

        // 摧毁物体
        Destroy(gameObject);
    }

    /// <summary>
    /// 购买
    /// </summary>
    public virtual void Buy()
    {
        // 可以购买
        if (isSelling)
        {
            // 可以重复购买
            if (rebuyable)
            {
                // 在同一位置创建相同的商品
                Unit unit = (Unit)gameController.Create(transform.position, prefab);
                unit.transform.parent = transform.parent;
                unit.gameObject.layer = (int)GameController.Layer.Goods;
            }
            isSelling = false;
            player = 1;
            transform.parent = gameController.playerObjects.transform;
            gameObject.layer = (int)GameController.Layer.PlayerUnit;

            // 添加Rigidbody
            body = gameObject.AddComponent<Rigidbody2D>();
            body.useAutoMass = true;
            FreezeRotation();
        }
    }

    // 购买后锁定旋转
    protected virtual void FreezeRotation()
    {
        body.constraints = RigidbodyConstraints2D.FreezeRotation;
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
            block = (Block)hit.transform.gameObject;
            return true;
        }
        block = null;
        return false;
    }

    // 拖动
    protected void Drag()
    {
        transform.position += (Vector3)MouseController.offset;
    }

    protected virtual void OnMouseDown()
    {
        if (clickable && GameController.gamePhase == GameController.GamePhase.Preparation && Input.GetMouseButton(0))
        {
            Buy();
            SetLayer(2);
            if (body != null)
            {
                body.bodyType = RigidbodyType2D.Static;
            }
        }
    }

    protected virtual void OnMouseDrag()
    {
        if (clickable && GameController.gamePhase == GameController.GamePhase.Preparation && Input.GetMouseButton(0))
        {
            Drag();
        }
    }

    protected virtual void OnMouseUp()
    {
        if (clickable && GameController.gamePhase == GameController.GamePhase.Preparation)
        {
            SetLayer(0);
            if (body != null)
            {
                body.bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }
}
