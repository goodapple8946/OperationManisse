using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core : Block
{
    private Vector2 originPosition = new Vector2(0f, 0.64f);

    protected override void Start()
    {
        base.Start();

        health = 200;
        price = 0;
    }

    // 将GameObject强制类型转换为Core
    public static explicit operator Core(GameObject gameObject)
    {
        return gameObject.GetComponent<Core>();
    }

    /// <summary>
    /// 初始化Core
    /// </summary>
    public void Init()
    {
        transform.position = originPosition;
    }
}
