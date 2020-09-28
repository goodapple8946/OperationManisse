using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Controller;

public class BallShuriken : Ball
{
	private bool holdShuriken = true;

	protected override void FixedUpdate()
	{
		DeathCheck();
		// 只改了这一处，当握着手里剑时才会减CD
		if (holdShuriken)
		{
			WeaponCoolDown();
		}
	
		if (IsAlive() && gameController.gamePhase == GamePhase.Playing)
		{
			// 寻找敌人
			Unit target = FindEnemy();

			// 已经有目标或索敌找到目标
			if (target != null)
			{
				// 转向目标
				bool alreadyAimed = AimTo(target);
				if (alreadyAimed)
				{
					// 检查CD，若CD允许，则攻击
					CheckCDAndAttack(target);
				}
			}
		}
		// 更新朝向，保证武器图像顶部朝上
		UpdateToward();
	}

	protected override Missile CreateMissile()
	{
		Missile missile = base.CreateMissile(); 

		MissileShuriken missileShuriken = (MissileShuriken)missile;
		missileShuriken.originBall = this;
		ReleaseShuriken();
		missileShuriken.Attack(FindEnemy());

		return missileShuriken;
	}

	/// <summary>
	/// 武器冷却重置
	/// </summary>
	public void ReholdShuriken()
	{
		holdShuriken = true;
		transform.GetChild(1).gameObject.SetActive(true);
		weaponCD = weaponCDMax * Random.Range(1f, 1.1f);
	}

	
	private void ReleaseShuriken()
	{
		// 清空武器
		holdShuriken = false;
		transform.GetChild(1).gameObject.SetActive(false);
	}
}
