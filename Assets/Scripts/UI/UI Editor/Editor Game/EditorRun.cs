using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorRun : MonoBehaviour
{
    Button button;

    public void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(()=>
        {
            gameController.Run();
        });
    }
}
