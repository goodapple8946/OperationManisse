using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameController;

public class Ball : Unit
{
    // 武器发射偏移
    public float weaponOffset;

    // 索敌范围
    public float findEnemyRange;

    // 旋转速率
    protected float rotationSpeed = 2f;

    // 武器冷却最大值
    public float weaponCDMax;

    // 武器冷却
    protected float weaponCD = 0;

    // 武器角度
    public float weaponAngle;

    // 弹药预设
    public GameObject missilePrefab;

    // 目标优先级容差
    private float priorityTolerant = 3f;

    protected override void Update()
    {
        base.Update();

        WeaponCoolDown();

        if (IsAlive() && gamePhase == GamePhase.Playing)
        {
            // 寻找敌人
            Unit target = FindEnemy();

            // 已经有目标或索敌找到目标
            if (target != null)
            {
                // 转向目标
                bool alreadyAimed = AimTo(target);
                if (alreadyAimed)
                {
                    // 检查CD，若CD允许，则攻击
                    CheckCDAndAttack(target);
                }
            }
        }
        // 更新朝向，保证武器图像顶部朝上
        UpdateToward();
    }

    // 检查目标是否合法
    protected bool IsLegalTarget(Unit unit)
    {
        // 射程范围之内
        bool unitInRange = (unit.transform.position - transform.position).magnitude <= findEnemyRange;

        // 敌对正营
        bool unitIsOpponent =
            player == Player.Player && unit.player == Player.Enemy ||
            player == Player.Enemy && unit.player == Player.Player;

        return unit.IsAlive() && unitInRange && unitIsOpponent;
    }
    
    // 索敌。返回最佳目标，如果没有则返回null
    protected Unit FindEnemy()
    {
        ArrayList gameObjects = new ArrayList();
        gameObjects.AddRange(GameObject.FindGameObjectsWithTag("Ball"));
        gameObjects.AddRange(GameObject.FindGameObjectsWithTag("Block"));

        Unit currentTarget = null;
        float currentPriority = float.MinValue;
        foreach (GameObject gameObject in gameObjects)
        {
            Unit unit = gameObject.GetComponent<Unit>();

            // 目标合法
            if (IsLegalTarget(unit))
            {
                float priority = GetTargetPriority(unit);

                // 优先级更高的目标
                if (priority > currentPriority + priorityTolerant)
                {
                    currentTarget = unit;
                    currentPriority = priority;
                }
            }
        }
        return currentTarget;
    }

    // 索敌优先级
    protected int GetTargetPriority(Unit unit)
    {
        float distance = (unit.transform.position - transform.position).magnitude;
        int priority = 0;

        // 目标在射程内
        if (distance <= findEnemyRange)
        {
            priority += (int)((findEnemyRange - distance) * 10f);
            priority += unit.priority;
        }
        return priority;
    }

    // 攻击
    protected void CheckCDAndAttack(Unit target)
    {
        if (weaponCD <= 0)
        {
            // 武器冷却重置
            weaponCD = weaponCDMax * Random.Range(1f, 1.1f);

            RangedAttack();
        }
    }

    /// <summary>
    /// 转动武器，尝试瞄准目标，返回是否已经瞄准目标
    /// </summary>
    protected bool AimTo(Unit target)
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
        return angle <= weaponAngle;
    }

    // 远程攻击
    protected virtual void RangedAttack()
    {
        // 创建弹药
        Missile missile = CreateMissile();

        // 发射弹药
        missile.Launch();
    }

    // 武器冷却
    protected virtual void WeaponCoolDown()
    {
        weaponCD -= Time.deltaTime;
    }

    // 朝向检测
    protected virtual void UpdateToward()
    {
        if (transform.localEulerAngles.z < 90f || transform.localEulerAngles.z > 270f)
        {
            transform.GetChild(1).localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.GetChild(1).localScale = new Vector3(1, -1, 1);
        }
    }

    // 创建弹药
    protected Missile CreateMissile()
    {
        // 创建弹药
        Missile missile = Instantiate(missilePrefab).GetComponent<Missile>();

        // 弹药玩家
        missile.player = player;

        // 弹药发射点
        missile.transform.position = transform.position + transform.right * weaponOffset;

        // 弹药朝向
        missile.transform.right = transform.right;

        // 弹药随机角度
        missile.transform.Rotate(0, 0, Random.Range(-weaponAngle, weaponAngle));

        // 设置父物体
        missile.transform.parent = gameController.missileObjects.transform;

        // Layer
        if (gameObject.layer == (int)Layer.PlayerBall)
        {
            missile.gameObject.layer = (int)Layer.PlayerMissile;
        }
        else if (gameObject.layer == (int)Layer.EnemyBall)
        {
            missile.gameObject.layer = (int)Layer.EnemyMissile;
        }

        return missile;
    }
}
