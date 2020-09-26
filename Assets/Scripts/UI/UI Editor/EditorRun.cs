using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorRun : MonoBehaviour
{
    Button button;

    public void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(()=>
        {
            GameObject.Find("Game Controller").GetComponent<GameController>().Run();
        });
    }
}
