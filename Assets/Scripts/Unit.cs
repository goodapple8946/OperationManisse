using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // 生命值
    public int health;

    // 是否存活
    public bool alive;

    // 死亡持续时间
    protected float deathDuration;

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

        deathDuration = 3f;
        isWheel = false;
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
        if (alive)
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
        alive = false;
        Destroy(gameObject.GetComponent<Collider2D>());

        // 等待死亡持续时间
        yield return new WaitForSeconds(deathDuration);

        // 摧毁物体
        Destroy(gameObject);
    }

    /// <summary>
    /// 购买并赋予tag
    /// </summary>
    public virtual void Buy(string tag)
    {
        // 可以购买
        if (this.tag == "Goods")
        {
            // 可以重复购买
            if (rebuyable)
            {
                // 在同一位置创建相同的商品
                Unit unit = (Unit)gameController.Create(transform.position, prefab);
                unit.tag = "Goods";
                unit.clickable = true;
                unit.alive = true;
                unit.transform.parent = transform.parent;
                unit.gameObject.layer = (int)GameController.Layer.Goods;
            }
            this.tag = tag;
            transform.parent = gameController.battleObjects.transform;
            gameObject.layer = (int)GameController.Layer.PlayerUnit;
        }
    }

    // 拖动
    protected void Drag()
    {
        transform.position += (Vector3)MouseController.offset;
    }
}
