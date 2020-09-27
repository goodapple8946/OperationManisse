using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameController;

public class ShopController : MonoBehaviour
{
    // 每个物品占据的高度
    private float goodsHeight = 150f;

    private GameObject content;

    public GameObject[] editorObjects;
    public GameObject[] blockObjects;
    public GameObject[] ballObjects;
    private GameObject[] gameObjects;

    public GameObject shopObjectPrefab;

    private ShopObject[] shopObjects;

    void Awake()
    {
        content = transform.GetChild(0).GetChild(0).gameObject;

        // 将editorObjects、blockObjects、ballObjects合并为gameObjects，因此在商店中排序为editor、block、ball
        gameObjects = new GameObject[editorObjects.Length + blockObjects.Length + ballObjects.Length];
        editorObjects.CopyTo(gameObjects, 0);
        blockObjects.CopyTo(gameObjects, editorObjects.Length);
        ballObjects.CopyTo(gameObjects, editorObjects.Length + blockObjects.Length);
    }

    void Start()
    {
        ArrayList arr = new ArrayList();

        // 为每个添加的物体
        foreach (GameObject gameObject in gameObjects)
        {
            // 创建商品实例，并且父物体设为Content
            GameObject obj = Instantiate(shopObjectPrefab, content.transform);

            // 商品初始化
            obj.GetComponent<ShopObject>().Init(gameObject);

            // 可滑动区域的高度增加一个商品的高度
            transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta += new Vector2(0, goodsHeight);

            arr.Add(obj.GetComponent<ShopObject>());
        }
        shopObjects = (ShopObject[])arr.ToArray(typeof(ShopObject));
    }

    // 根据商品可见性更新商店
    public void UpdateShop()
    {
        // 显示商品的计数
        int count = 0;
        foreach (ShopObject shopObject in shopObjects)
        {
            // 应用商品是否显示
            if (shopObject.isVisible || gamePhase == GamePhase.Editor)
            {
                shopObject.gameObject.SetActive(true);
                count++;
            }
            else
            {
                shopObject.gameObject.SetActive(false);
            }

            // 显示或隐藏商品的显示按钮
            shopObject.transform.GetChild(1).gameObject.SetActive(gamePhase == GamePhase.Editor);
        }

        // 更新可滑动区域的高度
        transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(0, goodsHeight * count);
    }
}
