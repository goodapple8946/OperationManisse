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

    // 朝向，"left"或"right"
    public string toward = "right";

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

                // 寻找敌人
                if (!TargetCheck(target) || !(target is Ball))
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
        // 检查方向，保证武器图像顶部朝上
        CheckToward();
    }

    // 检查目标是否合法
    protected virtual bool TargetCheck(Unit unit)
    {
        return
            unit != null && unit.isAlive && !unit.isSelling &&
            (unit.transform.position - transform.position).magnitude <= weaponRange &&
            (player == 1 && unit.player == 2 || player == 2 && unit.player == 1);
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

            // 目标合法
            if (TargetCheck(unit))
            {
                float priority = EnemyPriority(unit);

                // 优先级更高的目标
                if (priority > priorityTarget)
                {
                    target = unit;
                    priorityTarget = priority;
                }
            }
        }
    }

    // 索敌优先级
    protected virtual int EnemyPriority(Unit unit)
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
            MeleeAttack();
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
            weaponCD = weaponCDMax * Random.Range(1f, 1.1f);

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

    // 获取俯仰角，返回值范围[-90, 90]
    // 朝向为"right"时：X轴正方向为0度，逆时针为正，顺时针为负
    // 朝向为"left"时：X轴负方向为0度，顺时针为正，逆时针为负
    public float GetPitchAngle()
    {
        float angle = Vector2.SignedAngle(Vector2.right, transform.right);

        if (toward == "left")
        {
            angle = 180f - angle;
        }

        return angle;
    }

    // 朝向检测
    protected virtual void CheckToward()
    {
        if (transform.localEulerAngles.z < 90f || transform.localEulerAngles.z > 270f)
        {
            SetToward("right");
        }
        else
        {
            SetToward("left");
        }
    }

    // 设置朝向
    protected virtual void SetToward(string to)
    {
        if (toward != to)
        {
            if (toward == "left" && to == "right")
            {
                transform.GetChild(0).localScale = new Vector3(1, 1, 1);
            }
            else if (toward == "right" && to == "left")
            {
                transform.GetChild(0).localScale = new Vector3(1, -1, 1);
            }
            toward = to;
        }
    }
}
