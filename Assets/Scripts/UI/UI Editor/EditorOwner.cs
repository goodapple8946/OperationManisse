using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameController;

public class EditorOwner : MonoBehaviour
{
    private Dropdown dropdown;
    private EditorController editorController;

    public void Awake()
    {
        editorController = GameObject.Find("Editor Controller").GetComponent<EditorController>();
        dropdown = GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(value =>
        {
            Player player = (Player)value;
            editorController.playerOwner = player;
        });
    }

    void Update()
    {
        dropdown.SetValueWithoutNotify((int)editorController.playerOwner);
    }
}