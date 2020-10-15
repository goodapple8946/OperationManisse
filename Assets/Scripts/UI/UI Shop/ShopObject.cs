using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Controller;

/// <summary>
/// 商店里面的物品按钮
/// </summary>
public class ShopObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // 每个商品显示的尺寸比例
    private float goodsScale = 1.5f;

    // 商品是显示的（编辑模式下强制显示）
    private bool isVisible;
    public bool IsVisible
    {
        get => isVisible;
        set
        {
            isVisible = value;
            toggleObj.GetComponent<Toggle>().isOn = isVisible;
        }
    }

    // 购买时实例化的物体
    [HideInInspector] public ClickableObject clickableObject;

    // 商品提示条
    private Tooltip tooltip;

    // Toggle
    private GameObject toggleObj;

    private void Awake()
    {
        toggleObj = transform.GetChild(1).gameObject;

        // 商品提示条
        tooltip = GameObject.Find("UI Canvas/UI Tooltip").GetComponent<Tooltip>();
    }

    public void Init(GameObject obj)
    {
        clickableObject = obj.GetComponent<ClickableObject>();

        if (clickableObject is Unit)
        {
            // 商品图像为添加物体的图像
            GetComponent<Button>().image.sprite = obj.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            
            // 商品使用默认图像尺寸
            GetComponent<Button>().image.SetNativeSize();
        }
        else if (clickableObject is Background || clickableObject is TerrainA)
        {
            // 商品图像为添加物体的图像
            GetComponent<Button>().image.sprite = obj.GetComponent<SpriteRenderer>().sprite;
			GetComponent<Button>().image.color = obj.GetComponent<SpriteRenderer>().color;

			// 商品使用默认图像尺寸
			GetComponent<Button>().image.SetNativeSize();

            // 强制尺寸修正
            float fixedSize = 64f;
            GetComponent<Button>().image.rectTransform.sizeDelta = new Vector2(fixedSize, fixedSize);
        }
        else
        {
            throw new Exception("Unknown shop object type.");
        }

        // 商品点击事件：购买添加的物体
        GetComponent<Button>().onClick.AddListener(() => editorController.Buy(obj));

        // 商品按照比例缩放
        GetComponent<RectTransform>().localScale = new Vector2(goodsScale, goodsScale);

        // 根据添加物体的子物体设置商品子物体的图像与尺寸
        for (int i = 1; i < obj.transform.childCount; i++)
        {
            if (obj.transform.GetChild(i).GetComponent<SpriteRenderer>() != null)
            {
                transform.GetChild(0).GetChild(i - 1).GetComponent<Image>().sprite = obj.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
				transform.GetChild(0).GetChild(i - 1).GetComponent<Image>().color = obj.transform.GetChild(i).GetComponent<SpriteRenderer>().color;
				transform.GetChild(0).GetChild(i - 1).GetComponent<Image>().SetNativeSize();
            }
        }
    }

    // 鼠标移入，显示提示条
    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.Show(clickableObject.name, GetObjDescription(clickableObject));
    }

    // 鼠标移除，隐藏提示条
    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Hide();
    }

    // 显示或隐藏商品的显示按钮
    public void UpdateToggle(GamePhase gamePhase)
    {
        if (clickableObject is Unit)
        {
            toggleObj.SetActive(gamePhase == GamePhase.Editor);
        }
        else
        {
            toggleObj.SetActive(false);
        }
    }

    // 商品描述
    private string GetObjDescription(ClickableObject clickableObject)
    {
        if (clickableObject is Unit)
        {
            return GetObjDescription(clickableObject as Unit);
        }
        else
        {
            return clickableObject.info;
        }
    }

    // 单位商品描述
    private string GetObjDescription(Unit unit)
    {
        string ret = "";
        ret += "Price: " + unit.price + "\n";
        ret += unit.info;
        return ret;
    }
}
