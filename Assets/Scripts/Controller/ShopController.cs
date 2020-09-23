using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShopController : MonoBehaviour
{
    private GameObject content;

    public GameObject[] gameObjects;

    public GameObject shopObjectPrefab;

    private PreparationController preparationController;

    void Awake()
    {
        preparationController = GameObject.Find("Preparation Controller").GetComponent<PreparationController>();

        content = transform.GetChild(0).GetChild(0).gameObject;
    }

    void Start()
    {
        float offset = -75f;
        foreach (GameObject gameObject in gameObjects)
        {
            GameObject shopObject = Instantiate(shopObjectPrefab);
            shopObject.transform.SetParent(content.transform);
            shopObject.GetComponent<Button>().image.sprite = gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
            shopObject.GetComponent<Button>().image.SetNativeSize();
            shopObject.GetComponent<Button>().onClick.AddListener(() => preparationController.Buy(gameObject));
            shopObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, offset);
            shopObject.GetComponent<RectTransform>().localScale = new Vector2(1.5f, 1.5f);
            for (int i = 1; i < gameObject.transform.childCount; i++) 
            {
                shopObject.transform.GetChild(i - 1).GetComponent<Image>().sprite = gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
                shopObject.transform.GetChild(i - 1).GetComponent<Image>().SetNativeSize();
            }
            transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta += new Vector2(0, 150f);
            offset -= 150f;
        }
    }
}
