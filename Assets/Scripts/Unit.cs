using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // 生命值
    protected int health;

    // 是否存活
    public bool alive;

    // 死亡持续时间
    protected float deathDuration;

    protected Rigidbody2D body;

    protected virtual void Start()
    {
        body = GetComponent<Rigidbody2D>();
        
        alive = true;
        deathDuration = 3f;
    }

    protected virtual void Update()
    {
        DeathCheck();
    }

    // 死亡检测
    protected void DeathCheck()
    {
        if (alive)
        {
            // 生命值为0或以下
            if (health <= 0)
            {
                StartCoroutine(Die());
            }
        }
    }

    // 死亡
    protected virtual IEnumerator Die()
    {
        // 开始死亡效果
        alive = false;
        Destroy(gameObject.GetComponent<Collider2D>());

        // 等待死亡持续时间
        yield return new WaitForSeconds(deathDuration);

        // 摧毁物体
        Destroy(gameObject);
    }
}
