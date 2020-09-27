using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOption : MonoBehaviour
{
    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(()=>
        {
            GameObject objDropdown = transform.GetChild(0).gameObject;
            objDropdown.SetActive(!objDropdown.activeSelf);
        });
    }
}
