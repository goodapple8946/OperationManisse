using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 originPosition = new Vector3(0f, 2.88f, -10);

    private Camera mainCamera;

    // 屏幕缩放速度
    public float zoomSpeed;

    // 屏幕尺寸范围
    public float zoomSizeMax;
    public float zoomSizeMin;

    // 屏幕滚动速度
    public float scrollSpeed;

    // 屏幕滚动触发边缘距离
    private int scrollDistance = 20;

    // 屏幕跟随
    public bool follow;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    void Update()
    {
        Zoom();
        Scroll();
    }

    // 跟随
    void Follow()
    {
        if (follow && GameController.gamePhase == GameController.GamePhase.Playing)
        {
            transform.position = GameObject.Find("Core").transform.position;
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
        if (Input.mousePosition.x > Screen.width - scrollDistance)
        {
            transform.Translate(scrollSpeed * Time.deltaTime, 0, 0);
        }
        // 向左
        else if (Input.mousePosition.x < scrollDistance && transform.position.x >= originPosition.x)
        {
            transform.Translate(-scrollSpeed * Time.deltaTime, 0, 0);
        }
        // 向上
        if (Input.mousePosition.y > Screen.height - scrollDistance)
        {
            transform.Translate(0, scrollSpeed * Time.deltaTime, 0);
        }
        // 向下
        else if (Input.mousePosition.y < scrollDistance && transform.position.y >= originPosition.y)
        {
            transform.Translate(0, -scrollSpeed * Time.deltaTime, 0);
        }
    }

    /// <summary>
    /// 初始化Camera
    /// </summary>
    public void Init()
    {
        transform.position = originPosition;
    }
}
