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

	// 是否向前
	private bool forward = true;
	// 相对于初始位置偏移角度
	private float currentAngle = 0.0f;

	//protected override void Start()
	//{
	//	base.Start();
	//}

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
		float bias = attackSpead * Time.deltaTime;
		bias = forward ? bias : -bias;
		currentAngle += bias;
		swordTrans.RotateAround(point, Vector3.back, bias);
	}

	protected new void DeathCheck()
	{
		if (isAlive)
		{
			if (health <= 0)
			{
				StartCoroutine(Die());
			}
		}
	}

	protected override IEnumerator Die()
	{
		isAlive = false;
		// 剑与球分离，自由降落
		GameObject childGameObject = transform.GetChild(0).gameObject;
		Destroy(childGameObject.GetComponent<Collider2D>());
		childGameObject.transform.parent = null;

		// 球旋转自由降落
		Destroy(gameObject.GetComponent<Collider2D>());
		body.AddTorque(torqueDeath);

		// 等待死亡持续时间
		yield return new WaitForSeconds(3);
		Destroy(childGameObject);
		Destroy(gameObject);
	}

}
