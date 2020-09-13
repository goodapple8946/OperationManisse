using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // 摄像机Z轴
    private static float cameraZ = -10f;

    private Vector3 originPosition = new Vector3(0f, 2.88f, cameraZ);

    public Camera mainCamera;

    // 屏幕缩放速度
    public float zoomSpeed;

    // 屏幕尺寸范围
    public float zoomSizeMax;
    public float zoomSizeMin;

    // 屏幕滚动速度
    public float scrollSpeed;

    // 屏幕滚动触发边缘距离
    private int scrollDistance = 10;

    // 屏幕跟随
    public bool follow;

    // 屏幕边界
    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;

    private GameController gameController;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

    void Update()
    {
        Zoom();
        Scroll();
        Follow();
    }

    // 跟随
    void Follow()
    {
        if (follow)
        {
            GameObject core = GameObject.Find("Player Objects/Player Core");

            if (core != null && ((Block)core).isAlive)
            {
                // 跟随
                transform.position = core.transform.position + new Vector3(0, 0, cameraZ);

                FixBound();
            }
            else
            {
                follow = false;
            }
        }
    }

    // 缩放
    void Zoom()
    {
        float zoomSize = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        if (zoomSize != 0)
        {
            if (mainCamera.orthographicSize - zoomSize >= zoomSizeMin - 0.05f && mainCamera.orthographicSize - zoomSize <= zoomSizeMax + 0.05f)
            {
                mainCamera.orthographicSize -= zoomSize;
            }
        }
    }

    // 滚动
    void Scroll()
    {
        // 向右
        if (Input.mousePosition.x > Screen.width - scrollDistance && transform.position.x <= xMax)
        {
            transform.Translate(scrollSpeed * Time.deltaTime, 0, 0);
            follow = false;
        }
        // 向左
        else if (Input.mousePosition.x < scrollDistance && transform.position.x >= xMin)
        {
            transform.Translate(-scrollSpeed * Time.deltaTime, 0, 0);
            follow = false;
        }
        // 向上
        if (Input.mousePosition.y > Screen.height - scrollDistance && transform.position.y <= yMax)
        {
            transform.Translate(0, scrollSpeed * Time.deltaTime, 0);
            follow = false;
        }
        // 向下
        else if (Input.mousePosition.y < scrollDistance && transform.position.y >= yMin)
        {
            transform.Translate(0, -scrollSpeed * Time.deltaTime, 0);
            follow = false;
        }
    }

    // 边界修正
    void FixBound()
    {
        if (transform.position.x > xMax)
        {
            transform.position = new Vector3(xMax, transform.position.y, cameraZ);
        }
        else if (transform.position.x < xMin)
        {
            transform.position = new Vector3(xMin, transform.position.y, cameraZ);
        }
        if (transform.position.y > yMax)
        {
            transform.position = new Vector3(transform.position.x, yMax, cameraZ);
        }
        else if (transform.position.y < yMin)
        {
            transform.position = new Vector3(transform.position.x, yMin, cameraZ);
        }
    }

    /// <summary>
    /// 初始化Camera
    /// </summary>
    public void Init()
    {
        transform.position = originPosition;
        follow = false;
    }
}
