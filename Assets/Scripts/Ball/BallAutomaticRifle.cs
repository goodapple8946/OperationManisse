using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAutomaticRifle : BallGeneral
{
    // 三连发间隔
    private float timeOfShot = 0.075f;

    // 三连发计数
    private int countOfShot = 0;

    // 远程攻击
    protected override void RangedAttack()
    {
        countOfShot = 3;

        StartCoroutine(TripleShoot());
    }

    IEnumerator TripleShoot()
    {
        while (countOfShot-- > 0)
        {
            // 创建弹药
            Missile missile = CreateMissile(missilePrefab);

            // 发射弹药
            missile.Launch();

            yield return new WaitForSeconds(timeOfShot);
        }
    }
}
