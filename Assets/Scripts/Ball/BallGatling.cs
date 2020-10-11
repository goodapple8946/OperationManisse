using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Controller;

public class BallGatling : Ball
{
    // 预热时间,以秒为单位
    public float preheatTime;

    // 当前已经预热的时间,介于[0, preheatTime]
    public float currPreheatTime = 0.0f;

    private Color initColor = Color.black;
    // 机枪发红色
    private Color maxColor = Color.white;

	protected override void FixedUpdate()
	{
		base.Update();
		// 开始游戏后
		if (gameController.GamePhase == GamePhase.Playing)
		{
			UpdatePreheat();
			UpdateGatlingColor();

			WeaponCoolDown();
			// 寻找敌人
			Unit target = FindEnemy();
			// 已经有目标或索敌找到目标
			if (target != null)
			{
				// 转向目标
				RotateToward(target);
				if (CalculateAngle(target) <= weaponAngle && weaponCD <= 0)
				{
					// 武器冷却重置
					bool preheatOver = (currPreheatTime >= preheatTime);
					if (preheatOver)
					{
						weaponCD = weaponCDMax;
						RangedAttack();
						GatlingTrumble();
					}
				}
			}
		}
	}

    // 加特林预热完毕才能远程攻击
    protected override void RangedAttack()
    {
        if (currPreheatTime >= preheatTime)
        {
            base.RangedAttack();		
		}
    }

	// 存在敌人且未预热到最大值，加热
	// 不存在敌人且有预热，降温
	private void UpdatePreheat()
	{
		Unit target = FindEnemy();
		if (target != null && currPreheatTime <= preheatTime)
		{
			currPreheatTime += Time.deltaTime;
		}

		if (target == null && currPreheatTime >= 0)
		{
			currPreheatTime -= Time.deltaTime;
		}
	}

	// 加特林抖动一下
	private void GatlingTrumble()
	{
		Transform gatlingTrans = transform.Find("Weapon");
		Vector2 bias = new Vector2(Random.Range(-0.015f, 0.015f), Random.Range(-0.015f, 0.015f));
		Vector2 restoreVec = new Vector2(-gatlingTrans.localPosition.x, -gatlingTrans.localPosition.y);
		gatlingTrans.Translate(restoreVec + bias);
	}

    // 设置加特林颜色
    private void UpdateGatlingColor()
    {
        Color color = Color.Lerp(initColor, maxColor, currPreheatTime / preheatTime);
        Transform gatlingTrans = transform.Find("Weapon");
        gatlingTrans.GetComponent<SpriteRenderer>().color = color;
    }
}
