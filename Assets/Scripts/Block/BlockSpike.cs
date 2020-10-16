using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Controller;

public class BlockSpike : Block
{
    protected override void Awake()
    {
        base.Awake();

        velocityCollision = 0f;
    }

    public override bool IsLinkAvailable(int direction)
    {
        // 只能连接反方向
        return direction == Negative(this.direction)
			&& joints[direction] == null;
    }
}
