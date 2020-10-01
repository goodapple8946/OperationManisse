using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockWheel : Block
{
	protected override void Start()
	{
		base.Start();
	}

	public override bool IsLinkAvailable(int direction)
    { 
		// 正向链接
        return direction == this.direction && joints[direction] == null;
	}
}
