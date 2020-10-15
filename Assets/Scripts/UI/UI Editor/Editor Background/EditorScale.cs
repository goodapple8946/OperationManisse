using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorScale : EditorUI
{
    Slider slider;

    public void Awake()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(value =>
        {
            editorController.BackgroundScale = value;
        });

        tipTitle = "Background Scale";
        tipContent = "  Change the size of the background selected now or last time.";
    }

    public override void UpdateShowing()
    {
        slider.value = editorController.BackgroundScale;
    }
}
