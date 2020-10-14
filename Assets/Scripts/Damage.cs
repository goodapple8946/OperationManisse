using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Damage
{
	// 伤害量
	public int Amount { get; }

	// 攻击者的class类型
	public Type DamageType { get; }

	public Vector2 Force { get; }

	public Damage(int amount, Type damageType, Vector2 force)
	{
		Debug.Assert(amount >= 0);
		Debug.Assert(damageType != null);

		this.Amount = amount;
		this.DamageType = damageType;
		this.Force = force;
	}
}
