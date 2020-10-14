using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Controller;

public class BallGatling : Ball
{ 
    // 预热时间,以秒为单位
    public float heatTime;

    // 当前已经预热的时间,介于[0, heatTime]
    public float currHeatTime = 0.0f;

    private Color initColor = Color.black;

    // 机枪发红色
    private Color maxColor = Color.white;

	private bool isOverHeat = false;

	protected override void FixedUpdate()
	{
		base.Update();
		// 开始游戏后
		if (gameController.GamePhase == GamePhase.Playing)
		{

			UpdateGatlingColor();

			// 检查状态
			if (currHeatTime >= heatTime)
			{
				isOverHeat = true;
				currHeatTime -= Time.deltaTime;
			}
			else if (currHeatTime <= 0)
			{
				isOverHeat = false;
			}
			// 寻找敌人
			Unit target = FindEnemy();
			// 已经有目标或索敌找到目标
			if (target != null)
			{
				// 转向目标
				RotateToward(target);
				// 瞄准了目标并且有CD
				if (CalculateAngle(target) <= weaponAngle 
					&& weaponCD <= 0
					&& !isOverHeat)
				{
					currHeatTime += Time.deltaTime;
					RangedAttack();
					GatlingTrumble();
					// 武器冷却重置
					weaponCD = weaponCDMax;
				}
				else
				{
					WeaponCoolDown();
				}
			}
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
        Color color = Color.Lerp(initColor, maxColor, currHeatTime / heatTime);
        Transform gatlingTrans = transform.Find("Weapon");
        gatlingTrans.GetComponent<SpriteRenderer>().color = color;
    }
}
