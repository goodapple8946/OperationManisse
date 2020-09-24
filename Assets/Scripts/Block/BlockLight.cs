using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockLight : Block
{
    // 粒子预设
    public GameObject particlePrefab;

    // 粒子
    protected GameObject particle;

    protected override void Start()
    {
        base.Start();

        // 创建粒子
        if (particle == null)
        {
            particle = Instantiate(particlePrefab);
            particle.transform.position = transform.position;
            particle.transform.parent = transform;
        }
    }

    protected override void Die()
    {
        base.Die();

        // 删除粒子
        Destroy(particle);
    }
}
