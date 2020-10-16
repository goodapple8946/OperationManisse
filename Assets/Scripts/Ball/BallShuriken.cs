using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Controller;

public class BallShuriken : Ball
{
	// weaponCD 从握到手里剑减少
	private bool holdShuriken = true;

	protected override void FixedUpdate()
	{
		base.Update();
		if (gameController.GamePhase == GamePhase.Playing)
		{
			if (holdShuriken)
			{
				WeaponCoolDown();
			}

			// 寻找敌人
			Unit target = FindEnemyOptimized();

			// 已经有目标或索敌找到目标
			if (target != null)
			{
				// 转向目标
				RotateToward(target);
				if (Mathf.Abs(CalculateAngle(target)) <= weaponAngle 
					&& weaponCD <= 0 
					&& holdShuriken)
				{
					// 创建弹药
					MissileShuriken missile = CreateMissile(missilePrefab) as MissileShuriken;
					// 发射弹药
					missile.Launch(target);
					ReleaseShuriken();
				}
			}
		}
	}

	/// <summary>
	/// 武器冷却重置
	/// </summary>
	public void ReholdShuriken()
	{
		holdShuriken = true;
		transform.Find("Weapon").gameObject.SetActive(true);
		// 重置CD到最大值
		weaponCD = weaponCDMax;
	}

	private void ReleaseShuriken()
	{
		// 清空武器
		holdShuriken = false;
		transform.Find("Weapon").gameObject.SetActive(false);
	}
}
