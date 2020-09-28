using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Controller;

public class EditorContent : MonoBehaviour
{
    private GameObject editorUnit;
    private GameObject editorMap;

    void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject obj = transform.GetChild(i).gameObject;
            switch (obj.name)
            {
                case "Editor Unit":
                    editorUnit = obj;
                    break;
                case "Editor Map":
                    editorMap = obj;
                    break;
            }
        }
    }

    void Update()
    {

    }
}
