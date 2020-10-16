using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Controller;

public class EditorQuit : EditorUI
{
    private Button button;

    public void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Scene Menu");
        });

        tipTitle = "Quit";
        tipContent = "  Quit the editor.";
    }
}
