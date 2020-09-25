using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    // 可移动的
    public bool movable = true;

    // 屏幕跟随
    public bool follow;

    // 屏幕边界
    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;

    private GameController gameController;

	// 记录上一次镜头跟player core需要的偏移
	private Vector3 followOffset;

    void Awake()
    {
        mainCamera = GetComponent<Camera>();
        gameController = GameObject.Find("Game Controller").GetComponent<GameController>();
    }

	void Start()
    {
		Init();
    }

    void Update()
    {
		bool mouseOutOfScreen = (GetMouseDirection() != Vector2.zero);
		if (mouseOutOfScreen)
		{
			follow = false;
		}

        if (movable)
        {
            if (follow)
            {
                Zoom();
                Follow();
                FixBound();
            }
            else
            {
                Zoom();
                Scroll();
                FixBound();
            }
        }
    }

    // 初始化Camera
    public void Init()
    {
        transform.position = originPosition;
        followOffset = new Vector3(0, 0, 0);
    }

    // 跟随玩家
    private void Follow()
    {
        Vector2 center = Vector2.zero;
        Vector2 velocity = Vector2.zero;

        bool canFollow = gameController.CenterOfPlayerObjects(out center) && gameController.VelocityOfPlayerObjects(velocity);
        if (canFollow)
        {
            Vector3 cameraDepth = new Vector3(0, 0, cameraZ);
            Vector3 newOffset = CalcuteFollowOffset(velocity);

            // 维护更新followOffset
            followOffset = newOffset;

            // 设置摄像机位置
            transform.position = (Vector3)center + cameraDepth + newOffset;
        }
        
    }

	// 缩放
	private void Zoom()
    {
        // 鼠标不在UI上
        if (!EventSystem.current.IsPointerOverGameObject())
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
    }

	// 滚转
	private void Scroll()
    {
		Vector2 dir = GetMouseDirection();
		transform.Translate(scrollSpeed * Time.deltaTime * dir);
    }

	// 将摄像头限制在游戏地图内
	private void FixBound()
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

	private Vector2 CalcuteFollowOffset(Vector2 velocity)
	{
		// 获取当前镜头屏幕的一半长，一半宽
		float cameraHeight = mainCamera.orthographicSize;
		float cameraWeight = mainCamera.orthographicSize * mainCamera.aspect;

		// 获取玩家核心的运动方向向量
		Vector2 moveDirection = velocity.normalized;

		// 采取两者中小的,以获得平滑的效果
		float weightX = Mathf.Abs(velocity.x) < Mathf.Abs(moveDirection.x) ? 
			velocity.x : moveDirection.x;
		float weightY = Mathf.Abs(velocity.y) < Mathf.Abs(moveDirection.y) ?
			velocity.y : moveDirection.y;

		// 采用与屏幕size相关的较小量,以获得平滑的效果
		float offsetFactor = 0.001f;
		offsetFactor *= Mathf.Sqrt(cameraHeight * cameraHeight + cameraWeight * cameraWeight);
		Vector2 newFollowOffset = new Vector2(
			followOffset.x + offsetFactor * weightX,
			followOffset.y + offsetFactor * weightY);

		// 如果玩家核心快要超出镜头，就限制offset到(factor*cameraWeight, factor*cameraHeight)的镜头内
		float coreZoneFactor = 0.6f;
		newFollowOffset = ClipIntoSquare(
			newFollowOffset, cameraWeight * coreZoneFactor, cameraHeight * coreZoneFactor);
		return newFollowOffset;
	}

	// 若镜头相对核心偏移超过cameraHeight,cameraWeight,也即看不到玩家核心
	// 将偏移收缩至([-weight, height], [-weight, height])
	private Vector2 ClipIntoSquare(Vector2 vect, float weight, float height)
	{
		if (Mathf.Abs(vect.x) >= weight)
		{
			vect.x = vect.x >= 0 ? weight : -weight;
		}
		if (Mathf.Abs(vect.y) >= height)
		{
			vect.y = vect.y >= 0 ? height : -height;
		}
		return vect;
	}

	// 鼠标接近屏幕边缘触发的滚珠方向向量,
	// 右(1,0)左(-1,0)上(0,1)下(0,-1)
	// 不存在滚转为(0,0)
	private Vector2 GetMouseDirection()
	{
		Vector2 dir = new Vector2(0, 0);
		// 向右
		if (Input.mousePosition.x > Screen.width - scrollDistance && transform.position.x <= xMax)
		{
			dir += new Vector2(1, 0);
		}
		// 向左
		else if (Input.mousePosition.x < scrollDistance && transform.position.x >= xMin)
		{
			dir += new Vector2(-1, 0);
		}

		// 向上
		if (Input.mousePosition.y > Screen.height - scrollDistance && transform.position.y <= yMax)
		{
			dir += new Vector2(0, 1);
		}
		// 向下
		else if (Input.mousePosition.y < scrollDistance && transform.position.y >= yMin)
		{
			dir += new Vector2(0, -1);
		}
		return dir;
	}
}
