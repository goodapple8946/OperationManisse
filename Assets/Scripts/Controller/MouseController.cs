using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    // 鼠标在世界中的位置：起点、终点、偏移量
    public static Vector2 startPositionWorld;
    public static Vector2 endPositionWorld;
    public static Vector2 offsetWorld;

    // 鼠标在屏幕中的位置：起点、终点、偏移量
    public static Vector2 startPositionScreen;
    public static Vector2 endPositionScreen;
    public static Vector2 offsetScreen;

    // 鼠标左键按住的时间
    public static float leftHoldTime = 0;

    void Update()
    {
        // 鼠标键按下
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            startPositionWorld = MouseWorldPosition();
            startPositionScreen = Input.mousePosition;
        }

        // 鼠标键按住
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            endPositionWorld = MouseWorldPosition();
            endPositionScreen = Input.mousePosition;

            offsetWorld = endPositionWorld - startPositionWorld;
            offsetScreen = endPositionScreen - startPositionScreen;

            startPositionWorld = endPositionWorld;
            startPositionScreen = endPositionScreen;

            leftHoldTime += Time.deltaTime;
        }
        else
        {
            leftHoldTime = 0;
        }
    }

    // 鼠标的世界位置
    public static Vector2 MouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
