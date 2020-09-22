using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : Unit
{
    /* Link Direction:
     * 0: Right, 1: Top, 2: Left, 3: Bottom
     */

    protected int direction = 0;

    public Block[] blocksLinked = new Block[4];

    public Joint2D[] joints = new Joint2D[4];

    public virtual void LinkTo(Block another, int direction, Joint2D joint)
    {
        blocksLinked[direction] = another;
        joints[direction] = joint;
    }

    protected void Rotate()
    {
        direction = (direction + 1) % 4;
    }

    public virtual bool IsLinkAvailable(int direction)
    {
        return true;
    }
}