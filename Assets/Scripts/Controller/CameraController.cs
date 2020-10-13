using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Controller;

public class CameraController : MonoBehaviour
{
    // 摄像机Z轴
    private static readonly float CAMERA_Z = -10f;

    // 摄像机坐标
    private static readonly Vector3 POSITION_ORIGIN = new Vector3(0f, 0f, CAMERA_Z);

    // 摄像机缩放速度
    private static readonly float ZOOM_SPEED_ORIGIN = 2f;
    private float zoomSpeed;

    // 摄像机滚动速度
    private static readonly float SCROLL_SPEED_ORIGIN = 12f;
    private float scrollSpeed;

    // 摄像机滚动触发边缘距离
    private static readonly int SCROLL_DISTANCE = 10;

	// 相机左下角
	public Vector2 LeftBottomPoint { set; get; }
	// 相机右上角
	public Vector2 RightTopPoint { set; get; }

    public void SetView(float left, float bottom, float right, float top)
    {
        SetView(new Vector2(left, bottom), new Vector2(right, top));
    }

	public void SetView(Vector2 lbPoint, Vector2 rtPoint)
	{
		Debug.Assert(lbPoint.x <= rtPoint.x && lbPoint.y <= rtPoint.y);
        LeftBottomPoint = lbPoint;
		RightTopPoint = rtPoint;
	}

	// 跟随
	[HideInInspector] public bool follow;

    void Awake()
    {
        Init();
    }

    void Update()
    {
        // 缩放
        Zoom();
        // 修正缩放
        FixZoom();

        if (!follow)
        {
            // 在Editor阶段不能滚动屏幕
            if (gameController.GamePhase != GamePhase.Editor)
            {
                // 滚动
                Scroll();
            }

            // 鼠标右键按下
            if (Input.GetMouseButton(1))
            {
                // 拖动
                Drag();
            }

            // 修正滚动和拖动
            FixBound();
        }
    }

    // 初始化
    void Init()
    {
        // 初始化位置
        transform.position = POSITION_ORIGIN;
        // 初始化缩放速度
        zoomSpeed = ZOOM_SPEED_ORIGIN;
        // 初始化滚动速度
        scrollSpeed = SCROLL_SPEED_ORIGIN;
    }

    // 缩放
    private void Zoom()
    {
        // 鼠标不能在UI上
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        float zoomSize = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        // 此时摄像机尺寸将 += -zoomSize
        if (zoomSize != 0)
        {
            GetComponent<Camera>().orthographicSize -= zoomSize;
        }
    }

    // 修正摄像机缩放
    private void FixZoom()
    {
        Camera camera = GetComponent<Camera>();
        float sizeMin = 0.1f;
        float sizeMax =
            Mathf.Min((RightTopPoint.x - LeftBottomPoint.x) / 2, (RightTopPoint.y - LeftBottomPoint.y) / 2)
            + (gameController.GamePhase == GamePhase.Editor ? 3.2f : 0);

        if (camera.orthographicSize < sizeMin)
        {
            camera.orthographicSize = sizeMin;
        }
        else if (camera.orthographicSize > sizeMax)
        {
            camera.orthographicSize = sizeMax;
        }
    }

    // 拖动
    private void Drag()
    {
        // 鼠标不能在UI上
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        // 至少按住左键holdTime秒，才可以开始拖动
        float holdTime = 0.08f;
        // 拖动速度
        float speed = GetComponent<Camera>().orthographicSize * 0.0025f;

        if (MouseController.leftHoldTime >= holdTime)
        {
            transform.position -= (Vector3)MouseController.offsetScreen * speed;
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
        if (Input.mousePosition.x > Screen.width - SCROLL_DISTANCE && transform.position.x <= RightTopPoint.x)
        {
            dir += new Vector2(1, 0);
        }
        // 向左
        else if (Input.mousePosition.x < SCROLL_DISTANCE && transform.position.x >= LeftBottomPoint.x)
        {
            dir += new Vector2(-1, 0);
        }

        // 向上
        if (Input.mousePosition.y > Screen.height - SCROLL_DISTANCE && transform.position.y <= RightTopPoint.y)
        {
            dir += new Vector2(0, 1);
        }
        // 向下
        else if (Input.mousePosition.y < SCROLL_DISTANCE && transform.position.y >= LeftBottomPoint.y)
        {
            dir += new Vector2(0, -1);
        }
        return dir;
    }

    // 将摄像头限制在游戏地图内
    private void FixBound()
    {
        // 屏幕中心不能超出放置区
        float left = LeftBottomPoint.x;
        float right = RightTopPoint.x;
        float bottom = LeftBottomPoint.y;
        float top = RightTopPoint.y;

        if (transform.position.x > right)
        {
            transform.position = new Vector3(right, transform.position.y, CAMERA_Z);
        }
        else if (transform.position.x < left)
        {
            transform.position = new Vector3(left, transform.position.y, CAMERA_Z);
        }
        if (transform.position.y > top)
        {
            transform.position = new Vector3(transform.position.x, top, CAMERA_Z);
        }
        else if (transform.position.y < bottom)
        {
            transform.position = new Vector3(transform.position.x, bottom, CAMERA_Z);
        }
    }

    // 变化Follow
    public void ToggleFollow()
    {
        follow = !follow;
    }
}