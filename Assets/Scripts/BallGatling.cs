using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGatling : Ball
{
	// 预热时间,以秒为单位
	public float preheatTime;

	// 当前已经预热的时间,介于[0, preheatTime]
	private float currPreheatTime;

	protected override void Update()
	{
		base.Update();
		Preheat();
	}

	// 加特林预热完毕才能远程攻击
	protected override void RangedAttack()
	{
		if(currPreheatTime >= preheatTime)
		{
			base.RangedAttack();
		}
	}

	// 存在敌人且未预热到最大值，加热
	// 不存在敌人且有预热，降温
	private void Preheat()
	{
		if (target != null && currPreheatTime <= preheatTime)
		{
			currPreheatTime += Time.deltaTime;
		}

		if (target == null && currPreheatTime >= 0)
		{
			currPreheatTime -= Time.deltaTime;
		}
	}
}
