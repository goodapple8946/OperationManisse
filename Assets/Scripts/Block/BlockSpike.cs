using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpike : Block
{
    public override bool IsLinkAvailable(int direction)
    {
        // 只能连接反方向
        return direction == (this.direction + 2) % 4;
    }
}
