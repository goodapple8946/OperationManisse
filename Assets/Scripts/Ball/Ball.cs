using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : Unit
{
    // 武器发射偏移
    public float weaponOffset;

    // 武器冷却最大值
    public float weaponCDMax;

    // 武器冷却起始随机
    public bool weaponCDRandom;

    // 射程
    public float weaponRange;

    // 武器不精准角度
    public float weaponRandomAngle;

    // 弹药预设
    public GameObject missilePrefab;

	// 是否近战
	public bool isMelee;

	// 旋转速率
	protected float rotationSpeed = 2f;

	// 武器冷却
	protected float weaponCD = 0;

	// 近战武器被创建
	protected bool haveMeleeWeapon = false;

    // 发射物
    protected Missile missile;

    // 发射物关节
    protected Joint2D jointMissile;

    // 当前目标
    protected Unit target;

    protected override void Start()
    {
        base.Start();

        if (weaponCDRandom)
        {
            weaponCD = Random.Range(0, weaponCDMax);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (!isSelling && gameController.gamePhase == GameController.GamePhase.Playing)
        {
            if (isAlive)
            {
                WeaponCoolDown();

                // 如果当前没有目标，或者当前目标太远，寻找敌人
                if (target == null || !target.isAlive || (target.transform.position - transform.position).magnitude > weaponRange)
                {
                    FindEnemy();
                }

                // 已经有目标或索敌找到目标
                if (target != null)
                {
                    AimAndAttack();
                }

                // 如果是近战
                if (isMelee)
                {
                    // 如果没有近战武器
                    if (!haveMeleeWeapon)
                    {
                        // 添加近战武器
                        AddMeleeWeapon();
                    }
                }
            }
            else
            {
                // 如果拥有近战武器，但是死亡了
                if (haveMeleeWeapon)
                {
                    // 释放近战武器
                    ReleaseMeleeWeapon();
                }
            }
        }
    }

    // 索敌
    protected virtual void FindEnemy()
    {
        target = null;

        ArrayList gameObjects = new ArrayList();
        gameObjects.AddRange(GameObject.FindGameObjectsWithTag("Ball"));
        gameObjects.AddRange(GameObject.FindGameObjectsWithTag("Block"));

        float priorityTarget = 0;

        foreach (GameObject gameObject in gameObjects)
        {
            Unit unit = gameObject.GetComponent<Unit>();

            // 目标存活且非卖品
            if (unit.isAlive && !unit.isSelling)
            {
                // 目标是敌对的
                if (player == 1 && unit.player == 2 || player == 2 && unit.player == 1)
                {
                    float priority = enemyPriority(unit);

                    // 优先级更高的目标
                    if (priority > priorityTarget)
                    {
                        target = unit;
                        priorityTarget = priority;
                    }
                }
            }
        }
    }

    // 索敌优先级
    protected virtual int enemyPriority(Unit unit)
    {
        float distance = (unit.transform.position - transform.position).magnitude;
        int priority = 0;

        // 目标在射程内
        if (distance <= weaponRange)
        {
            priority += (int)((weaponRange - distance) * 10f);
            priority += unit.priority;
        }
        return priority;
    }

    // 瞄准并攻击
    protected virtual void AimAndAttack()
    {
        if (isMelee)
        {
            // 自己的朝向
            Vector2 vector = transform.right;

            // 两者间的向量
            Vector2 vectorTarget = target.transform.position - transform.position;

            // 需要旋转的角度
            float angle = Vector2.SignedAngle(vector, vectorTarget);

            // 朝向敌人
            if (angle > rotationSpeed)
            {
                transform.Rotate(0, 0, rotationSpeed);
            }
            else if (angle < -rotationSpeed)
            {
                transform.Rotate(0, 0, -rotationSpeed);
            }
            else
            {
                transform.Rotate(0, 0, angle);
            }

            // 在武器不精准角度范围内，尝试攻击
            if (angle <= weaponRandomAngle)
            {
                MeleeAttack();
            }
        }
        else
        {
            // 自己的朝向
            Vector2 vector = transform.right;

            // 两者间的向量
            Vector2 vectorTarget = target.transform.position - transform.position;

            // 需要旋转的角度
            float angle = Vector2.SignedAngle(vector, vectorTarget);

            // 朝向敌人
            if (angle > rotationSpeed)
            {
                transform.Rotate(0, 0, rotationSpeed);
            }
            else if (angle < -rotationSpeed)
            {
                transform.Rotate(0, 0, -rotationSpeed);
            }
            else
            {
                transform.Rotate(0, 0, angle);
            }

            // 在武器不精准角度范围内，尝试攻击
            if (angle <= weaponRandomAngle)
            {
                RangedAttack();
            }
        }
    }

    // 近战攻击
    protected virtual void MeleeAttack()
    {

    }

    // 远程攻击
    protected virtual void RangedAttack()
    {
        if (weaponCD <= 0)
        {
            // 武器冷却重置
            weaponCD = weaponCDMax;

            // 创建弹药
            Missile missile = Instantiate(missilePrefab).GetComponent<Missile>();

            // 弹药玩家
            missile.player = player;

            // 弹药发射点
            missile.transform.position = transform.position + transform.right * weaponOffset;

            // 弹药朝向
            missile.transform.right = transform.right;

            // 弹药随机角度
            missile.transform.Rotate(0, 0, Random.Range(-weaponRandomAngle, weaponRandomAngle));

            // 弹药初速度
            // missile.body.velocity = body.velocity;

            // 添加入playerObjects
            missile.transform.parent = gameController.playerObjects.transform;

            // Layer
            if (gameObject.layer == (int)GameController.Layer.PlayerBall)
            {
                missile.gameObject.layer = (int)GameController.Layer.PlayerMissile;
            }
            else if (gameObject.layer == (int)GameController.Layer.EnemyBall)
            {
                missile.gameObject.layer = (int)GameController.Layer.EnemyMissile;
            }

            // 发射
            missile.Launch();
        }
    }

    // 武器冷却
    protected virtual void WeaponCoolDown()
    {
        weaponCD -= Time.deltaTime;
    }

    // 添加近战武器
    void AddMeleeWeapon()
    {
        // 删除子对象（武器图像）
        Destroy(transform.GetChild(0).gameObject);

        // 创建武器
        missile = Instantiate(missilePrefab).GetComponent<Missile>();

        // 武器玩家
        missile.player = player;

        // 武器位置
        missile.transform.position = transform.position;

        // 武器朝向
        missile.transform.right = transform.right;

        // 添加入子物体
        missile.transform.parent = transform.parent;

        // 添加关节
        FixedJoint2D joint = missile.gameObject.AddComponent<FixedJoint2D>();
        joint.connectedBody = body;
        joint.enableCollision = false;
        jointMissile = joint;

        // Layer
        if (gameObject.layer == (int)GameController.Layer.PlayerBall)
        {
            missile.gameObject.layer = (int)GameController.Layer.PlayerMissile;
        }
        else if (gameObject.layer == (int)GameController.Layer.EnemyBall)
        {
            missile.gameObject.layer = (int)GameController.Layer.EnemyMissile;
        }

        haveMeleeWeapon = true;
    }

    // 释放近战武器
    void ReleaseMeleeWeapon()
    {
        // 删除关节
        Destroy(jointMissile);

        // 视为远程武器
        missile.isMelee = false;

        missile = null;
        haveMeleeWeapon = false;
    }
}
