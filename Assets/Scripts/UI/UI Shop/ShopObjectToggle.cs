using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopObjectToggle : MonoBehaviour
{
    Toggle toggle;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(value =>
        {
            transform.parent.GetComponent<ShopObject>().IsVisible = value;
        });
    }
}
