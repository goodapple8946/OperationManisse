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
		if (gameController.gamePhase == GamePhase.Playing)
		{
			UpdatePreheat();
			UpdateGatlingColor();
			if (currPreheatTime >= preheatTime)
			{
				GatlingTrumble();
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

	// 添加武器晃动效果
	private void GatlingTrumble()
	{
		Transform gatlingTrans = transform.Find("Weapon");
		Vector2 bias = new Vector2(Random.Range(-0.015f, 0.015f), Random.Range(-0.015f, 0.015f));
		Vector2 restoreVec = new Vector2(-gatlingTrans.localPosition.x, -gatlingTrans.localPosition.y);
		gatlingTrans.Translate(restoreVec + bias);
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

    // 设置加特林颜色
    private void UpdateGatlingColor()
    {
        Color color = Color.Lerp(initColor, maxColor, currPreheatTime / preheatTime);
        Transform gatlingTrans = transform.Find("Weapon");
        gatlingTrans.GetComponent<SpriteRenderer>().color = color;
    }
}
