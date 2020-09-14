using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
	// 武器伤害
	public int damage;

	// 撞击
	protected virtual void OnCollisionEnter2D(Collision2D other)
	{
		// 击中Ball
		Ball ball = other.gameObject.GetComponent<Ball>();
		if (ball != null)
		{
			ball.health -= damage;
			//ball.body.AddForce(transform.right * forceHit);
		}

		// 击中Block
		Block block = other.gameObject.GetComponent<Block>();
		if (block != null)
		{
			block.health -= damage;
			//block.body.AddForce(transform.right * forceHit);
		}
	}
}
