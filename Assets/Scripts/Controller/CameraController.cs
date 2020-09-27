using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    private EditorController editorController;

    void Awake()
    {
        editorController = GameObject.Find("Editor Controller").GetComponent<EditorController>();

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
            if (zoomSize != 0)
            {
                // 摄像机尺寸不能为负
                if (camera.orthographicSize - zoomSize > 0)
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
        if (transform.position.x > editorController.xMax)
        {
            transform.position = new Vector3(editorController.xMax, transform.position.y, cameraZ);
        }
        else if (transform.position.x < editorController.xMin)
        {
            transform.position = new Vector3(editorController.xMin, transform.position.y, cameraZ);
        }
        if (transform.position.y > editorController.yMax)
        {
            transform.position = new Vector3(transform.position.x, editorController.yMax, cameraZ);
        }
        else if (transform.position.y < editorController.yMin)
        {
            transform.position = new Vector3(transform.position.x, editorController.yMin, cameraZ);
        }
    }
}