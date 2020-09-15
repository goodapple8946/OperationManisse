using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLance : Ball
{
    private bool isForward;

    private float speedBackward;

    // 添加近战武器
    void AddMeleeWeapon()
    {
        // 删除子对象（武器图像）
        Destroy(transform.GetChild(0).gameObject);

        // 创建武器
        missile = Instantiate(missilePrefab).GetComponent<Missile>();

        // 武器位置
        missile.transform.position = transform.position;

        // 武器朝向
        missile.transform.right = transform.right;

        // 添加入子物体
        missile.transform.parent = transform.parent;

        // 添加关节
        SpringJoint2D joint = missile.gameObject.AddComponent<SpringJoint2D>();
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

    protected override void MeleeAttack()
    {

    }
}
