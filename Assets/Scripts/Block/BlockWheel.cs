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
        return this.direction == direction;
	}
}
