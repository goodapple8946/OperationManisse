using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BallSword : Ball
{
	// Start is called before the first frame update
	// 向前攻击的角度(角度制)
	public float maxForwardAngle;
	public float attackSpead;
	public float liftSwordSpeed;

	// 是否向前
	private bool forward = true;
	// 相对于初始位置偏移角度
	private float currentAngle = 0.0f;

	// Update is called once per frame
	protected override void Update()
	{
		//base.Update();
		DeathCheck();
		if (isAlive && !isSelling && gameController.gamePhase == GameController.GamePhase.Playing)
		{
			RotateSword();
		}
	}

	private void RotateSword()
	{
		Transform swordTrans = transform.GetChild(0).gameObject.transform;
		Vector3 point = transform.position;
		// forward的为顺时针
		if (currentAngle >= maxForwardAngle)
		{
			forward = false;
		}
		else if (currentAngle <= 0.0f)
		{
			forward = true;
		}
		float factor = forward ? attackSpead : -liftSwordSpeed;
		float bias = factor * Time.deltaTime;
		currentAngle += bias;
		swordTrans.RotateAround(point, Vector3.back, bias);
	}

	protected new void DeathCheck()
	{
		if (health <= 0)
		{
			isAlive = false;
			// 剑与球分离，自由降落
			GameObject childObject = transform.GetChild(0).gameObject;
			childObject.GetComponent<Collider2D>().enabled = false;

			// 剑添加关节到ball
			FixedJoint2D joint = childObject.AddComponent<FixedJoint2D>();
			joint.connectedBody = body;
			joint.enableCollision = false;

			// 球旋转自由降落
			gameObject.GetComponent<Collider2D>().enabled = false;
			body.AddTorque(torqueDeath);
			Destroy(gameObject, deathDuration);
		}
	}

}
