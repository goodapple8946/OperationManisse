using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        // 为每个添加的物体
        foreach (GameObject gameObject in gameObjects)
        {
            // 创建商品实例，并且父物体设为Content
            GameObject shopObject = Instantiate(shopObjectPrefab, content.transform);
            
            // 商品初始化
            shopObject.GetComponent<ShopObject>().Init(gameObject);

            // 可滑动区域的高度增加一个商品的高度
            transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta += new Vector2(0, goodsHeight);
        }
    }
}
