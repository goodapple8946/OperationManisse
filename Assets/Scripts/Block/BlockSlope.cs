using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSlope : Block
{
    public override bool IsLinkAvailable(int direction)
    {
        // 只能连接当前和顺时针一个方向
        return direction == this.direction || direction == (this.direction + 3) % 4;
    }
}
