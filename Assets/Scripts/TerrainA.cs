using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TerrainA : ClickableObject
{
    private static readonly float GRID_SIZE = 0.6f;

    public int Width
    {
        get => width;
        set
        {
            width = value;
            UpdateSize(width, height);
        }
    }
    private int width;

    public int Height
    {
        get => height;
        set
        {
            height = value;
            UpdateSize(width, height);
        }
    }
    private int height;

    private void UpdateSize(int width, int height)
    {
        Vector2 shape = new Vector2(width * GRID_SIZE, height * GRID_SIZE);
        GetComponent<SpriteRenderer>().size = shape;
        GetComponent<BoxCollider2D>().size = shape;
    }
}
