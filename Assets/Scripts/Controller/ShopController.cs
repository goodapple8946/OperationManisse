using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class ShopController : MonoBehaviour
{
    // 每个物品占据的高度
    private float goodsHeight = 150f;

    private GameObject content;

	// prefab
    public GameObject[] gameObjects;

    public GameObject shopObjectPrefab;

	// 所有的商店中的物品
    private ShopObject[] shopObjects;

    void Awake()
    {
        content = GameObject.Find("UI Canvas/UI Shop/Viewport/Content");

        // 将editorObjects、blockObjects、ballObjects合并为gameObjects，因此在商店中排序为editor、block、ball
        gameObjects = new GameObject[resourceController.editorObjects.Length + resourceController.blockObjects.Length + resourceController.ballObjects.Length + resourceController.backgroundObjects.Length];
        resourceController.editorObjects.CopyTo(gameObjects, 0);
        resourceController.blockObjects.CopyTo(gameObjects, resourceController.editorObjects.Length);
        resourceController.ballObjects.CopyTo(gameObjects, resourceController.editorObjects.Length + resourceController.blockObjects.Length);
        resourceController.backgroundObjects.CopyTo(gameObjects, resourceController.editorObjects.Length + resourceController.blockObjects.Length + resourceController.ballObjects.Length);
    }

    void Start()
    {
        ArrayList arr = new ArrayList();

        // 为每个添加的物体
        foreach (GameObject gameObject in gameObjects)
        {
            // 创建商品实例，并且父物体设为Content
            GameObject obj = Instantiate(shopObjectPrefab, content.transform);
            obj.name = gameObject.name;

            // 商品初始化
            obj.GetComponent<ShopObject>().Init(gameObject);

            arr.Add(obj.GetComponent<ShopObject>());
        }
        shopObjects = (ShopObject[])arr.ToArray(typeof(ShopObject));
        UpdateShop();
    }

    // 根据商品可见性更新商店
    public void UpdateShop()
    {
        // 显示商品的计数
        int count = 0;
        foreach (ShopObject shopObject in shopObjects)
        {
            // 应用商品是否显示
            bool active = false;
            if (gameController.gamePhase == GamePhase.Editor)
            {
                active |= editorController.EditorMode == EditorMode.Unit && shopObject.clickableObject is Unit;
                active |= editorController.EditorMode == EditorMode.Background && shopObject.clickableObject is Background;
            }
            else if (gameController.gamePhase == GamePhase.Preparation)
            {
                active |= shopObject.clickableObject is Unit && shopObject.IsVisible;
            }

            if (active)
            {
                shopObject.gameObject.SetActive(true);
                shopObject.UpdateToggle();
                count++;
            }
            else
            {
                shopObject.gameObject.SetActive(false);
            }
        }

        // 更新可滑动区域的高度
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, goodsHeight * count);
    }

    // 设置商品可见性：提供一个名称数组，若商品名称在该数组内，则可见；否则不可见
    public void SetShopObjectVisibility(List<string> names)
    {
        foreach (ShopObject shopObject in shopObjects)
        {
            shopObject.IsVisible = names.Contains(shopObject.gameObject.name);
        }
        UpdateShop();
    }

    // 获得商品可见性：返回一个所有可见的商品名称数组
    public List<string> GetShopObjectVisibility()
    {
        List<string> ret = new List<string>();
        foreach (ShopObject shopObject in shopObjects)
        {
            if (shopObject.IsVisible)
            {
                ret.Add(shopObject.gameObject.name);
            }
        }
        return ret;
    }
}
