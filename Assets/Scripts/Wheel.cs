using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : Block
{
    // 力
    public float force;

    // 最大速度（未达最大速度时可以提供力）
    public float speedMax;

    protected override void Update()
    {
        base.Update();

        Run();
    }

    protected void Run()
    {
        Block block;
        if (isAlive && !isSelling && GameController.gamePhase == GameController.GamePhase.Playing && IsGrounded(out block))
        {
            // 轮子受力
            body.AddForce(new Vector2(force, 0));

            // 轮子下方Block受力
            if (block != null)
            {
                block.body.AddForce(new Vector2(-force, 0));
            }
        }
    }
}
