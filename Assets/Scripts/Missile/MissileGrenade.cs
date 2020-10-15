using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileGrenade : Missile
{
    // 爆炸半径
    public float radiusExplode;

    // 撞击
    protected override void OnCollisionEnter2D(Collision2D other)
    {
        base.OnCollisionEnter2D(other);

        Explode();
    }

    // 爆炸
    void Explode()
    {
        ArrayList gameObjects = new ArrayList();
        gameObjects.AddRange(GameObject.FindGameObjectsWithTag("Ball"));
        gameObjects.AddRange(GameObject.FindGameObjectsWithTag("Block"));

        foreach (GameObject gameObject in gameObjects)
        {
            Unit unit = gameObject.GetComponent<Unit>();

            if (player != unit.player)
            {
                // 爆炸中心与该Unit的向量
                Vector2 vector = unit.transform.position - transform.position;

                // 爆炸中心与该Unit的距离
                float distance = vector.magnitude;

                // 在爆炸范围内
                if (distance < radiusExplode)
                {
                    // 伤害系数
                    float damageScale = 1f - distance / radiusExplode;

					unit.TakeDamage(CreateDamage());

                    // 造成冲击力
                    unit.body.AddForce(vector.normalized * forceHit * damageScale);
                }
            }
        }
    }
}
