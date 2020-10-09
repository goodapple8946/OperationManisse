using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Controller;

public class CameraController : MonoBehaviour
{
    // 摄像机Z轴
    private static float cameraZ = -10f;

    // 摄像机坐标
    private Vector3 positionOrigin = new Vector3(0f, 0f, cameraZ);

    // 摄像机尺寸
    private float orthographicSizeOrigin = 3.2f;

    // 摄像机缩放速度
    private float zoomSpeedOrigin = 2f;
    private float zoomSpeed;

    // 摄像机滚动速度
    private float scrollSpeedOrigin = 12f;
    private float scrollSpeed;

    // 摄像机滚动触发边缘距离
    private int scrollDistance = 10;

    // 跟随
    [HideInInspector] public bool follow;

    void Awake()
    {
        Init();
    }

    void Update()
    {
        Zoom();
        Scroll();
        FixBound();
    }

    // 初始化
    void Init()
    {
        // 初始化位置
        transform.position = positionOrigin;
        // 初始化尺寸
        GetComponent<Camera>().orthographicSize = orthographicSizeOrigin;
        // 初始化缩放速度
        zoomSpeed = zoomSpeedOrigin;
        // 初始化滚动速度
        scrollSpeed = scrollSpeedOrigin;
    }

    // 缩放
    private void Zoom()
    {
        Camera camera = GetComponent<Camera>();

        // 鼠标不在UI上
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            float zoomSize = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            // 此时摄像机尺寸将 += -zoomSize
            if (zoomSize != 0)
            {
                // 摄像机最大尺寸
                float sizeMax = Mathf.Min((editorController.xMax - editorController.xMin) / 2, (editorController.yMax - editorController.yMin) / 2);
                // 摄像机尺寸不能过小（画面内图像过大，内容过少）
                if (-zoomSize < 0 && camera.orthographicSize - zoomSize > 0)
                {
                    camera.orthographicSize -= zoomSize;
                }
                // 摄像机尺寸不能过大（画面内图像过小，内容过多）
                else if (-zoomSize > 0 && camera.orthographicSize - zoomSize < sizeMax)
                {
                    camera.orthographicSize -= zoomSize;
                }
            }
        }
    }

    // 滚动
    private void Scroll()
    {
        Vector2 dir = GetMouseDirection();
        transform.Translate(scrollSpeed * Time.deltaTime * dir);
    }

    // 鼠标接近屏幕边缘时滚动,
    // 右(1,0)左(-1,0)上(0,1)下(0,-1)
    // 不存在滚动为(0,0)
    private Vector2 GetMouseDirection()
    {
        Vector2 dir = new Vector2(0, 0);
        // 向右
        if (Input.mousePosition.x > Screen.width - scrollDistance && transform.position.x <= editorController.xMax)
        {
            dir += new Vector2(1, 0);
        }
        // 向左
        else if (Input.mousePosition.x < scrollDistance && transform.position.x >= editorController.xMin)
        {
            dir += new Vector2(-1, 0);
        }

        // 向上
        if (Input.mousePosition.y > Screen.height - scrollDistance && transform.position.y <= editorController.yMax)
        {
            dir += new Vector2(0, 1);
        }
        // 向下
        else if (Input.mousePosition.y < scrollDistance && transform.position.y >= editorController.yMin)
        {
            dir += new Vector2(0, -1);
        }
        return dir;
    }

    // 变化Follow
    public void ToggleFollow()
    {
        follow = !follow;
    }

    // 将摄像头限制在游戏地图内
    private void FixBound()
    {
        // 屏幕中心不能超出放置区
        float left = editorController.xMin;
        float right = editorController.xMax;
        float bottom = editorController.yMin;
        float top = editorController.yMax;

        if (transform.position.x > right)
        {
            transform.position = new Vector3(right, transform.position.y, cameraZ);
        }
        else if (transform.position.x < left)
        {
            transform.position = new Vector3(left, transform.position.y, cameraZ);
        }
        if (transform.position.y > top)
        {
            transform.position = new Vector3(transform.position.x, top, cameraZ);
        }
        else if (transform.position.y < bottom)
        {
            transform.position = new Vector3(transform.position.x, bottom, cameraZ);
        }
    }
}