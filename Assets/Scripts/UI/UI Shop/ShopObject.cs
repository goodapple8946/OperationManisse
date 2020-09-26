using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopObject : MonoBehaviour
{
    // 每个物品显示的尺寸比例
    private float goodsScale = 1.5f;

    private EditorController editorController;

    void Awake()
    {
        editorController = GameObject.Find("Editor Controller").GetComponent<EditorController>();
    }

    public void Init(GameObject gameObject)
    {
        // 商品图像为添加物体的图像
        GetComponent<Button>().image.sprite = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;

        // 商品使用默认图像尺寸
        GetComponent<Button>().image.SetNativeSize();

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
                transform.GetChild(0).GetChild(i - 1).GetComponent<Image>().SetNativeSize();
            }
        }
    }
}
