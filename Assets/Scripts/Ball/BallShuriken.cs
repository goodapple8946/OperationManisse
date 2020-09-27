using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShuriken : Ball
{
	// 攻击
	protected new void CheckCDAndAttack(Unit target)
	{
		if (weaponCD <= 0)
		{
			transform.GetComponent<MissileShuriken>().Attack(target);
		}
	}

	/// <summary>
	/// 武器冷却重置
	/// </summary>
	public void ResetCD()
	{
		weaponCD = weaponCDMax * Random.Range(1f, 1.1f);
	}
}
