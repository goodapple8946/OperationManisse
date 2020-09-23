using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGatling : Ball
{
	//// 预热时间,以秒为单位
	//public float preheatTime;

	//// 当前已经预热的时间,介于[0, preheatTime]
	//private float currPreheatTime;

	//private Color initColor = Color.black;

	//// 机枪发红色
	//private Color hotColor = new Color(0.91f, 0.10f, 0.05f, 1.0f);
	//private Color maxColor = Color.white;

	//protected override void Update()
	//{
	//	base.Update();
	//	//Preheat();
	//	HeatGatling();
	//}

	//// 加特林预热完毕才能远程攻击
	//protected override void RangedAttack()
	//{
	//	if(currPreheatTime >= preheatTime)
	//	{
	//		base.RangedAttack();
	//	}
	//}

 //   // 存在敌人且未预热到最大值，加热
 //   // 不存在敌人且有预热，降温
 //   private void Preheat()
 //   {
 //       if (target != null && currPreheatTime <= preheatTime)
 //       {
 //           currPreheatTime += Time.deltaTime;
 //       }

 //       if (target == null && currPreheatTime >= 0)
 //       {
 //           currPreheatTime -= Time.deltaTime;
 //       }
 //   }

 //   // 设置加特林颜色
 //   private void HeatGatling()
	//{
	//	Color color = Color.Lerp(initColor, maxColor, currPreheatTime / preheatTime);
	//	Transform gatlingTrans = transform.GetChild(0).transform;
	//	gatlingTrans.GetComponent<SpriteRenderer>().color = color;
	//}
}
