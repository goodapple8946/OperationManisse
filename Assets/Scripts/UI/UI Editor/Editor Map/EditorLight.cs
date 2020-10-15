using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Controller;

public class EditorLight : EditorUI
{
    Slider slider;

    public void Awake()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(value =>
        {
			editorController.LightIntensity = value;
        });

        tipTitle = "Map Global Light";
        tipContent = "  The intensity of light falling on all objects.";
    }

	public override void UpdateShowing()
    {
        slider.SetValueWithoutNotify(editorController.LightIntensity);
	}
}
