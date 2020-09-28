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

	// prefab
    private GameObject[] gameObjects;

    public GameObject shopObjectPrefab;

	// 所有的商店中的物品
    private ShopObject[] shopObjects;

    private ResourceController resourceController;

    void Awake()
    {
		resourceController = GameObject.Find("Resource Controller").GetComponent<ResourceController>();
		content = transform.GetChild(0).GetChild(0).gameObject;

        // 将editorObjects、blockObjects、ballObjects合并为gameObjects，因此在商店中排序为editor、block、ball

        gameObjects = new GameObject[resourceController.editorObjects.Length + resourceController.blockObjects.Length + resourceController.ballObjects.Length];
        resourceController.editorObjects.CopyTo(gameObjects, 0);
        resourceController.blockObjects.CopyTo(gameObjects, resourceController.editorObjects.Length);
        resourceController.ballObjects.CopyTo(gameObjects, resourceController.editorObjects.Length + resourceController.blockObjects.Length);
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

    // 设置商品可见性：提供一个名称数组，若商品名称在该数组内，则可见；否则不可见
    public void SetShopObjectVisibility(List<string> names)
    {
        foreach (ShopObject shopObject in shopObjects)
        {
            shopObject.isVisible = names.Contains(shopObject.name);
        }
        UpdateShop();
    }

    // 获得商品可见性：返回一个所有可见的商品名称数组
    public List<string> GetShopObjectVisibility()
    {
        List<string> ret = new List<string>();
        foreach (ShopObject shopObject in shopObjects)
        {
            if (shopObject.isVisible)
            {
                ret.Add(shopObject.name);
            }
        }
        return ret;
    }
}
