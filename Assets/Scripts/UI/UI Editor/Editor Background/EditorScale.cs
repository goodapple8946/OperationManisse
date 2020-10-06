using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorScale : MonoBehaviour
{
    Slider slider;

    public void Awake()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(value =>
        {
            editorController.BackgroundScale = value;
        });
    }

    public void UpdateShowing()
    {
        slider.value = editorController.BackgroundScale;
    }
}
