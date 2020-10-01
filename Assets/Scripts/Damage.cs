using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Damage
{
	public int Amount { get; }

	public Type DamageType { get; }

	public Damage(int amount, Type damageType)
	{
		Debug.Assert(amount >= 0);
		Debug.Assert(damageType != null);

		this.Amount = amount;
		this.DamageType = damageType;
	}
}
