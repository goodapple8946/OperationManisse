using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMoney : MonoBehaviour
{
    private EditorController editorController;
    private Text text;

    void Awake()
    {
        editorController = GameObject.Find("Editor Controller").GetComponent<EditorController>();
        text = GetComponent<Text>();
    }

    void FixedUpdate()
    {
        int textMoney = int.Parse(text.text);
        int playerMoney = editorController.playerMoney;
        int difference = Mathf.Abs(playerMoney - textMoney);

        // 每次变化值
        int deltaMoney = 1;

        if (difference > 10000)
        {
            deltaMoney = 10000;
        }
        else if (difference > 1000)
        {
            deltaMoney = 1000;
        }
        else if (difference > 100)
        {
            deltaMoney = 100;
        }
        else if (difference > 10)
        {
            deltaMoney = 10;
        }

        if (textMoney < playerMoney)
        {
            text.text = (textMoney + deltaMoney).ToString();
        }
        else if (textMoney > playerMoney)
        {
            text.text = (textMoney - deltaMoney).ToString();
        }
    }
}
