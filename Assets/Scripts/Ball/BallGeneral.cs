using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Controller;

public class BallGeneral : Ball
{
	protected override void FixedUpdate()
	{
		base.Update();

		if (gameController.GamePhase == GamePhase.Playing)
		{
			WeaponCoolDown();

			// 寻找敌人
			Unit target = FindEnemy();

			// 已经有目标或索敌找到目标
			if (target != null)
			{
				// 转向目标
				RotateToward(target);
				if (Mathf.Abs(CalculateAngle(target)) <= weaponAngle && weaponCD <= 0)
				{
					// 武器冷却重置
					weaponCD = weaponCDMax * Random.Range(1f, 1.1f);
					RangedAttack();
				}
			}
		}
	}
}
