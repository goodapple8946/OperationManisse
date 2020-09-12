using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    private GameController gameController;
    private static Camera mainCamera;

    // 鼠标起点、终点、偏移量
    public static Vector2 startPosition;
    public static Vector2 endPosition;
    public static Vector2 offset;

    void Start()
    {
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void Update()
    {
        // 鼠标左键按下
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = MouseWorldPosition();
        }

        // 鼠标左键按住
        if (Input.GetMouseButton(0))
        {
            endPosition = MouseWorldPosition();
            offset = endPosition - startPosition;
            startPosition = endPosition;
        }

        // 鼠标左键抬起
        if (Input.GetMouseButtonUp(0))
        {

        }
    }

    // 鼠标的世界位置
    public static Vector2 MouseWorldPosition()
    {
        return mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }
}
