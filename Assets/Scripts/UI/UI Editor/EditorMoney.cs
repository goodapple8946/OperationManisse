using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorMoney : MonoBehaviour
{
    InputField inputField;

    public void Awake()
    {
        inputField = GetComponent<InputField>();
        inputField.onValueChanged.AddListener(value => 
        {
            if(value != "" && value != "-")
            {
                int money = int.Parse(value);
                editorController.PlayerMoneyOrigin = money;
            }
        });
    }

	public void UpadteShowing()
	{
		inputField.text = editorController.PlayerMoneyOrigin + "";
	}
}
