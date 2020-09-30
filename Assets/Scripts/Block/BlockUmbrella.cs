using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockUmbrella : Block
{
    // 力的大小
    public float force;

    // 转向正上方的力矩
    protected float forceTorque = 1f;

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

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (IsAlive())
        {
            // 向上施加力
            body.AddForce(Vector2.up * force);

            // 朝向上方
            if (transform.eulerAngles.z > 270f || transform.eulerAngles.z < 88f)
            {
                body.AddTorque(forceTorque);
            }
            else if (transform.eulerAngles.z > 92f && transform.eulerAngles.z <= 270f)
            {
                body.AddTorque(-forceTorque);
            }
        }
    }

    protected override void OnDestroy()
    {

		base.OnDestroy();
		// 删除粒子
		Destroy(particle);
    }

    public override bool IsLinkAvailable(int direction)
    {
        return direction == (this.direction + 2) % 4;
    }
}
