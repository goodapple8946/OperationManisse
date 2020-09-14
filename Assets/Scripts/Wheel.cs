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
        if (isAlive && !isSelling && gameController.gamePhase == GameController.GamePhase.Playing)
        {
            // 轮子下方有物体，未超过最大速度
            if (IsGrounded(out block) && body.velocity.x <= speedMax)
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

    protected override void UpdateCover()
    {
        base.UpdateCover();

        // 如果有遮罩预设
        if (coverPrefab != null)
        {
            // 删除当前遮罩
            if (transform.childCount > 0)
            {
                GameObject cover = transform.GetChild(0).gameObject;
                Destroy(cover);
            }

            // 遍历四个方向
            for (int i = 0; i < 4; i++)
            {
                // 如果有连接的Block，添加遮罩
                if (blocksLinked[i] != null)
                {
                    // 创建，旋转遮罩
                    GameObject cover = Instantiate(coverPrefab);
                    cover.name = coverPrefab.name;
                    cover.transform.position = transform.position;
                    cover.transform.Rotate(0, 0, i * 90);
                    cover.transform.parent = transform;
                    break;
                }
            }
        }
    }
}
