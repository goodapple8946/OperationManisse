using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static Controller;

public class ClickableObject : MonoBehaviour
{
    protected virtual void OnMouseOver()
    {
        // 鼠标不在UI上
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            // 鼠标左键按下
            if (Input.GetMouseButtonDown(0))
            {
                editorController.LeftClick(this);
            }

            // 鼠标左键按住
            if (Input.GetMouseButton(0))
            {
                editorController.LeftClick(this, true);
            }

            // 鼠标右键按下
            if (Input.GetMouseButtonDown(1))
            {
                editorController.RightClick(this);
            }

            // 鼠标右键按住
            if (Input.GetMouseButton(1))
            {
                editorController.RightClick(this, true);
            }
        }
    }
}
