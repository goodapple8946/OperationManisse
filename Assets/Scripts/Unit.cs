﻿using System.Collections;
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

    public Rigidbody2D body;
    protected GameController gameController;
    protected ResourceController resourceController;

    protected virtual void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        resourceController = GameObject.Find("Resource Controller").GetComponent<ResourceController>();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        if (clickable && gameController.gamePhase == GameController.GamePhase.Preparation)
        {
            BoundCheckPreparation();
        }
        if (isAlive && !isSelling && isForceProvider && gameController.gamePhase == GameController.GamePhase.Playing)
        {
            ProvideForce();
        }
        DeathCheck();
    }

    protected virtual void OnMouseOver()
    {
        // 准备阶段
        if (clickable && gameController.gamePhase == GameController.GamePhase.Preparation)
        {
            // 鼠标左键按下
            if (Input.GetMouseButtonDown(0))
            {
                if (isSelling)
                {
                    Buy();
                }

                if (body != null)
                {
                    body.bodyType = RigidbodyType2D.Static;
                }

                SetSpriteSortingLayer("Pick");
            }

            // 鼠标左键抬起
            if (Input.GetMouseButtonUp(0))
            {
                SetSpriteSortingLayer("Unit");

                if (body != null)
                {
                    body.bodyType = RigidbodyType2D.Dynamic;
                }
            }
        }

        // 鼠标右键按下
        if (Input.GetMouseButtonDown(1))
        {
            if (clickable && !isSelling && isSellable)
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
            
        }
    }

    protected virtual void OnMouseOut()
    {
        GetComponent<SpriteRenderer>().sortingLayerName = "Unit";

        if (body != null)
        {
            body.bodyType = RigidbodyType2D.Dynamic;
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

    // 购买
    public virtual void Buy()
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
            }
            isSelling = false;
            player = 1;
            transform.parent = gameController.playerObjects.transform;

            // 添加Rigidbody
            body = gameObject.AddComponent<Rigidbody2D>();
            body.mass = mass;
            FreezeRotation();

            gameController.playerMoney -= price;
        }
        // 钱不够
        else
        {
            StartCoroutine(gameController.MoneyNotEnough());
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

    // 准备阶段边界检测
    protected void BoundCheckPreparation()
    {
        // 超过边界
        if (transform.position.x >= gameController.boundRightPreparation)
        {
            if (body != null)
            {
                body.bodyType = RigidbodyType2D.Dynamic;
                body.velocity = new Vector2(-4f, 3f);
            }

            // 关闭Clickable
            clickable = false;

            // 恢复Clickable
            StartCoroutine(ResetClickable());
        }
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
}
