using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class ShopController : MonoBehaviour
{
    // 每个物品占据的高度
    private static readonly float GOODS_HEIGHT = 150f;

    private GameObject content;

	// prefab
    [HideInInspector] public GameObject[] gameObjects;

    public GameObject shopObjectPrefab;

	// 所有的商店中的物品
    private ShopObject[] shopObjects = new ShopObject[0];

    void Awake()
    {
        content = GameObject.Find("UI Canvas/UI Shop/Viewport/Content");

        // 将editorObjects、blockObjects、ballObjects合并为gameObjects，因此在商店中排序为editor、block、ball
        gameObjects = new GameObject[resourceController.terrainObjects.Length + resourceController.blockObjects.Length + resourceController.ballObjects.Length + resourceController.backgroundObjects.Length];
        resourceController.terrainObjects.CopyTo(gameObjects, 0);
        resourceController.blockObjects.CopyTo(gameObjects, resourceController.terrainObjects.Length);
        resourceController.ballObjects.CopyTo(gameObjects, resourceController.terrainObjects.Length + resourceController.blockObjects.Length);
        resourceController.backgroundObjects.CopyTo(gameObjects, resourceController.terrainObjects.Length + resourceController.blockObjects.Length + resourceController.ballObjects.Length);
    }

    void Start()
    {
        ArrayList arr = new ArrayList();

        // 为每个添加的物体
        foreach (GameObject obj in gameObjects)
        {
            // 创建商品实例，并且父物体设为Content
            GameObject clone = Instantiate(shopObjectPrefab, content.transform);
            clone.name = obj.name;

            // 商品初始化
            clone.GetComponent<ShopObject>().Init(obj);

            arr.Add(clone.GetComponent<ShopObject>());
        }
        shopObjects = (ShopObject[])arr.ToArray(typeof(ShopObject));
        UpdateShop(GamePhase.Editor);
    }

    // 根据商品可见性更新商店
    public void UpdateShop(GamePhase gamePhase)
    {
        // 显示商品的计数
        int count = 0;
        foreach (ShopObject shopObject in shopObjects)
        {
            // 应用商品是否显示
            bool active = false;
            if (gamePhase == GamePhase.Editor)
            {
                active |= editorController.EditorMode == EditorMode.Unit && shopObject.clickableObject is Unit;
                active |= editorController.EditorMode == EditorMode.Background && shopObject.clickableObject is Background;
                active |= editorController.EditorMode == EditorMode.Terrain && shopObject.clickableObject is TerrainA;
            }
            else if (gamePhase == GamePhase.Preparation)
            {
                active |= shopObject.clickableObject is Unit && shopObject.IsVisible;
            }

            if (active)
            {
                shopObject.gameObject.SetActive(true);
                shopObject.UpdateToggle(gamePhase);
                count++;
            }
            else
            {
                shopObject.gameObject.SetActive(false);
            }
        }

        // 更新可滑动区域的高度
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, GOODS_HEIGHT * count);
    }

    // 设置商品可见性：提供一个名称数组，若商品名称在该数组内，则可见；否则不可见
    public void SetShopObjectVisibility(List<string> names)
    {
        foreach (ShopObject shopObject in shopObjects)
        {
            shopObject.IsVisible = names.Contains(shopObject.gameObject.name);
        }
        UpdateShop(gameController.GamePhase);
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
