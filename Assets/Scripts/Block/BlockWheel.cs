using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockWheel : Block
{
    public override bool IsLinkAvailable(int direction)
    { 
        return direction == this.direction;
    }
}
