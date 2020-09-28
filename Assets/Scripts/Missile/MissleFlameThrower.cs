using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleFlameThrower : Missile
{

	/// <summary>
	/// 由于Trigger会与右方碰撞，要检测是否为右方
	/// </summary>
	private void OnTriggerEnter2D(Collider2D other)
	{
		Unit unit = other.gameObject.GetComponent<Unit>();
		if (unit != null && unit.player != this.player)
		{
			unit.TakeDamage(this.damage);
		}
	}
}
