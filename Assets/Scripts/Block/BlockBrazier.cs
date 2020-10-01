using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class BlockBrazier : Block
{
    // 粒子预设
    public GameObject particlePrefab;

    // 粒子
    protected GameObject particle;

    // 照亮范围
    protected float[] outerRaiuds = { 7.2f, 8.8f };

    protected override void Start()
    {
        base.Start();
		
        // 创建粒子
        particle = Instantiate(particlePrefab);
    }

    protected override void Update()
    {
        base.Update();

        // 粒子跟随
        particle.transform.position = transform.position;

        // 随机照亮范围
        transform.GetChild(1).GetComponent<Light2D>().pointLightOuterRadius = Random.Range(outerRaiuds[0], outerRaiuds[1]);

    }

    protected override void OnDestroy()
    {
		base.OnDestroy();
		// 删除粒子
		Destroy(particle);
    }

    public override bool IsLinkAvailable(int direction)
    {
        // 只能连接反方向
        return direction == GetDirectionNegative(this.direction)
			&& joints[direction] == null;
    }

	// 不能旋转,只能朝上
    public override void Rotate()
    {

	}
}
