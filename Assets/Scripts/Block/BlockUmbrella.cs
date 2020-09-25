using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockUmbrella : Block
{
    // 力的大小
    public float force;

    // 提供力时的最大速度
    public float speedMax;

    // 粒子预设
    public GameObject particlePrefab;

    // 粒子
    protected GameObject particle;

    public override void GameStart()
    {
        base.GameStart();

        // 创建粒子
        particle = Instantiate(particlePrefab);
        particle.transform.position = transform.position;
        particle.transform.parent = transform;
        particle.transform.Rotate(0, 0, GetAngleByDirection(direction));
    }

    protected override void Update()
    {
        base.Update();

        // 向上施加力
        if (IsAlive() && body.velocity.magnitude <= speedMax)
        {
            Vector2 forceAdded = Vector2.up * force;
            body.AddForce(forceAdded);
        }
    }

    protected override void Die()
    {
        // 删除粒子
        Destroy(particle);

        base.Die();
    }

    public override bool IsLinkAvailable(int direction)
    {
        return direction == (this.direction + 2) % 4;
    }
}
