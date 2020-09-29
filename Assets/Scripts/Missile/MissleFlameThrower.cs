using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleFlamethrower : Missile
{
	private void OnTriggerEnter2D(Collider2D other)
	{
		Unit unit = other.gameObject.GetComponent<Unit>();
		if (unit != null && unit.player != this.player)
		{
			unit.TakeDamage(this.damage);
		}
	}
}
