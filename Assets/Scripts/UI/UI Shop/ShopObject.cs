using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

/// <summary>
/// 商店里面的物品按钮
/// </summary>
public class ShopObject : MonoBehaviour
{
    // 每个物品显示的尺寸比例
    private float goodsScale = 1.5f;

    // 物品是显示的（编辑模式下强制显示）
    private bool isVisible;
    public bool IsVisible
    {
        get => isVisible;
        set
        {
            isVisible = value;
            toggleObject.GetComponent<Toggle>().isOn = isVisible;
        }
    }

    // 购买时实例化的物体
    public ClickableObject clickableObject;

    // Toggle
    private GameObject toggleObject;

    void Awake()
    {
        toggleObject = transform.GetChild(1).gameObject;
    }

    public void Init(GameObject gameObject)
    {
        clickableObject = gameObject.GetComponent<ClickableObject>();

        if (clickableObject is Unit)
        {
            // 商品图像为添加物体的图像
            GetComponent<Button>().image.sprite = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            
            // 商品使用默认图像尺寸
            GetComponent<Button>().image.SetNativeSize();
        }
        else if (clickableObject is Background || clickableObject is TerrainA)
        {
            // 商品图像为添加物体的图像
            GetComponent<Button>().image.sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
			GetComponent<Button>().image.color = gameObject.GetComponent<SpriteRenderer>().color;

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
        GetComponent<Button>().onClick.AddListener(() => editorController.Buy(gameObject));

        // 商品按照比例缩放
        GetComponent<RectTransform>().localScale = new Vector2(goodsScale, goodsScale);

        // 根据添加物体的子物体设置商品子物体的图像与尺寸
        for (int i = 1; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>() != null)
            {
                transform.GetChild(0).GetChild(i - 1).GetComponent<Image>().sprite = gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
				transform.GetChild(0).GetChild(i - 1).GetComponent<Image>().color = gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().color;
				transform.GetChild(0).GetChild(i - 1).GetComponent<Image>().SetNativeSize();
            }
        }
    }

    // 显示或隐藏商品的显示按钮
    public void UpdateToggle(GamePhase gamePhase)
    {
        if (clickableObject is Unit)
        {
            toggleObject.SetActive(gamePhase == GamePhase.Editor);
        }
        else
        {
            toggleObject.SetActive(false);
        }
    }
}
