using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopController : MonoBehaviour
{
    private GameObject content;

    public GameObject[] blockObjects;
    public GameObject[] ballObjects;
    private GameObject[] gameObjects;

    public GameObject shopObjectPrefab;

    private PreparationController preparationController;

    // 每个物品占据的高度
    private float goodsHeight = 150f;

    // 每个物品显示的尺寸比例
    private float goodsScale = 1.5f;

    void Awake()
    {
        preparationController = GameObject.Find("Preparation Controller").GetComponent<PreparationController>();

        content = transform.GetChild(0).GetChild(0).gameObject;

        // 将blockObjects与ballObjects合并为gameObjects，因此在商店中的位置block排在ball前面
        gameObjects = new GameObject[blockObjects.Length + ballObjects.Length];
        blockObjects.CopyTo(gameObjects, 0);
        ballObjects.CopyTo(gameObjects, blockObjects.Length);
    }

    void Start()
    {
        float offset = -goodsHeight / 2;

        // 为每个添加的物体
        foreach (GameObject gameObject in gameObjects)
        {
            // 创建商品实例
            GameObject shopObject = Instantiate(shopObjectPrefab);

            // 将商品设为Content的子物体
            shopObject.transform.SetParent(content.transform);

            // 商品图像为添加物体的图像
            shopObject.GetComponent<Button>().image.sprite = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            
            // 商品使用默认图像尺寸
            shopObject.GetComponent<Button>().image.SetNativeSize();
           
            // 商品点击事件：购买添加的物体
            shopObject.GetComponent<Button>().onClick.AddListener(() => preparationController.Buy(gameObject));
            
            // 商品移动到指定位置
            shopObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, offset);
           
            // 商品按照比例缩放
            shopObject.GetComponent<RectTransform>().localScale = new Vector2(goodsScale, goodsScale);
           
            // 根据添加物体的子物体设置商品子物体的图像与尺寸
            for (int i = 1; i < gameObject.transform.childCount; i++) 
            {
                if (gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>() != null)
                {
                    shopObject.transform.GetChild(i - 1).GetComponent<Image>().sprite = gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
                    shopObject.transform.GetChild(i - 1).GetComponent<Image>().SetNativeSize();
                }
            }
            
            // 可滑动区域的高度增加一个商品的高度
            transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta += new Vector2(0, goodsHeight);

            offset -= goodsHeight;
        }
    }
}
