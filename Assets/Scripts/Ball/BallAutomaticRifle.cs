using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAutomaticRifle : Ball
{
    // 三连发计数
    private int shootCount = 0;

    // 远程攻击：三连发
    protected override void RangedAttack()
    {
        if (weaponCD <= 0)
        {
            // 发射计数增加
            shootCount++;

            if (shootCount < 3)
            {
                // 武器冷却连发重置
                weaponCD = weaponCDMax * 0.1f;
            }
            else
            {
                // 武器冷却重置
                weaponCD = weaponCDMax * Random.Range(1f, 1.1f);
                shootCount = 0;
            }

            // 创建弹药
            Missile missile = Instantiate(missilePrefab).GetComponent<Missile>();

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
}
