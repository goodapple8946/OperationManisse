using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : Unit
{
    // 武器发射偏移
    protected float weaponOffset = 0.48f;

    // 武器冷却CD
    public float weaponCDMax;
    protected float weaponCD = 0;

    // 弹药预设
    public GameObject missilePrefab;

    // 将GameObject强制类型转换为Ball
    public static explicit operator Ball(GameObject gameObject)
    {
        return gameObject.GetComponent<Ball>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        FindEnemy();
    }

    /// <summary>
    /// 购买并赋予tag，Ball在Unit的基础上增加Rigidbody
    /// </summary>
    public override void Buy(string tag)
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

            // 获得Rigidbody
            body = gameObject.AddComponent<Rigidbody2D>();
            body.useAutoMass = true;
        }
    }

    // 索敌
    protected virtual void FindEnemy()
    {
        if (alive && tag != "Goods" && GameController.gamePhase == GameController.GamePhase.Playing)
        {
            weaponCD -= Time.deltaTime;

            if (weaponCD <= 0)
            {
                Attack();
                weaponCD = weaponCDMax;
            }
        }
    }

    // 攻击
    protected virtual void Attack()
    {
        // 创建弹药
        Missile missile = (Missile)Instantiate(missilePrefab);

        // 弹药发射点
        missile.transform.position = transform.position + transform.right * weaponOffset;

        // 弹药朝向
        missile.transform.right = transform.right;

        // 添加入BattleObjects
        missile.transform.parent = gameController.battleObjects.transform;

        // Layer
        missile.gameObject.layer = (int)GameController.Layer.PlayerMissile;

        // 发射
        missile.Launch();
    }

    private void OnMouseDown()
    {
        if (GameController.gamePhase == GameController.GamePhase.Preparation && Input.GetMouseButton(0))
        {
            Buy("Unit");
            SetLayer(2);
            if (body != null)
            {
                body.bodyType = RigidbodyType2D.Static;
            }
        }
    }

    private void OnMouseDrag()
    {
        if (GameController.gamePhase == GameController.GamePhase.Preparation && Input.GetMouseButton(0))
        {
            Drag();
        }
    }

    private void OnMouseUp()
    {
        if (GameController.gamePhase == GameController.GamePhase.Preparation)
        {
            SetLayer(0);
            if (body != null)
            {
                body.bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }
}
