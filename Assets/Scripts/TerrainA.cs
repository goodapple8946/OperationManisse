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
            AdjustPosition();
        }
    }
    private int width = 1;

    public int Height
    {
        get => height;
        set
        {
            height = value;
            UpdateSize(width, height);
            AdjustPosition();
        }
    }
    private int height = 1;

    private void UpdateSize(int width, int height)
    {
        Vector2 shape = new Vector2(width * GRID_SIZE, height * GRID_SIZE);
        GetComponent<SpriteRenderer>().size = shape;
        GetComponent<BoxCollider2D>().size = shape;
    }

    // 调整位置，使地形图片吸附镶嵌到格子中
    public void AdjustPosition()
    {
        Vector2 pos = GetLeftBottomPos();
        pos.x = Mathf.Round(pos.x / GRID_SIZE) * GRID_SIZE;
        pos.y = Mathf.Round(pos.y / GRID_SIZE) * GRID_SIZE;
        SetLeftBottomPos(pos);
    }

    // 获取这个地形图片左下角的点
    private Vector2 GetLeftBottomPos()
    {
        float x = transform.position.x - width * GRID_SIZE / 2;
        float y = transform.position.y - height * GRID_SIZE / 2;
        return new Vector2(x, y);
    }

    // 设置获取这个地形图片左下角的点，从而设置地形位置
    private void SetLeftBottomPos(Vector2 pos)
    {
        float x = pos.x + width * GRID_SIZE / 2;
        float y = pos.y + height * GRID_SIZE / 2;
        transform.position = new Vector2(x, y);
    }
}
