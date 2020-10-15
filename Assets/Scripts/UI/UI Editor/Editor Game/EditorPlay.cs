using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorPlay : EditorUI
{
    private Button button;

    public void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            gameController.GamePhase = GamePhase.Preparation;
        });

        tipTitle = "Play";
        tipContent = "  Try your work!";
    }
}
