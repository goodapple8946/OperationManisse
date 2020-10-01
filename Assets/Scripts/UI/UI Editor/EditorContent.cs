using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Controller;

public class EditorContent : MonoBehaviour
{
    private GameObject editorUnit;
    private GameObject editorBackground;

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
                case "Editor Background":
                    editorBackground = obj;
                    break;
            }
        }
    }

    public void UpdateUIShowing()
    {
        UpdateByEditorMode();
        BroadcastMessage("UpdateShowing");
    }

    public void UpdateByEditorMode()
    {
        editorUnit.SetActive(editorController.EditorMode == EditorMode.Unit);
        editorBackground.SetActive(editorController.EditorMode == EditorMode.Background);
    }
}
