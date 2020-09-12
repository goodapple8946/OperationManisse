using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Vector3 originPosition = new Vector3(0f, 3.2f, -10);

    public Core core;

    void Update()
    {
        FollowCore();
    }

    // 跟随核心
    void FollowCore()
    {
        if (core != null && GameController.gamePhase == GameController.GamePhase.Playing)
        {
            gameObject.transform.position = new Vector3(core.transform.position.x, core.transform.position.y, -10);
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
