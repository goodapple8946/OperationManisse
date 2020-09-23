using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPropeller : Block
{
    // 力的大小
    public float force;

    // 提供力时的最大速度
    public float speedMax;

    // 粒子预设
    public GameObject particlePrefab;

    public override void GameStart()
    {
        base.GameStart();

        // 创建粒子
        GameObject particle = Instantiate(particlePrefab);
        particle.transform.position = transform.position;
        particle.transform.parent = transform;
        particle.transform.Rotate(0, 0, GetAngleByDirection(direction));
    }

    protected override void Update()
    {
        base.Update();

        // 向朝向的方向施加力
        if (body.velocity.magnitude <= speedMax)
        {
            Vector2 forceAdded = Quaternion.AngleAxis(transform.localEulerAngles.z, Vector3.forward) * Vector2.right * force;
            body.AddForce(forceAdded);
        }
    }
}
