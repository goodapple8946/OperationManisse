using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSword : Ball
{
	// Start is called before the first frame update
	// 向前攻击的角度(弧度制)
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
		base.Update();
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

}
