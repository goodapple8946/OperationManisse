using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : Block
{
    // Wheel的半径
    private float radius = 0.32f;

    // 力
    public float force;

    // 最大速度（未达最大速度时可以提供力）
    public float speedMax;

    // 地面检测射线起始点在Wheel底部的向下偏移
    protected float groundCheckOffset = 0.01f;

    // 地面检测射线长度
    protected float groundCheckDistance = 0.01f;

    protected override void Start()
    {
        base.Start();

        absorptionProvision = false;
        isWheel = true;
    }

    protected override void Update()
    {
        base.Update();

        Run();
    }

    protected void Run()
    {
        if (alive && tag != "Goods" &&
            GameController.gamePhase == GameController.GamePhase.Playing &&
            IsGrounded() &&
            entity.body != null &&
            entity.body.velocity.magnitude <= speedMax)
        {
            entity.body.AddForceAtPosition(new Vector2(force, 0), transform.position);
        }
    }

    protected bool IsGrounded()
    {
        // 射线起始点
        Vector2 origin = (Vector2)transform.position + Vector2.down * (radius + groundCheckOffset);

        // 向下发射射线
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance);
        
        // 如果射线触碰到物体，并且不是自己的Entity
        if (hit.transform != null && hit.transform != entity.transform)
        {
            return true;
        }
        return false;
    }
}
