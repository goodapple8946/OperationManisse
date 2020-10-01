using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileFlamethrower : Missile
{

	private void Update()
	{
		transform.eulerAngles = unit.transform.eulerAngles;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		Unit unit = other.gameObject.GetComponent<Unit>();
		if (unit != null && unit.player != player)
		{
			unit.TakeDamage(damage);
		}
	}
}
