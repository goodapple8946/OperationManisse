using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockWheel : Block
{
	protected override void Start()
	{
		base.Start();
		// 设置方向
		Direction = 1;
	}

	public override bool IsLinkAvailable(int direction)
    { 
        return direction == this.direction;
    }
}
